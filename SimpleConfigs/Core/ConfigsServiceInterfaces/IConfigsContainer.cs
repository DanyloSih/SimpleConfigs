namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigsContainer
    {
        /// <summary>
        /// (ConfigType.FullName, instance) <br/>
        /// Returns all registered configs.
        /// </summary>
        IReadOnlyDictionary<string, object> RegisteredConfigs { get; }

        /// <summary>
        /// Return config instance via provided type.
        /// </summary>
        object GetConfig(string configTypeName);

        /// <summary>
        /// Return true if provided config type contained in <see cref="RegisteredConfigs"/>
        /// </summary>
        bool IsConfigExist(string configTypeName);
    }

    public static class IConfigsContainerExtensions
    {
        /// <summary>
        /// <inheritdoc cref="IConfigsContainer.IsConfigExist(string)"/>
        /// </summary>
        public static object GetConfig(this IConfigsContainer member, Type configType)
        {
            return member.GetConfig(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigsContainer.IsConfigExist(string)"/>
        /// </summary>
        public static T GetConfig<T>(this IConfigsContainer member)
        {
            return (T)member.GetConfig(typeof(T).FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigsContainer.IsConfigExist(string)"/>
        /// </summary>
        public static bool IsConfigExist(this IConfigsContainer member, Type configType)
        {
            return member.IsConfigExist(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigsContainer.IsConfigExist(string)"/>
        /// </summary>
        public static bool IsConfigExist<T>(this IConfigsContainer member)
        {
            return member.IsConfigExist(typeof(T).FullName!);
        }
    }
}