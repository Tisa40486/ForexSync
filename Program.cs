using ForexSync.Data;
using ForexSync.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

RegisterDbExtension.HttpClientRegister(builder.Services);
RegisterDbExtension.RegisterDb(builder.Services, builder.Configuration);

using var host = builder.Build();

try
{
    using var scope = host.Services.CreateScope();

    var downloadService = scope.ServiceProvider.GetRequiredService<XmlDownloadService>();

    var parser = scope.ServiceProvider.GetRequiredService<XmlParserService>();

    var syncService = scope.ServiceProvider.GetRequiredService<SynchronizationService>();

    var url = builder.Configuration["Ecb:Url"];

    if (string.IsNullOrEmpty(url))
        throw new InvalidOperationException("URL is not configured.");

    Console.WriteLine("Download XML...");

    var xml = await downloadService.DownloadAsync(url);

    Console.WriteLine("Analyzing exchange rates...");

    var rates = parser.Parse(xml);

    Console.WriteLine($"{rates.Count} exchange rates found.");

    await syncService.SynchronizeAsync(rates);

    Console.WriteLine("Synchronization complete.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error during import : {ex}");

    Environment.ExitCode = 1;
}