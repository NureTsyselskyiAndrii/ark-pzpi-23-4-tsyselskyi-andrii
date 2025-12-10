using SafeDose.Application.Contracts.FileProcessing;
using SafeDose.Application.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace SafeDose.Infrastructure.FileProcessing
{
    public class ImageProcessor : IImageProcessor
    {
        public async Task<MemoryStream> ProcessSmallImageAsync(Stream stream, int size = 512)
        {
            try
            {
                using var image = await Image.LoadAsync(stream);

                int cropSize = Math.Min(image.Width, image.Height);
                var cropRect = new Rectangle(
                    (image.Width - cropSize) / 2,
                    (image.Height - cropSize) / 2,
                    cropSize,
                    cropSize
                );

                image.Mutate(x => x
                    .Crop(cropRect)
                    .Resize(size, size));

                var outputStream = new MemoryStream();
                await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = 90 });
                outputStream.Position = 0;

                return outputStream;
            }
            catch (UnknownImageFormatException)
            {
                throw new BadRequestException("The uploaded file is not a valid image.");
            }
        }
    }
}
