import os
import pandas as pd
import gzip
import json
from minio import Minio
from pymongo import MongoClient

# MinIO and MongoDB Configurations
MINIO_URL = "localhost:9000"
MINIO_ACCESS_KEY = "minioadmin"
MINIO_SECRET_KEY = "minioadmin"
BUCKET_NAME = "baket"

MONGO_URI = "mongodb+srv://enoo:enoeno2100@pppk.m7gqhqc.mongodb.net/?retryWrites=true&w=majority&appName=pppk"
DB_NAME = "pppk"
COLLECTION_NAME = "pppk"

GENES_OF_INTEREST = {"C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9", "CXCL11", "NFKB1", "IKBKE", "IRF3", "TREX1", "ATM", "IL6", "IL8"}
DATA_DIR = os.path.join(os.path.dirname(__file__), "data")  # Save inside the same directory as the script

# Ensure the data folder exists
os.makedirs(DATA_DIR, exist_ok=True)

# Initialize Clients
minio_client = Minio(
    MINIO_URL,
    access_key=MINIO_ACCESS_KEY,
    secret_key=MINIO_SECRET_KEY,
    secure=False
)

mongo_client = MongoClient(MONGO_URI)
db = mongo_client[DB_NAME]
collection = db[COLLECTION_NAME]


# Download and Process Gene Expression Data
def process_gene_expression():
    print("\nFetching objects from MinIO bucket...")
    objects = minio_client.list_objects(BUCKET_NAME, recursive=True)
    
    for obj in objects:
        if not obj.object_name.endswith(".tsv.gz"):
            continue
        
        cohort = obj.object_name.split(".")[0]  # Extract cohort from filename
        local_path = os.path.join(DATA_DIR, obj.object_name)  # Save inside `data/`

        print(f"\u2913 Downloading {obj.object_name} -> {local_path}")
        minio_client.fget_object(BUCKET_NAME, obj.object_name, local_path)
        
        with gzip.open(local_path, 'rt') as f:
            df = pd.read_csv(f, sep='\t')

        if 'sample' not in df.columns:
            print(f"Skipping {cohort} - 'sample' column missing.")
            os.remove(local_path)
            continue

        df = df[df['sample'].str.startswith("TCGA")]  # Keep only TCGA patient data
        df = df.set_index('sample')

        # Extract only genes of interest
        df = df[df.index.isin(GENES_OF_INTEREST)].T
        df.reset_index(inplace=True)
        df.rename(columns={'index': 'patient_id'}, inplace=True)
        df["cancer_cohort"] = cohort

        # Store in MongoDB
        records = df.to_dict(orient="records")
        if records:
            collection.insert_many(records)
            print(f"Inserted {len(records)} records for {cohort}.")
        else:
            print(f"No relevant gene data for {cohort}.")

        os.remove(local_path)  # Clean up local file


# Download clinical data if missing
def get_clinical_data():
    local_clinical_path = os.path.join(DATA_DIR, "TCGA_clinical_survival_data.tsv")

    if not os.path.exists(local_clinical_path):
        print("\nClinical data file missing. Checking MinIO storage...")
        objects = [obj.object_name for obj in minio_client.list_objects(BUCKET_NAME, recursive=True)]
        
        if "TCGA_clinical_survival_data.tsv" in objects:
            print(f"\u2913 Downloading TCGA_clinical_survival_data.tsv -> {local_clinical_path}")
            minio_client.fget_object(BUCKET_NAME, "TCGA_clinical_survival_data.tsv", local_clinical_path)
        else:
            print("Clinical data not found in MinIO. Ensure it is uploaded.")
            return None

    return local_clinical_path


# Merge with Clinical Data
def merge_with_clinical():
    clinical_path = get_clinical_data()
    if not clinical_path:
        return

    print("\nMerging gene expression data with clinical data...")
    clinical_data = pd.read_csv(clinical_path, sep='\t')

    updated_count = 0
    for record in collection.find():
        patient_id = record['patient_id']
        clinical_info = clinical_data[clinical_data['bcr_patient_barcode'] == patient_id]

        if not clinical_info.empty:
            update_fields = {
                "DSS": int(clinical_info["DSS"].values[0]),
                "OS": int(clinical_info["OS"].values[0]),
                "clinical_stage": clinical_info["clinical_stage"].values[0]
            }
            collection.update_one({"patient_id": patient_id}, {"$set": update_fields})
            updated_count += 1

    print(f"Merged clinical data for {updated_count} patients.")


# Fetch Data for Visualization
def fetch_patient_data(patient_id):
    patient_data = collection.find_one({"patient_id": patient_id})
    if patient_data:
        print("\nPatient Data Found:")
        print(json.dumps(patient_data, indent=4))
    else:
        print(f"No data found for patient: {patient_id}")


if __name__ == "__main__":
    print("Starting Data Processing Pipeline...\n")
    
    process_gene_expression()
    merge_with_clinical()

    print("\nAll data successfully processed and stored in MongoDB.")