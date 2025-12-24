using SafeDose.Application.Helpers;

namespace SafeDose.Application.Contracts.Storage
{
    public interface IBlobStorageService
    {
        string GetBlobUrl(BlobContainerType containerType, string blobName);
        Task RemoveBlobAsync(BlobContainerType containerType, string blobName);
        Task<string> UploadBlob(BlobContainerType containerType, Stream stream, string fileName, string folder = "", string? originalBlobName = null);
    }
}
