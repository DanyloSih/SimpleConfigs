namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigFileDeleter : IConfigsContainer
    {
        /// <summary>
        /// Deletes a config file if it exists.
        /// </summary>
        Task DeleteConfigFileAsync(string configTypeName);
    }

    public static class IConfigFileDeleterExtensions
    {

        /// <summary>
        /// <inheritdoc cref="IConfigFileDeleter.DeleteConfigFileAsync(string)"/><br/> 
        /// For each config!
        /// </summary>
        public static async Task DeleteAllConfigFilesAsync(
            this IConfigsServicesHubMember member)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.DeleteConfigFileAsync(item.Key);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileDeleter.DeleteConfigFileAsync(string)"/>
        /// </summary>
        public static async Task DeleteConfigFileAsync(
            this IConfigsServicesHubMember member, Type configType)
        {
            await member.DeleteConfigFileAsync(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileDeleter.DeleteConfigFileAsync(string)"/>
        /// </summary>
        public static async Task DeleteConfigFileAsync<T>(
           this IConfigsServicesHubMember member)
        {
            await member.DeleteConfigFileAsync(typeof(T).FullName!);
        }
    }
}