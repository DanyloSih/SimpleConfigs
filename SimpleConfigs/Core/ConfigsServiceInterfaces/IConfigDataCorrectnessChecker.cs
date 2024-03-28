namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigDataCorrectnessChecker : IConfigsContainer
    {
        /// <summary>
        /// Invoke <see cref="IDataCorrectnessChecker.CheckDataCorrectnessAsync"/> method <br/>
        /// if config implement <see cref="IDataCorrectnessChecker"/> interface.
        /// </summary>
        Task CheckDataCorrectnessAsync(string configTypeName);
    }

    public static class IConfigDataCorrectnessCheckerExtensions
    {
        /// <summary>
        /// <inheritdoc cref="IConfigDataCorrectnessChecker.CheckDataCorrectnessAsync(string)"/> <br/>
        /// For each config!
        /// </summary>
        public static async Task CheckAllConfigsDataCorrectnessAsync(
            this IConfigsServicesHubMember member)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.CheckDataCorrectnessAsync(item.Key);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigDataCorrectnessChecker"/>
        /// </summary>
        public static async Task CheckDataCorrectnessAsync(
            this IConfigsServicesHubMember member, Type configType)
        {
            await member.CheckDataCorrectnessAsync(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigDataCorrectnessChecker"/>
        /// </summary>
        public static async Task CheckDataCorrectnessAsync<T>(
            this IConfigsServicesHubMember member)
        {
            await member.CheckDataCorrectnessAsync(typeof(T).FullName!);
        }
    }
}