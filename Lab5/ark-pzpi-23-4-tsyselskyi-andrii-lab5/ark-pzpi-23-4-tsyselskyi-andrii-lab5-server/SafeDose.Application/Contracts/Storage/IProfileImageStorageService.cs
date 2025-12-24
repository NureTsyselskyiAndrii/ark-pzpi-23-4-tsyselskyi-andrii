namespace SafeDose.Application.Contracts.Storage
{
    public interface IProfileImageStorageService
    {
        Task<string> UploadProfileImageAsync(Stream stream, string fileName, long userId, string? originalBlobName = null);
        string GetProfileImageUrl(string blobName);
        Task RemoveProfileImageAsync(string blobName);
    }
}
