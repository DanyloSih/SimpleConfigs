using System.Reflection;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigRegistrar : IConfigsContainer
    {
        /// <summary>
        /// Create instance of config type and create with it necessary data associations. <br/>
        /// After registration you be able to create, load and save file for this config type object. <br/> 
        /// </summary>
        void RegisterConfigType(string configTypeName, Assembly assembly, PathSettings? pathOverrideSettigns = null);

        /// <summary>
        /// After unregistering, all data associations for config type will be removed. <br/>
        /// You no longer be able to create, load or save file for this config type object. <br/>
        /// </summary>
        void UnregisterConfigType(string configTypeName);
    }

    public static class IConfigsRegistratorExtensions
    {
        /// <summary>
        /// <inheritdoc cref="IConfigRegistrar.RegisterConfigType(string, PathSettings?)"/>
        /// </summary>
        public static void RegisterConfigType(
            this IConfigRegistrar registrar, Type configType, PathSettings? pathOverrideSettigns = null)
        {
            registrar.RegisterConfigType(configType.FullName!, configType.Assembly, pathOverrideSettigns);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigRegistrar.RegisterConfigType(string, PathSettings?)"/>
        /// </summary>
        public static void RegisterConfigType<T>(
            this IConfigRegistrar registrar, PathSettings? pathOverrideSettigns = null)
        {
            var type = typeof(T);
            registrar.RegisterConfigType(type.FullName!, type.Assembly, pathOverrideSettigns);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigRegistrar.UnregisterConfigType(string)"/>
        /// </summary>
        public static void UnregisterConfigType(
            this IConfigRegistrar registrar, Type configType)
        {
            registrar.UnregisterConfigType(configType.FullName!);
        }

        /// <summary>
        /// <inheritdoc cref="IConfigRegistrar.UnregisterConfigType(string)"/>
        /// </summary>
        public static void UnregisterConfigType<T>(
            this IConfigRegistrar registrar)
        {
            registrar.UnregisterConfigType(typeof(T).FullName!);
        }
    }
}