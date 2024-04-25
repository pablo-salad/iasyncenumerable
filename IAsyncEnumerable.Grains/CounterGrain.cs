using IAsyncEnumerable.Grains.Abstractions;
using System.Threading.Channels;

namespace IAsyncEnumerable.Grains
{
    public class CounterGrain : Grain, ICounter
    {
        private readonly Channel<int> _jobChannel = Channel.CreateBounded<int>(200);

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var t= Task.Run(async () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    await _jobChannel.Writer.WriteAsync(i, cancellationToken);
                    await Task.Delay(250, cancellationToken);
                }
            });
        }

        public async IAsyncEnumerable<int> GetCount()
        {
            Console.WriteLine("GetCount started");
            try
            {
                await foreach (var i in _jobChannel.Reader.ReadAllAsync())
                {
                    if (i == 0)
                    {
                        yield break;
                    }
                    yield return i;
                }
            }
            finally
            {
                Console.WriteLine("GetCount cancelled");
            }
        }
    }
}
