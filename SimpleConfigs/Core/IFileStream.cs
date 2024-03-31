namespace SimpleConfigs.Core
{
    public interface IFileStream : IDisposable
    {
        public int WriteTimeoutInMilliseconds { get; set; }

        public Task WriteAsync(byte[] bytes, int offset, int count);
    }
}