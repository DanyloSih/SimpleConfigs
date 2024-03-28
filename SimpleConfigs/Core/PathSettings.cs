using SimpleConfigs.Utilities;

namespace SimpleConfigs.Core
{
    public class PathSettings : ICloneable
    {
        private string? _relativeDirectoryPath;
        private string? _fileName;

        public string RelativeFilePath
        {
            get
            {
                var directory = _relativeDirectoryPath == null ? string.Empty : _relativeDirectoryPath;
                var fileName = _fileName == null ? string.Empty : _fileName;
                return Path.Combine(directory, fileName);
            }
        }
        public string? RelativeDirectoryPath { get => _relativeDirectoryPath; }
        public string? FileName { get => _fileName; }

        public PathSettings()
        {
        }

        public PathSettings(string? relativeDirectoryPath, string? fileName)
        {
            SetRelativeDirectoryPath(relativeDirectoryPath);
            SetFileName(fileName);
        }

        /// <summary>
        /// File path relative to current assembly: "AssemblyPath" + "<paramref name="relativeFilePath"/>" <br/>
        /// Example: Proxies/Google/available.json
        /// </summary>
        public void SetRelativeFilePath(string relativeFilePath)
        {
            if (string.IsNullOrEmpty(relativeFilePath))
            {
                _relativeDirectoryPath = null;
                _fileName = null;
                return;
            }

            PathUtilities.CheckFilePathCorrectness(relativeFilePath);

            string? directory = Path.GetDirectoryName(relativeFilePath);
            _relativeDirectoryPath = string.IsNullOrEmpty(directory) ? null : directory;
            _fileName = Path.GetFileName(relativeFilePath);
        }

        public void SetFileName(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                _fileName = null;
                return;
            }

            PathUtilities.CheckFilePathCorrectness(fileName);

            var directoryPath = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException(
                    $"File name should not contain directory path: \"{directoryPath}\"!");
            }

            _fileName = fileName;
        }

        public void SetRelativeDirectoryPath(string? relativeDirectoryPath)
        {
            if (string.IsNullOrEmpty(relativeDirectoryPath))
            {
                _relativeDirectoryPath = null;
                return;
            }

            PathUtilities.CheckDirectoryPathCorrectness(relativeDirectoryPath);

            _relativeDirectoryPath = relativeDirectoryPath;
        }

        public object Clone()
        {
            var clone = new PathSettings();
            clone.SetRelativeFilePath(RelativeFilePath);
            return clone;
        }
    }
}