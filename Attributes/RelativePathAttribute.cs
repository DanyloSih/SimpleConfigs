namespace SimpleConfigs.Attributes
{
    /// <summary>
    /// Specify relative path for config file on device.
    /// If not specified, config file will be generated in assembly folder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RelativePathAttribute : Attribute
    {
        /// <summary>
        /// Path relative to current assembly. <br/>
        /// Like this: Assembly Directory path / Relative Directory Path
        /// </summary>
        public string RelativeDirectoryPath { get; private set; }

        public RelativePathAttribute(string relativePath)
        {
            RelativeDirectoryPath = relativePath;
        }
    }
}
