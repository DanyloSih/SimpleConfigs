using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigFileDeleter : IConfigsContainer
    {
        public int ConfigDeletingTimeoutInMilliseconds { get; set; }

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
        public static Task DeleteAllConfigFilesAsync(
            this IConfigFileDeleter member)
        {
            int awaitTime = member.RegisteredConfigs.Count *
                member.ConfigDeletingTimeoutInMilliseconds;

            return DeleteAllConfigFilesBaseAsync(member).WaitAsync(awaitTime);
        }

        private static async Task DeleteAllConfigFilesBaseAsync(IConfigFileDeleter member)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.DeleteConfigFileAsync(item.Key);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileDeleter.DeleteConfigFileAsync(string)"/>
        /// </summary>
        public static Task DeleteConfigFileAsync(
            this IConfigFileDeleter member, Type configType)
        {
            return member.DeleteConfigFileAsync(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileDeleter.DeleteConfigFileAsync(string)"/>
        /// </summary>
        public static Task DeleteConfigFileAsync<T>(
           this IConfigFileDeleter member)
        {
            return member.DeleteConfigFileAsync(typeof(T).FullName!);
        }
    }
}