﻿namespace SimpleConfigs.Core
{
    public interface IFileSystem
    {
        public bool IsFileExist(string filePath);
        public IFileStream Create(string filePath);
        public IFileStream OpenWrite(string filePath);
        public Task DeleteAsync(string filePath);
        public Task<byte[]> ReadAllBytesAsync(string filePath);
        public Task CreateDirectoriesAlongPathAsync(string path);
        public string GetApplicationDirectory();
    }
}