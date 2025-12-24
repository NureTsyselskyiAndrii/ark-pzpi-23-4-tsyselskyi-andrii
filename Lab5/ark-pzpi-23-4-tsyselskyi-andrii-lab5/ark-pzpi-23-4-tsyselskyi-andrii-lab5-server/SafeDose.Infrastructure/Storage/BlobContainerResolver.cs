using Microsoft.Extensions.Options;
using SafeDose.Application.Contracts.Storage;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Helpers;
using SafeDose.Application.Models.Storage;

namespace SafeDose.Infrastructure.Storage
{
    public class BlobContainerResolver : IBlobContainerResolver
    {
        private readonly BlobStorageContainerOptions _options;

        public BlobContainerResolver(IOptions<BlobStorageContainerOptions> options)
        {
            _options = options.Value;
        }

        public string GetContainerName(BlobContainerType type)
        {
            return type switch
            {
                BlobContainerType.MedicineImages => _options.MedicineImages,
                BlobContainerType.ProfileImages => _options.ProfileImages,
                _ => throw new InternalServerException()
            };
        }
    }
}
