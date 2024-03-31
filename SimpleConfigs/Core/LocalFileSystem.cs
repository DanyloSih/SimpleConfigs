using System.Reflection;
using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core
{
    public class LocalFileSystem : IFileSystem
    {
        public int DeleteFileTimeoutInMilliseconds { get; set; }
        public int ReadAllBytesTimeoutInMilliseconds { get; set; }

        public LocalFileSystem(int deleteFileTimeout = 1000, int readAllBytesTimeout = 1000)
        {
            DeleteFileTimeoutInMilliseconds = deleteFileTimeout;
            ReadAllBytesTimeoutInMilliseconds = readAllBytesTimeout;
        }

        public Task CreateDirectoriesAlongPathAsync(string path)
        {
            string directoryPath = Path.GetDirectoryName(path)!;

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return Task.CompletedTask;
        }

        public string GetApplicationDirectory()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyLocation = assembly.Location;
            return Path.GetDirectoryName(assemblyLocation)!;
        }

        public IFileStream Create(string filePath)
        {
            return new LocalFileStream(File.Create(filePath));
        }

        public Task DeleteAsync(string filePath)
        {
            return Task.Run(() => File.Delete(filePath)).WaitAsync(DeleteFileTimeoutInMilliseconds);
        }

        public bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public IFileStream OpenWrite(string filePath)
        {
            return new LocalFileStream(File.OpenWrite(filePath));
        }

        public Task<byte[]> ReadAllBytesAsync(string filePath)
        {
            return File.ReadAllBytesAsync(filePath).WaitAsync(ReadAllBytesTimeoutInMilliseconds);
        }
    }
}