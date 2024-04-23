namespace IAsyncEnumerable.Grains.Abstractions
{
    public interface ICounter : IGrainWithStringKey
    {
        IAsyncEnumerable<int> GetCount();
    }
}