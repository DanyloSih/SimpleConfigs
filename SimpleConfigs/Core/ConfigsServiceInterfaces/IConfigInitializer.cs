namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigInitializer : IConfigsContainer
    {

    }

    public static class IConfigInitializerExtensions
    {
        /// <summary>
        /// <inheritdoc cref="InitializeConfigAsync(IConfigsServicesHubMember, string, bool)"/> <br/>
        /// For each config!
        /// </summary>
        public static async Task InitializeAllConfigsAsync(
            this IConfigsServicesHubMember member, bool checkDataCorrectness = true)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await InitializeConfigAsync(member, item.Key, checkDataCorrectness);
            }
        }

        /// <summary>
        /// 1. <inheritdoc cref="IConfigFileCreator.CreateConfigFileAsync(string)"/> <br/>
        /// 2. <inheritdoc cref="IConfigFromFileLoader.LoadConfigFromFileAsync(string, bool)"/>
        /// </summary>
        public static async Task InitializeConfigAsync(
            this IConfigsServicesHubMember member, string configTypeName, bool checkDataCorrectness = true)
        {
            await member.CreateConfigFileAsync(configTypeName);
            await member.LoadConfigFromFileAsync(configTypeName, checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="InitializeConfigAsync(IConfigsServicesHubMember, string)"/>
        /// </summary>
        public static async Task InitializeConfigAsync(
           this IConfigsServicesHubMember member, Type configType, bool checkDataCorrectness = true)
        {
            await member.CreateConfigFileAsync(configType.FullName!);
            await member.LoadConfigFromFileAsync(configType.FullName!, checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="InitializeConfigAsync(IConfigsServicesHubMember, string)"/>
        /// </summary>
        public static async Task InitializeConfigAsync<T>(
           this IConfigsServicesHubMember member, bool checkDataCorrectness = true)
        {
            await member.CreateConfigFileAsync(typeof(T).FullName!);
            await member.LoadConfigFromFileAsync(typeof(T).FullName!, checkDataCorrectness);
        }
    }
}