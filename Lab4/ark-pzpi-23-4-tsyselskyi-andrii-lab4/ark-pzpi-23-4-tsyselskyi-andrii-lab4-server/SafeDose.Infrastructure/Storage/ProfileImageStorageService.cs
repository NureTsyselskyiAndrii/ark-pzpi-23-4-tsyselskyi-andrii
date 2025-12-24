using Microsoft.Extensions.Options;
using SafeDose.Application.Contracts.FileProcessing;
using SafeDose.Application.Contracts.Storage;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Helpers;
using SafeDose.Application.Models.Storage;

namespace SafeDose.Infrastructure.Storage
{
    public class ProfileImageStorageService : IProfileImageStorageService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly DefaultFiles _defaultFiles;
        private readonly IImageProcessor _imageProcessor;

        public ProfileImageStorageService(IBlobStorageService blobStorageService, IOptions<DefaultFiles> defaultFiles, IImageProcessor imageProcessor)
        {
            _blobStorageService = blobStorageService;
            _defaultFiles = defaultFiles.Value;
            _imageProcessor = imageProcessor;
        }

        public string GetProfileImageUrl(string blobName)
        {
            return _blobStorageService.GetBlobUrl(BlobContainerType.ProfileImages, blobName);
        }

        public async Task RemoveProfileImageAsync(string blobName)
        {
            if (blobName == _defaultFiles.DefaultProfileImage)
            {
                throw new InternalServerException();
            }
            await _blobStorageService.RemoveBlobAsync(BlobContainerType.ProfileImages, blobName);
        }

        public async Task<string> UploadProfileImageAsync(Stream stream, string fileName, long userId, string? originalBlobName = null)
        {
            try
            {
                var processedStream = await _imageProcessor.ProcessSmallImageAsync(stream);

                string newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}.jpg";

                if (originalBlobName == _defaultFiles.DefaultProfileImage)
                {
                    originalBlobName = null;
                }

                return await _blobStorageService.UploadBlob(
                    BlobContainerType.ProfileImages,
                    processedStream,
                    newFileName,
                    userId.ToString(),
                    originalBlobName
                );
            }
            catch
            {
                throw new InternalServerException();
            }
        }
    }
}
