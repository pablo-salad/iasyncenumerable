using IAsyncEnumerable.Grains;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Serialization;

try
{
    var host = await StartSiloAsync();
    Console.WriteLine("\n\n Press Enter to terminate...\n\n");
    Console.ReadLine();

    await host.StopAsync();
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 1;
}

static async Task<IHost> StartSiloAsync()
{
    var builder = new HostBuilder()
        .UseOrleans(c =>
        {
            c.UseLocalhostClustering().ConfigureLogging(logging => logging.AddConsole());
        });

    builder.ConfigureServices(services =>
    {
        services.AddSerializer(options => options.AddAssembly(typeof(CounterGrain).Assembly));
    });

    var host = builder.Build();
    await host.StartAsync();

    return host;
}