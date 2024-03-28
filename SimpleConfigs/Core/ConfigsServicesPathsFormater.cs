using SimpleConfigs.Utilities;

namespace SimpleConfigs.Core
{
    public struct ConfigsServicesPathsFormater
    {
        private static string s_idPlaceholder = "{id}";
        private static string s_extensionPlaceholder = "{ex}";

        public string CommonRelativeDirectory { get; private set; }
        public string SubdirectoryNameFormat { get; private set; }
        public string ConfigFileNameFormat { get; private set; }

        /// <param name="commonRelativeDirectory">Directory path relative to assembly path in which all subdirectories will be stored.</param>
        /// <param name="subdirectoryNameFormat">
        /// {id} - (Optional) ConfigService id in <see cref="ConfigsServicesHub"/> <br/>
        /// Example for id 5: "Subdirectory_{id}" -> "Subdirectory_5"</param>
        /// <param name="configFileNameFormat">
        /// {n} - (Optional) file name without extension <br/>
        /// {ex} - (Optional) file extension <br/>
        /// {id} - (Necessarily) ConfigService id in <see cref="ConfigsServicesHub"/> <br/>
        /// Example for id 5: "{n} copy {id}{ex}" -> "filename copy 5.extension"
        /// </param>
        public ConfigsServicesPathsFormater(
            string? commonRelativeDirectory,
            string subdirectoryNameFormat,
            string configFileNameFormat)
        {
            if (!string.IsNullOrEmpty(commonRelativeDirectory))
            {
                PathUtilities.CheckDirectoryPathCorrectness(commonRelativeDirectory);
                CommonRelativeDirectory = commonRelativeDirectory;
            }
            else
            {
                CommonRelativeDirectory = string.Empty;
            }

            PathUtilities.CheckDirectoryPathCorrectness(subdirectoryNameFormat);
            
            SubdirectoryNameFormat = subdirectoryNameFormat;

            if (!configFileNameFormat.Contains("{id}"))
            {
                throw new ArgumentException(
                    $"{nameof(configFileNameFormat)} must contain placeholder: " +
                    $" \"{s_idPlaceholder}\"");
            }

            ConfigFileNameFormat = configFileNameFormat;
            PathUtilities.CheckFilePathCorrectness(GetFormatedFileName("name.test", 10));
        }

        public string GetFormatedFileName(string fileName, int id)
        {
            PathUtilities.CheckFilePathCorrectness(fileName);

            string extension = Path.GetExtension(fileName)!;
            string name = Path.GetFileName(fileName).Replace(extension, "")!;

            return ConfigFileNameFormat.Replace("{n}", name)
                .Replace("{ex}", extension).Replace("{id}", $"{id}");
        }

        public string GetFormatedSubdirectory(int id)
        {
            return SubdirectoryNameFormat.Replace("{id}", $"{id}");
        }
    }
}