using Orleans.Concurrency;

namespace IAsyncEnumerable.Grains.Abstractions
{
    public interface ICounter : IGrainWithStringKey
    {
        [AlwaysInterleave]
        IAsyncEnumerable<int> GetCount(GrainCancellationToken cancellationToken);
    }
}