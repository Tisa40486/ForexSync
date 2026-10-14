using ForexSync.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForexSync.Data
{
    public class RegisterDbExtension
    {
        public static void RegisterDb(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ForexDb");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'ForexDb' is not configured.");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<XmlParserService>();
            services.AddScoped<SynchronizationService>();
        }

        public static void HttpClientRegister(IServiceCollection services)
        {
            services.AddHttpClient<XmlDownloadService>();
        }
    }
}