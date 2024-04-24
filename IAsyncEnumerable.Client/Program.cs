using IAsyncEnumerable.Grains.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = await CreateClientAsync();
var client = host.Services.GetRequiredService<IClusterClient>();

var source = new CancellationTokenSource(3000);
var gcts = new GrainCancellationTokenSource();
StartTicker(source.Token, gcts);
await DoClientWorkAsync(client, source.Token);

Console.WriteLine("done");

Console.ReadKey();

return 0;

static async Task<IHost> CreateClientAsync()
{
    var builder = new HostBuilder()
        .UseOrleansClient(clientBuilder =>
        {
            clientBuilder.UseLocalhostClustering();
        }).ConfigureLogging(logging => logging.AddConsole())
        .UseConsoleLifetime();

    var host = builder.Build();
    await host.StartAsync();

    return host;
}

async Task DoClientWorkAsync(IClusterClient client, CancellationToken cancellationToken)
{
    var counter = client.GetGrain<ICounter>(string.Empty);
    var counterStream = counter.GetCount(gcts.Token);
    await foreach (var c in counterStream.WithCancellation(cancellationToken))
    {
        Console.WriteLine($"counter: {c}");
    }
    Console.WriteLine("stream completed");
}

static async Task StartTicker(CancellationToken cancellationToken, GrainCancellationTokenSource gcts)
{
    while (true)
    {
        try
        {
            Console.WriteLine("tick");
            await Task.Delay(250, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("ticker stopped");
            await gcts.Cancel();
            break;
        }
    }
}