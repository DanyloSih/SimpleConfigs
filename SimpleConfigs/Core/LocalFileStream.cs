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

        public async Task WriteAsync(byte[] bytes, int offset, int count)
        {
            _stream.SetLength(count);
            await _stream.WriteAsync(bytes, offset, count);
        }
    }
}