namespace SimpleConfigs.Core
{
    public class LocalFileStream : IFileStream
    {
        private FileStream _stream;

        public LocalFileStream(FileStream stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public Task WriteAsync(byte[] bytes, int offset, int count)
        {
           return _stream.WriteAsync(bytes, offset, count);
        }
    }
}