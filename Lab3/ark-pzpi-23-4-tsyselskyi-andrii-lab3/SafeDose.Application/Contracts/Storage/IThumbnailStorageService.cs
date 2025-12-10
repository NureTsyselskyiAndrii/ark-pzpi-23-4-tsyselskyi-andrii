namespace SafeDose.Application.Contracts.Storage
{
    public interface IMedicineImageStorageService
    {
        Task<string> UploadMedicineImageAsync(Stream stream, string fileName, long medicineId, string? originalBlobName = null);
        string GetMedicineImageUrl(string blobName);
        Task RemoveMedicineImageAsync(string blobName);
    }
}
