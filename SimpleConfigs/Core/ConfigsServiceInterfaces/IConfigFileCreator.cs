using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigFileCreator : IConfigsContainer
    {
        public int SerializationTimeoutInMilliseconds { get; set; }
        public int ConfigCreationTimeoutInMilliseconds { get; set; }

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
        public static Task CreateAllConfigFilesAsync(
            this IConfigFileCreator member)
        {
            int operationTime = member.SerializationTimeoutInMilliseconds
                + member.ConfigCreationTimeoutInMilliseconds;

            int awaitTime = operationTime * member.RegisteredConfigs.Count;

            return CreateAllConfigFilesBaseAsync(member).WaitAsync(awaitTime);
        }

        private static async Task CreateAllConfigFilesBaseAsync(
            IConfigFileCreator member)
        {
            foreach (var configs in member.RegisteredConfigs)
            {
                await member.CreateConfigFileAsync(configs.Key);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileCreator.CreateConfigFileAsync(string)"/>
        /// </summary>
        public static Task CreateConfigFileAsync(
            this IConfigFileCreator member, Type configType)
        {
            return member.CreateConfigFileAsync(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFileCreator.CreateConfigFileAsync(string)"/>
        /// </summary>
        public static Task CreateConfigFileAsync<T>(
            this IConfigFileCreator member)
        {
            return member.CreateConfigFileAsync(typeof(T).FullName!);
        }
    }
}