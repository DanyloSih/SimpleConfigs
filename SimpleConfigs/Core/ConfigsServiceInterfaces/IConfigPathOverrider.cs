namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigPathOverrider : IConfigsContainer
    {
        bool IsPathOverrideSettingsNull(string configTypeName);
        void SetPathOverrideSettings(string configTypeName, PathSettings pathOverrideSettings);
        PathSettings? GetPathOverrideSettings(string configTypeName);
    }

    public static class IConfigPathOverriderExtensions
    {
        public static bool IsPathOverrideSettingsNull(
            this IConfigPathOverrider overrider, Type configType)
        {
            return overrider.IsPathOverrideSettingsNull(configType.FullName!);
        }

        public static bool IsPathOverrideSettingsNull<T>(
            this IConfigPathOverrider overrider)
        {
            return overrider.IsPathOverrideSettingsNull(typeof(T).FullName!);
        }

        public static void SetPathOverrideSettings(
            this IConfigPathOverrider overrider, Type configType, PathSettings pathOverrideSettings)
        {
            overrider.SetPathOverrideSettings(configType.FullName!, pathOverrideSettings);
        }

        public static void SetPathOverrideSettings<T>(
           this IConfigPathOverrider overrider, PathSettings pathOverrideSettings)
        {
            overrider.SetPathOverrideSettings(typeof(T).FullName!, pathOverrideSettings);
        }

        public static PathSettings? GetPathOverrideSettings(
            this IConfigPathOverrider overrider, Type configType)
        {
            return overrider.GetPathOverrideSettings(configType.FullName!);
        }

        public static PathSettings? GetPathOverrideSettings<T>(
            this IConfigPathOverrider overrider)
        {
            return overrider.GetPathOverrideSettings(typeof(T).FullName!);
        }
    }
}