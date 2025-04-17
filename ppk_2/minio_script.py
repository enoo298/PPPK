import os
import requests
from minio import Minio

DATA_DIR = os.path.join(os.path.dirname(__file__), "data")
DOWNLOAD_CACHE_FILE = os.path.join(DATA_DIR, "tcga_download_links.txt")

MINIO_URL = "localhost:9000"
MINIO_ACCESS_KEY = "minioadmin"
MINIO_SECRET_KEY = "minioadmin"
BUCKET_NAME = "baket"

os.makedirs(DATA_DIR, exist_ok=True)

minio_client = Minio(
    MINIO_URL,
    access_key=MINIO_ACCESS_KEY,
    secret_key=MINIO_SECRET_KEY,
    secure=False
)

if not minio_client.bucket_exists(BUCKET_NAME):
    minio_client.make_bucket(BUCKET_NAME)
    print(f"Created bucket: {BUCKET_NAME}")

def read_download_links():
    if not os.path.exists(DOWNLOAD_CACHE_FILE):
        print("No download links cached! Ensure 'tcga_download_links.txt' exists in 'data/'.")
        return []

    with open(DOWNLOAD_CACHE_FILE, "r") as file:
        return [line.strip().split(": ")[1] for line in file.readlines() if line.strip()]

def download_and_store_files():
    download_links = read_download_links()

    count = 1
    count_good = 0
    count_bad = 0

    count_max = len(download_links)

    if not download_links:
        return

    for link in download_links:
        cohort_name = link.split("/")[-1].split(".")[1]
        local_filename = f"{cohort_name}.tsv.gz"
        local_filepath = os.path.join(DATA_DIR, local_filename)

        print(f"\u2913 Downloading ({count}/{count_max}): {cohort_name}")
        response = requests.get(link, stream=True)

        if response.status_code == 200:
            with open(local_filepath, "wb") as file:
                for chunk in response.iter_content(chunk_size=8192):
                    file.write(chunk)

            print(f"\u2912 Uploading {local_filename} to MinIO")
            minio_client.fput_object(BUCKET_NAME, local_filename, local_filepath)

            os.remove(local_filepath)
            print(f"{local_filename} uploaded.\n")

            count += 1
            count_good += 1
        else:
            count += 1
            count_bad += 1
            print(f"Skipping {cohort_name}, file not found (HTTP {response.status_code}).\n")

    print(f"Completed:\t{count_good}")
    print(f"Failed:\t\t{count_bad}")
    print(f"Total fetches:\t{count - 1}")

if __name__ == "__main__":
    print("Starting TCGA Data Download & Storage...\n")
    download_and_store_files()
    print("\nAll available files have been stored in MinIO.\n")