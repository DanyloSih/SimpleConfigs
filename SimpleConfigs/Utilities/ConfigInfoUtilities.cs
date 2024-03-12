using System.IO;
using System.Reflection;
using SimpleConfigs.Attributes;

namespace SimpleConfigs.Utilities
{
    public static class ConfigInfoUtilities
    {
        public static string GetConfigFileName(object configObject)
        {
            Type configObjectType = configObject.GetType();

            ConfigNameAttribute configNameAttribute = configObjectType.GetCustomAttribute<ConfigNameAttribute>();

            if (configNameAttribute != null)
            {
                return configNameAttribute.ConfigName;
            }

            return $"{configObjectType.Name}.cfg";
        }

        public static string GetFullPathForConfigFile(object configObject)
        {
            Type configObjectType = configObject.GetType();
            string configFileName = GetConfigFileName(configObject);

            RelativePathAttribute relativePathAttribute = configObjectType.GetCustomAttribute<RelativePathAttribute>();

            if (relativePathAttribute != null)
            {
                string relativePath = relativePathAttribute.RelativeDirectoryPath;
                var relativeDirectoryName = Path.HasExtension(relativePath) ? Path.GetDirectoryName(relativePath) : relativePath;
                return Path.Combine(PathUtilities.GetApplicationDirectory(), relativeDirectoryName!, configFileName);
            }

            return Path.Combine(PathUtilities.GetApplicationDirectory(), configFileName);
        }
    }
}
