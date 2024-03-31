using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core
{
    public class LocalFileStream : IFileStream
    {
        private FileStream _stream;

        public int WriteTimeoutInMilliseconds { get; set; }

        public LocalFileStream(FileStream stream, int writeTimeout = 1000)
        {
            _stream = stream;
            WriteTimeoutInMilliseconds = writeTimeout;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public Task WriteAsync(byte[] bytes, int offset, int count)
        {
            _stream.SetLength(count);
            return _stream.WriteAsync(bytes, offset, count).WaitAsync(WriteTimeoutInMilliseconds);
        }
    }
}