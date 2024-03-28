namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigFileCreator : IConfigsContainer
    {
        /// <summary>
        /// Create configuration file and specified directories along config path if they do not already exist.
        /// </summary>
        Task CreateConfigFileAsync(string configTypeName);
    }

    public static class IConfigFileCreatorExtensions
    {
        /// <summary>
        /// <inheritdoc cref="IConfigFileCreator.CreateConfigFileAsync(string)"/><br/> 
        /// For each config!
        /// </summary>
        public static async Task CreateAllConfigFilesAsync(
            this IConfigsServicesHubMember member, bool inParallel = true)
        {
            foreach (var configs in member.RegisteredConfigs)
            {
                await member.CreateConfigFileAsync(configs.Key);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileCreator.CreateConfigFileAsync(string)"/>
        /// </summary>
        public static async Task CreateConfigFileAsync(
            this IConfigsServicesHubMember member, Type configType)
        {
            await member.CreateConfigFileAsync(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileCreator.CreateConfigFileAsync(string)"/>
        /// </summary>
        public static async Task CreateConfigFileAsync<T>(
            this IConfigsServicesHubMember member)
        {
            await member.CreateConfigFileAsync(typeof(T).FullName!);
        }
    }
}