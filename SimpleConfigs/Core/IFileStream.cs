namespace SimpleConfigs.Core
{
    public interface IFileStream : IDisposable
    {
        public Task WriteAsync(byte[] bytes, int offset, int count);
    }
}