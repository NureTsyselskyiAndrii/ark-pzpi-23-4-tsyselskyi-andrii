using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using SafeDose.Application.Contracts.Storage;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Helpers;
using System.Text.RegularExpressions;

namespace SafeDose.Infrastructure.Storage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IBlobContainerResolver _containerResolver;

        public BlobStorageService(BlobServiceClient blobServiceClient, IBlobContainerResolver containerResolver)
        {
            _blobServiceClient = blobServiceClient;
            _containerResolver = containerResolver;
        }

        public async Task<string> UploadBlob(BlobContainerType containerType, Stream stream, string fileName, string folder = "", string? originalBlobName = null)
        {
            var containerName = _containerResolver.GetContainerName(containerType);
            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            string slug = Slugify(fileNameWithoutExtension);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff");
            string guid = Guid.NewGuid().ToString("N");

            string blobName = $"{slug}_{timestamp}_{guid}{extension}";
            if (!string.IsNullOrWhiteSpace(folder))
            {
                blobName = $"{folder.TrimEnd('/')}/{blobName}";
            }

            if (string.IsNullOrWhiteSpace(originalBlobName) == false)
            {
                await RemoveBlobAsync(containerType, originalBlobName);
            }

            var blob = container.GetBlobClient(blobName);

            try
            {
                await blob.UploadAsync(stream);
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }


            return blobName;
        }

        public string GetBlobUrl(BlobContainerType containerType, string blobName)
        {
            var containerName = _containerResolver.GetContainerName(containerType);
            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            var blob = container.GetBlobClient(blobName);

            BlobSasBuilder blobSasBuilder = new()
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),
                Protocol = SasProtocol.Https,
                Resource = "b"
            };
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blob.GenerateSasUri(blobSasBuilder).ToString();
        }

        public async Task RemoveBlobAsync(BlobContainerType containerType, string blobName)
        {
            var containerName = _containerResolver.GetContainerName(containerType);
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(blobName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private static string Slugify(string text)
        {
            text = text.ToLowerInvariant().Trim();
            text = Regex.Replace(text, @"[^a-z0-9]+", "_");
            return text.Trim('_');
        }
    }
}
