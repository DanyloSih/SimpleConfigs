namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigFromFileLoader : IConfigsContainer
    {
        /// <summary>
        /// Load deserealized config data from file if file exist, if not - throw <see cref="InvalidOperationException"/>.
        /// </summary>
        Task LoadConfigFromFileAsync(string configTypeName, bool checkDataCorrectness = true);
    }

    public static class IConfigFromFileLoaderExtensions
    {
        /// <summary>
        /// <inheritdoc cref="IConfigFromFileLoader.LoadConfigFromFileAsync(string, bool)"/> <br/> 
        /// For each config!
        /// </summary>
        public static async Task LoadAllConfigsFromFilesAsync(
            this IConfigsServicesHubMember member, bool checkDataCorrectness = true)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.LoadConfigFromFileAsync(item.Key, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFromFileLoader.LoadConfigFromFileAsync(string, bool)"/>
        /// </summary>
        public static async Task LoadConfigFromFileAsync(
            this IConfigsServicesHubMember member, Type configType, bool checkDataCorrectness = true)
        {
            await member.LoadConfigFromFileAsync(configType.FullName!, checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFromFileLoader.LoadConfigFromFileAsync(string, bool)"/>
        /// </summary>
        public static async Task LoadConfigFromFileAsync<T>(
            this IConfigsServicesHubMember member, bool checkDataCorrectness = true)
        {
            await member.LoadConfigFromFileAsync(typeof(T).FullName!, checkDataCorrectness);
        }
    }
}