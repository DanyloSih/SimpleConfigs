namespace SimpleConfigs.Attributes
{
    /// <summary>
    /// Specify config file name. <br/>
    /// If not specified, config file will be named like this: "TypeName.json"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigNameAttribute : Attribute
    {
        /// <summary>
        /// Name of config file with extension.
        /// </summary>
        public string ConfigName { get; private set; }

        public ConfigNameAttribute(string configName)
        {
            ConfigName = configName;
        }
    }
}