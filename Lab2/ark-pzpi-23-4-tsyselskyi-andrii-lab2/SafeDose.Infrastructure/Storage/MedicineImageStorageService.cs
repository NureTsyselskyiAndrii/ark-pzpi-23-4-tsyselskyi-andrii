using Microsoft.Extensions.Options;
using SafeDose.Application.Contracts.Storage;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Helpers;
using SafeDose.Application.Models.Storage;

namespace SafeDose.Infrastructure.Storage
{
    public class MedicineImageStorageService : IMedicineImageStorageService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly DefaultFiles _defaultFiles;

        public MedicineImageStorageService(IBlobStorageService blobStorageService, IOptions<DefaultFiles> defaultFiles)
        {
            _blobStorageService = blobStorageService;
            _defaultFiles = defaultFiles.Value;
        }

        public string GetMedicineImageUrl(string blobName)
        {
            return _blobStorageService.GetBlobUrl(BlobContainerType.MedicineImages, blobName);
        }

        public async Task RemoveMedicineImageAsync(string blobName)
        {
            if (blobName == _defaultFiles.DefaultMedicineImage)
            {
                throw new InternalServerException();
            }
            await _blobStorageService.RemoveBlobAsync(BlobContainerType.MedicineImages, blobName);
        }

        public async Task<string> UploadMedicineImageAsync(Stream stream, string fileName, long medicineId, string? originalBlobName = null)
        {
            if (originalBlobName == _defaultFiles.DefaultMedicineImage)
            {
                originalBlobName = null;
            }
            return await _blobStorageService.UploadBlob(BlobContainerType.MedicineImages, stream, fileName, medicineId.ToString(), originalBlobName);
        }
    }
}
