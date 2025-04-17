from minio import Minio
import pandas as pd
import gzip
import io
import os

DATA_DIR = os.path.join(os.path.dirname(__file__), "data")
SURVIVAL_FILE = os.path.join(DATA_DIR, "TCGA_clinical_survival_data.tsv")

MINIO_URL        = "localhost:9000"
MINIO_ACCESS_KEY = "minioadmin"
MINIO_SECRET_KEY = "minioadmin"
BUCKET_NAME      = "baket"
OUTPUT_NAME      = "_merged_gene_clinical_data.tsv"

client = Minio(
    MINIO_URL,
    access_key=MINIO_ACCESS_KEY,
    secret_key=MINIO_SECRET_KEY,
    secure=False
)

# Check survival file exists
if not os.path.exists(SURVIVAL_FILE):
    print("TCGA_clinical_survival_data.tsv not found!")
    exit()

# Fetch list of gene files in bucket
print("\nFetching data from MinIO…")
objects    = client.list_objects(BUCKET_NAME, prefix="", recursive=True)
gene_files = [obj.object_name for obj in objects if obj.object_name.endswith(".tsv.gz")]
print(f"  → found {len(gene_files)} gene files\n")

# Load clinical data
clinical_df = pd.read_csv(
    SURVIVAL_FILE,
    sep="\t",
    usecols=["bcr_patient_barcode", "DSS", "OS", "clinical_stage"]
)

# Stream‐process each gene file, merge, and append to disk
out_path = os.path.join(DATA_DIR, OUTPUT_NAME)

for i, file in enumerate(gene_files, start=1):
    print(f"[{i}/{len(gene_files)}] Processing {file}…", flush=True)

    # Stream download & decompress
    with client.get_object(BUCKET_NAME, file) as obj, \
         gzip.GzipFile(fileobj=obj, mode="r") as f:
        gene_df = pd.read_csv(io.TextIOWrapper(f), sep="\t")

    # Pivot & trim barcodes
    gene_df = (
        gene_df
        .set_index("sample")
        .transpose()
        .reset_index()
        .rename(columns={"index": "bcr_patient_barcode"})
    )
    gene_df["bcr_patient_barcode"] = gene_df["bcr_patient_barcode"].str[:12]

    # Merge with clinical
    merged = pd.merge(clinical_df, gene_df, on="bcr_patient_barcode", how="inner")
    print(f"  → merged shape: {merged.shape}", flush=True)

    # Write header for first chunk, then append without header
    if i == 1:
        merged.to_csv(out_path, sep="\t", index=False)
    else:
        merged.to_csv(out_path, sep="\t", index=False, header=False, mode="a")

print("\nAll files processed. Final TSV written to:", out_path)

# Upload the single merged TSV to MinIO
print("Uploading merged TSV back to MinIO…", flush=True)
client.fput_object(
    bucket_name=BUCKET_NAME,
    object_name=OUTPUT_NAME,
    file_path=out_path,
    content_type="text/tab-separated-values"
)
print("Upload complete:", OUTPUT_NAME)
