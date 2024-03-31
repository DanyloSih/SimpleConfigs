using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigInitializer : IConfigsContainer, IConfigFileCreator, IConfigFromFileLoader
    {

    }

    public static class IConfigInitializerExtensions
    {
        /// <summary>
        /// <inheritdoc cref="InitializeConfigAsync(IConfigInitializer, string, bool)"/> <br/>
        /// For each config!
        /// </summary>
        public static Task InitializeAllConfigsAsync(
            this IConfigInitializer member, bool checkDataCorrectness = true)
        {
            int operationTime = GetOperationTime(member) * member.RegisteredConfigs.Count;
            return InitializeAllConfigsBaseAsync(member, checkDataCorrectness).WaitAsync(operationTime);
        }

        /// <summary>
        /// 1. <inheritdoc cref="IConfigFileCreator.CreateConfigFileAsync(string)"/> <br/>
        /// 2. <inheritdoc cref="IConfigFromFileLoader.LoadConfigFromFileAsync(string, bool)"/>
        /// </summary>
        public static Task InitializeConfigAsync(
            this IConfigInitializer member,
            string configTypeName,
            bool checkDataCorrectness = true)
        {
            return InitializeConfigBaseAsync(member, configTypeName, checkDataCorrectness)
                .WaitAsync(GetOperationTime(member));
        }

        /// <summary>
        /// <inheritdoc cref="InitializeConfigAsync(IConfigInitializer, string, bool)"/>
        /// </summary>
        public static Task InitializeConfigAsync(
           this IConfigInitializer member, Type configType, bool checkDataCorrectness = true)
        {
            return InitializeConfigAsync(member, configType.FullName!, checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="InitializeConfigAsync(IConfigInitializer, string, bool)"/>
        /// </summary>
        public static Task InitializeConfigAsync<T>(
           this IConfigInitializer member, bool checkDataCorrectness = true)
        {
            return InitializeConfigAsync(member, typeof(T).FullName!, checkDataCorrectness);
        }

        private static async Task InitializeAllConfigsBaseAsync(
            IConfigInitializer member, bool checkDataCorrectness = true)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await InitializeConfigAsync(member, item.Key, checkDataCorrectness);
            }
        }

        private static async Task InitializeConfigBaseAsync(
            IConfigInitializer member, string configTypeName, bool checkDataCorrectness = true)
        {
            await member.CreateConfigFileAsync(configTypeName);
            await member.LoadConfigFromFileAsync(configTypeName, checkDataCorrectness);
        }

        private static int GetOperationTime(IConfigInitializer member)
        {
            int configCreationTime = member.SerializationTimeoutInMilliseconds
                + member.ConfigCreationTimeoutInMilliseconds;

            int configLoadingTime = member.CheckDataCorrectnessTimeoutInMilliseconds
                + member.DeserializationTimeoutInMilliseconds;

            return configCreationTime + configLoadingTime;
        }
    }
}