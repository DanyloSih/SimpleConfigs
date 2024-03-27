using System.Reflection;
using SimpleConfigs.Attributes;
using SimpleConfigs.Core;

namespace SimpleConfigs.Utilities
{
    public static class ConfigInfoUtilities
    {
        public static string GetConfigFileName(Type configObjectType, PathSettings? pathOverriteSettings = null)
        {
            if (pathOverriteSettings != null && !string.IsNullOrEmpty(pathOverriteSettings.FileName))
            {
                return pathOverriteSettings.FileName;
            }

            ConfigNameAttribute? configNameAttribute = configObjectType.GetCustomAttribute<ConfigNameAttribute>();

            if (configNameAttribute != null)
            {
                return configNameAttribute.ConfigName;
            }

            return $"{configObjectType.Name}.cfg";
        }

        public static string GetRelativePathForConfigFile(Type configObjectType, PathSettings? pathOverriteSettings = null)
        {
            string configFileName = GetConfigFileName(configObjectType, pathOverriteSettings);

            if (pathOverriteSettings != null && !string.IsNullOrEmpty(pathOverriteSettings.RelativeDirectoryPath))
            {
                return Path.Combine(pathOverriteSettings.RelativeDirectoryPath, configFileName);
            }

            RelativePathAttribute? relativePathAttribute = configObjectType.GetCustomAttribute<RelativePathAttribute>();
            if (relativePathAttribute != null)
            {
                PathSettings attributePathSettigns = new PathSettings();
                string? relativePath = relativePathAttribute.RelativeDirectoryPath;
                attributePathSettigns.SetRelativeDirectoryPath(relativePath);
                relativePath = attributePathSettigns.RelativeDirectoryPath;
                return Path.Combine(
                    string.IsNullOrEmpty(relativePath) ? string.Empty : relativePath, 
                    configFileName);
            }

            return configFileName;
        }
    }
}