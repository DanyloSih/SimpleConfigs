using System.Reflection;

namespace SimpleConfigs.Core
{
    public class LocalFileSystem : IFileSystem
    {
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

        public async Task DeleteAsync(string filePath)
        {
            await Task.Run(() => File.Delete(filePath));
        }

        public bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public IFileStream OpenWrite(string filePath)
        {
            return new LocalFileStream(File.OpenWrite(filePath));
        }

        public async Task<byte[]> ReadAllBytesAsync(string filePath)
        {
            return await File.ReadAllBytesAsync(filePath);
        }
    }
}