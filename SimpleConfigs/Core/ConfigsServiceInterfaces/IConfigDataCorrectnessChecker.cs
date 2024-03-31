using SimpleConfigs.Extensions;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigDataCorrectnessChecker : IConfigsContainer
    {
        public int CheckDataCorrectnessTimeoutInMilliseconds { get; set; }

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
        public static Task CheckAllConfigsDataCorrectnessAsync(
            this IConfigDataCorrectnessChecker member)
        {
            int awaitTime = member.RegisteredConfigs.Count 
                * member.CheckDataCorrectnessTimeoutInMilliseconds;

            return CheckAllConfigsDataCorrectnessBaseAsync(member).WaitAsync(awaitTime);
        }

        private static async Task CheckAllConfigsDataCorrectnessBaseAsync(IConfigDataCorrectnessChecker member)
        {
            foreach (var item in member.RegisteredConfigs)
            {
                await member.CheckDataCorrectnessAsync(item.Key);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IConfigDataCorrectnessChecker.CheckDataCorrectnessAsync(string)"/>
        /// </summary>
        public static Task CheckDataCorrectnessAsync(
            this IConfigDataCorrectnessChecker member, Type configType)
        {
            return member.CheckDataCorrectnessAsync(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigDataCorrectnessChecker.CheckDataCorrectnessAsync(string)"/>
        /// </summary>
        public static Task CheckDataCorrectnessAsync<T>(
            this IConfigDataCorrectnessChecker member)
        {
            return member.CheckDataCorrectnessAsync(typeof(T).FullName!);
        }
    }
}