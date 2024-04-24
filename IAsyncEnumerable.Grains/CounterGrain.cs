using IAsyncEnumerable.Grains.Abstractions;
using System.Threading.Channels;

namespace IAsyncEnumerable.Grains
{
    public class CounterGrain : Grain, ICounter
    {
        private readonly Channel<int> _jobChannel = Channel.CreateBounded<int>(200);

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
        }

        public async IAsyncEnumerable<int> GetCount()
        {
            Console.WriteLine("GetCount started");
            try
            {
                await foreach (var i in _jobChannel.Reader.ReadAllAsync())
                {
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
