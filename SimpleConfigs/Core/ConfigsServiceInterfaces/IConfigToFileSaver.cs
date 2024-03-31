using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigToFileSaver : IConfigsContainer
    {
        public int CheckDataCorrectnessTimeoutInMilliseconds { get; set; }
        public int SerializationTimeoutInMilliseconds { get; set; }
        public int FileWriteTimeoutInMilliseconds { get; set; }

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
        public static Task SaveAllConfigsToFilesAsync(
            this IConfigToFileSaver member, bool checkDataCorrectness = true)
        {
            int operationTime = member.CheckDataCorrectnessTimeoutInMilliseconds
                + member.SerializationTimeoutInMilliseconds 
                + member.FileWriteTimeoutInMilliseconds;

            int awaitTime = operationTime * member.RegisteredConfigs.Count;

            return SaveAllConfigsToFilesBaseAsync(member, checkDataCorrectness)
                .WaitAsync(awaitTime);
        }

        private static async Task SaveAllConfigsToFilesBaseAsync(IConfigToFileSaver member, bool checkDataCorrectness = true)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.SaveConfigToFileAsync(item.Key, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigToFileSaver.SaveConfigToFileAsync(string, bool)"/>
        /// </summary>
        public static Task SaveConfigToFileAsync(
            this IConfigToFileSaver member, Type configType, bool checkDataCorrectness = true)
        {
            return member.SaveConfigToFileAsync(configType.FullName!, checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigToFileSaver.SaveConfigToFileAsync(string, bool)"/>
        /// </summary>
        public static Task SaveConfigToFileAsync<T>(
            this IConfigToFileSaver member, bool checkDataCorrectness = true)
        {
            return member.SaveConfigToFileAsync(typeof(T).FullName!, checkDataCorrectness);
        }
    }
}