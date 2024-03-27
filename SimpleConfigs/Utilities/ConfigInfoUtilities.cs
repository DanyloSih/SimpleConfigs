using System.Reflection;
using SimpleConfigs.Attributes;
using SimpleConfigs.Core;

namespace SimpleConfigs.Utilities
{
    public static class ConfigInfoUtilities
    {
        public static string GetConfigFileName(Type configObjectType, PathSettings? pathOverriteSettings = null)
        {
            if (pathOverriteSettings != null)
            {
                var name = string.IsNullOrEmpty(pathOverriteSettings.FileName) 
                    ? GetDefaultConfigName(configObjectType) : pathOverriteSettings.FileName;

                return name;
            }

            ConfigNameAttribute? configNameAttribute = configObjectType.GetCustomAttribute<ConfigNameAttribute>();

            if (configNameAttribute != null)
            {
                return configNameAttribute.ConfigName;
            }

            return GetDefaultConfigName(configObjectType);
        }

        private static string GetDefaultConfigName(Type configObjectType)
        {
            return $"{configObjectType.Name}.cfg";
        }

        public static string GetRelativePathForConfigFile(Type configObjectType, PathSettings? pathOverriteSettings = null)
        {
            string configFileName = GetConfigFileName(configObjectType, pathOverriteSettings);

            if (pathOverriteSettings != null)
            {
                var path = string.IsNullOrEmpty(pathOverriteSettings.RelativeDirectoryPath) 
                    ? string.Empty : pathOverriteSettings.RelativeDirectoryPath;

                return Path.Combine(path, configFileName);
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