namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigToFileSaver : IConfigsContainer
    {
        /// <summary>
        /// Creates config file with serialized data if it do not already exist, 
        /// or overwriting already existing config file.
        /// </summary>
        Task SaveConfigToFileAsync(string configTypeName, bool checkDataCorrectness = true);
    }

    public static class IConfigToFileSaverExtensions
    {
        /// <summary>
        /// <inheritdoc cref="IConfigToFileSaver.SaveConfigToFileAsync(string, bool)"/> <br/> 
        /// For each config!
        /// </summary>
        public static async Task SaveAllConfigsToFilesAsync(
            this IConfigsServicesHubMember member, bool checkDataCorrectness = true)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.SaveConfigToFileAsync(item.Key, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigToFileSaver.SaveConfigToFileAsync(string, bool)"/>
        /// </summary>
        public static async Task SaveConfigToFileAsync(
            this IConfigsServicesHubMember member, Type configType, bool checkDataCorrectness = true)
        {
            await member.SaveConfigToFileAsync(configType.FullName!, checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigToFileSaver.SaveConfigToFileAsync(string, bool)"/>
        /// </summary>
        public static async Task SaveConfigToFileAsync<T>(
            this IConfigsServicesHubMember member, bool checkDataCorrectness = true)
        {
            await member.SaveConfigToFileAsync(typeof(T).FullName!, checkDataCorrectness);
        }
    }
}