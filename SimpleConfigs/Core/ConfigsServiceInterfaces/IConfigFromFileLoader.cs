using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigFromFileLoader : IConfigsContainer
    {
        public int CheckDataCorrectnessTimeoutInMilliseconds { get; set; }
        public int DeserializationTimeoutInMilliseconds { get; set; }

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
        public static Task LoadAllConfigsFromFilesAsync(
            this IConfigFromFileLoader member, bool checkDataCorrectness = true)
        {
            int operationTime = member.CheckDataCorrectnessTimeoutInMilliseconds
                + member.DeserializationTimeoutInMilliseconds;

            int awaitTime = operationTime * member.RegisteredConfigs.Count;

            return LoadAllConfigsFromFilesBaseAsync(member, checkDataCorrectness)
                .WaitAsync(awaitTime);
        }

        private static async Task LoadAllConfigsFromFilesBaseAsync(
            this IConfigFromFileLoader member, bool checkDataCorrectness = true)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.LoadConfigFromFileAsync(item.Key, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFromFileLoader.LoadConfigFromFileAsync(string, bool)"/>
        /// </summary>
        public static Task LoadConfigFromFileAsync(
            this IConfigFromFileLoader member, Type configType, bool checkDataCorrectness = true)
        {
            return member.LoadConfigFromFileAsync(configType.FullName!, checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigFromFileLoader.LoadConfigFromFileAsync(string, bool)"/>
        /// </summary>
        public static Task LoadConfigFromFileAsync<T>(
            this IConfigFromFileLoader member, bool checkDataCorrectness = true)
        {
            return member.LoadConfigFromFileAsync(typeof(T).FullName!, checkDataCorrectness);
        }
    }
}