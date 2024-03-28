using System.IO;

namespace SimpleConfigs.Utilities
{
    public static class PathUtilities
    {     
        public static void CheckDirectoryPathCorrectness(string? directoryPath)
        {
            CheckCommonPathCorrectness(directoryPath);

            string? extension = Path.GetExtension(directoryPath);
            if (!string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"Directory path cannot contain extension: \"{extension}\"");
            }
        }

        public static void CheckFilePathCorrectness(string? filePath)
        {
            CheckCommonPathCorrectness(filePath);

            string? extension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"{nameof(filePath)} should contain file extension.");
            }

            string? name = Path.GetFileName(filePath);
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"{nameof(filePath)} should contain name.");
            }
        }

        private static void CheckCommonPathCorrectness(string? path)
        {
            if (path == null)
            {
                throw new ArgumentException("The path cannot be null or empty.");
            }

            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new ArgumentException("Path contains incorect chars!");
            }
        }
    }
}