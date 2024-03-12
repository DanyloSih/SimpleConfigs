using System.Reflection;

namespace SimpleConfigs.Utilities
{
    public static class PathUtilities
    {
        public static void CreateDirectoriesAlongPath(string path)
        {
            string directoryPath = Path.GetDirectoryName(path)!;

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static string GetApplicationDirectory()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyLocation = assembly.Location;
            return Path.GetDirectoryName(assemblyLocation)!;
        }
    }
}