using SafeDose.Application.Helpers;

namespace SafeDose.Application.Contracts.Storage
{
    public interface IBlobContainerResolver
    {
        string GetContainerName(BlobContainerType type);
    }
}
