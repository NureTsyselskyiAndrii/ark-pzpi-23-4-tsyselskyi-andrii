using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeDose.Application.Contracts.Email;
using SafeDose.Application.Contracts.FileProcessing;
using SafeDose.Application.Contracts.Logging;
using SafeDose.Application.Contracts.Storage;
using SafeDose.Application.Models.Email;
using SafeDose.Application.Models.Storage;
using SafeDose.Infrastructure.EmailService;
using SafeDose.Infrastructure.FileProcessing;
using SafeDose.Infrastructure.Logging;
using SafeDose.Infrastructure.Storage;

namespace SafeDose.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddScoped<IImageProcessor, ImageProcessor>();

            services.Configure<BlobStorageContainerOptions>(configuration.GetSection("AzureStorage:Containers"));
            services.Configure<DefaultFiles>(configuration.GetSection("AzureStorage:DefaultFiles"));

            var storageConnectionString = configuration["AzureStorage:ConnectionString"];
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(storageConnectionString);
            });

            services.AddSingleton<IBlobContainerResolver, BlobContainerResolver>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();
            services.AddScoped<IProfileImageStorageService, ProfileImageStorageService>();
            services.AddScoped<IMedicineImageStorageService, MedicineImageStorageService>();

            return services;
        }
    }
}
