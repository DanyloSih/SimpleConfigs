using System.Collections.Concurrent;
using System.Reflection;
using SimpleConfigs.Core.ConfigsServiceInterfaces;
using SimpleConfigs.Extensions;
using SimpleConfigs.Utilities;

namespace SimpleConfigs.Core
{
    /// <summary>
    /// This object manage config objects life.
    /// </summary>
    public class ConfigsService : IConfigsService
    {
        private ConcurrentDictionary<string, object> _registeredConfigs = new();
        private ConcurrentDictionary<string, PathSettings?> _pathOverrideSettings = new();
        private ISerializationManager _serializationManager;
        private IFileSystem _fileSystem;
        private PathSettings _commonPathSettings = new();

        /// <summary>
        /// if not null, all configs will be use path like this: <br/> 
        /// "CommonRelativeDirectoryPath" + "ConfigFileRelativePath" 
        /// </summary>
        public string? CommonRelativeDirectoryPath
        {
            get => _commonPathSettings.RelativeDirectoryPath;
            set => _commonPathSettings.SetRelativeDirectoryPath(value);
        }
        public int CheckDataCorrectnessTimeoutInMilliseconds { get; set; } = 5000;
        public int SerializationTimeoutInMilliseconds { get; set; } = 5000;
        public int DeserializationTimeoutInMilliseconds { get; set; } = 5000;
        public int ConfigCreationTimeoutInMilliseconds { get; set; } = 5000;
        public int ConfigDeletingTimeoutInMilliseconds { get; set; } = 5000;
        public int FileWriteTimeoutInMilliseconds { get; set; } = 5000;

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="registeringConfigTypes">
        /// Unique types of config objects. <br/>
        /// Registering type should have at least one constructor without parameters.
        /// </param>
        public ConfigsService(
            ISerializationManager serializationManager,
            IFileSystem fileSystem,
            params Type[] registeringConfigTypes)
        {
            foreach (var registeringType in registeringConfigTypes)
            {
                RegisterConfigType(registeringType.FullName, registeringType.Assembly, null);
            }

            _serializationManager = serializationManager;
            _fileSystem = fileSystem;
        }

        public ConfigsService(
            ISerializationManager serializationManager,
            IFileSystem fileSystem,
            params (Type, PathSettings?)[] registeringConfigTypes)
        {
            foreach (var registeringType in registeringConfigTypes)
            {
                RegisterConfigType(registeringType.Item1.FullName, registeringType.Item1.Assembly, registeringType.Item2);
            }

            _serializationManager = serializationManager;
            _fileSystem = fileSystem;
        }

        #endregion Constructors

        #region Registration
  
        public void RegisterConfigType(string? configTypeName, Assembly assembly, PathSettings? pathOverrideSettigns = null)
        {
            if (string.IsNullOrEmpty(configTypeName))
            {
                throw new ArgumentNullException($"{nameof(configTypeName)} cannot be null or empty!");
            }

            if (_registeredConfigs.ContainsKey(configTypeName))
            {
                throw new ArgumentException(
                    $"Registering types should be unique! \n" +
                    $"\"{configTypeName}\" type already registered!");
            }

            Type? type = assembly!.GetType(configTypeName);
            object typeInstance = Activator.CreateInstance(type!)!;


            _registeredConfigs.TryAdd(configTypeName, typeInstance);
            _pathOverrideSettings.TryAdd(configTypeName, pathOverrideSettigns);           
        }
 
        public void UnregisterConfigType(string? configTypeName)
        {
            if (string.IsNullOrEmpty(configTypeName))
            {
                throw new ArgumentNullException($"{nameof(configTypeName)} cannot be null or empty!");
            }

            CheckIsConfigTypeExist(configTypeName);

            _registeredConfigs.TryRemove(configTypeName, out var removingValue1);
            _pathOverrideSettings.TryRemove(configTypeName, out var removingValue2);      
        }

        #endregion

        #region PathOverrideSettings

        public bool IsPathOverrideSettingsNull(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);
            return _pathOverrideSettings[configTypeName] == null;
        }

        public void SetPathOverrideSettings(string configTypeName, PathSettings pathOverrideSettings)
        {
            CheckIsConfigTypeExist(configTypeName);
            _pathOverrideSettings[configTypeName] = pathOverrideSettings;
        }

        /// <summary>
        /// <inheritdoc cref="CheckIsConfigTypeExist"/>
        /// </summary>
        public PathSettings? GetPathOverrideSettings(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);
            return _pathOverrideSettings[configTypeName];
        }

        #endregion

        #region RegisteredConfigsInfo

        public IReadOnlyDictionary<string, object> RegisteredConfigs => _registeredConfigs;
      
        public object GetConfig(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);
            return _registeredConfigs[configTypeName];
        }

        public bool IsConfigExist(string configTypeName)
        {
            return _registeredConfigs.ContainsKey(configTypeName);
        }

        #endregion RegisteredConfigsInfo

        #region ConfigDataCorrectness
        
        public Task CheckDataCorrectnessAsync(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);

            return CheckDataCorrectnessAsync(_registeredConfigs[configTypeName], true)
                .WaitAsync(CheckDataCorrectnessTimeoutInMilliseconds);
        }

        private async Task CheckDataCorrectnessAsync(object configObject, bool checkDataCorrectness)
        {
            if (checkDataCorrectness
             && typeof(IDataCorrectnessChecker).IsAssignableFrom(configObject.GetType()))
            {
                IDataCorrectnessChecker dataCorrectnessChecker = (IDataCorrectnessChecker)configObject;

                await dataCorrectnessChecker.CheckDataCorrectnessAsync()
                         .WaitAsync(CheckDataCorrectnessTimeoutInMilliseconds);
            }
        }

        #endregion ConfigDataCorrectness

        #region CreateConfig
          
        public Task CreateConfigFileAsync(string configTypeName)
        {
            return CreateConfigFileBaseAsync(configTypeName)
                .WaitAsync(SerializationTimeoutInMilliseconds + ConfigCreationTimeoutInMilliseconds);
        }

        private async Task CreateConfigFileBaseAsync(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);

            object configObject = _registeredConfigs[configTypeName];
            string configFilePath = GetFilePathForConfig(configObject.GetType().FullName!);

            await _fileSystem.CreateDirectoriesAlongPathAsync(configFilePath);

            if (!_fileSystem.IsFileExist(configFilePath))
            {
                using (IFileStream stream = _fileSystem.Create(configFilePath))
                {
                    byte[] serializationData = await SerializeConfigObjectAsync(configObject)
                        .WaitAsync(SerializationTimeoutInMilliseconds);

                    await stream.WriteAsync(serializationData, 0, serializationData.Length)
                        .WaitAsync(ConfigCreationTimeoutInMilliseconds);
                }
            }

        }

        #endregion CreateConfig

        #region DeleteConfig

        public Task DeleteConfigFileAsync(string configTypeName)
        {
            return DeleteConfigFileBaseAsync(configTypeName)
                .WaitAsync(ConfigDeletingTimeoutInMilliseconds);
        }

        private async Task DeleteConfigFileBaseAsync(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);
            string configFilePath = GetFilePathForConfig(configTypeName);

            if (_fileSystem.IsFileExist(configFilePath))
            {
                await _fileSystem.DeleteAsync(configFilePath)
                    .WaitAsync(ConfigDeletingTimeoutInMilliseconds); ;
            }
        }

        #endregion

        #region SaveConfig     

        public Task SaveConfigToFileAsync(string configTypeName, bool checkDataCorrectness = true)
        {
            int operationTime = CheckDataCorrectnessTimeoutInMilliseconds
                + SerializationTimeoutInMilliseconds
                + FileWriteTimeoutInMilliseconds;

            return SaveConfigToFileBaseAsync(configTypeName, checkDataCorrectness)
                .WaitAsync(operationTime);
        }

        private async Task SaveConfigToFileBaseAsync(string configTypeName, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configTypeName);

            object configObject = _registeredConfigs[configTypeName];

            try
            {
                await CheckDataCorrectnessAsync(configObject, checkDataCorrectness)
                    .WaitAsync(CheckDataCorrectnessTimeoutInMilliseconds);
            }
            catch
            {
                await Console.Out.WriteLineAsync($"\"{configTypeName}\" serialization canceled due to exception!");
                throw;
            }

            string configFilePath = GetFilePathForConfig(configObject.GetType().FullName!);

            byte[] serializationData = await SerializeConfigObjectAsync(configObject)
                .WaitAsync(SerializationTimeoutInMilliseconds);

            if (_fileSystem.IsFileExist(configFilePath))
            {
                using (IFileStream stream = _fileSystem.OpenWrite(configFilePath))
                {
                    await stream.WriteAsync(serializationData, 0, serializationData.Length)
                        .WaitAsync(FileWriteTimeoutInMilliseconds);
                }
            }
            else
            {
                await _fileSystem.CreateDirectoriesAlongPathAsync(configFilePath);
                using (IFileStream stream = _fileSystem.Create(configFilePath))
                {
                    await stream.WriteAsync(serializationData, 0, serializationData.Length)
                        .WaitAsync(FileWriteTimeoutInMilliseconds);
                }
            }
        }

        #endregion SaveConfig

        #region LoadConfig

        public Task LoadConfigFromFileAsync(string configTypeName, bool checkDataCorrectness = true)
        {
            int operationTime = CheckDataCorrectnessTimeoutInMilliseconds
                + DeserializationTimeoutInMilliseconds;

            return LoadConfigFromBaseFileAsync(configTypeName, checkDataCorrectness)
                .WaitAsync(operationTime);
        }

        private async Task LoadConfigFromBaseFileAsync(string configTypeName, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configTypeName!);
            object configObject = _registeredConfigs[configTypeName];

            string configFilePath = GetFilePathForConfig(configTypeName);

            if (_fileSystem.IsFileExist(configFilePath))
            {
                var data = await _fileSystem.ReadAllBytesAsync(configFilePath);
                Type configObjectType = configObject.GetType();

                await _serializationManager.DeserializeAsync(configObject, data)
                    .WaitAsync(DeserializationTimeoutInMilliseconds);

                try
                {
                    await CheckDataCorrectnessAsync(configObject, checkDataCorrectness)
                        .WaitAsync(CheckDataCorrectnessTimeoutInMilliseconds);
                }
                catch
                {
                    await Console.Out.WriteLineAsync($"\"{configObjectType.Name}\" deserialization canceled due to exception!");
                    throw;
                }

                if (typeof(ISerializationListner).IsAssignableFrom(configObjectType))
                {
                    ISerializationListner serializableObject = (ISerializationListner)configObject;
                    serializableObject.OnAfterDeserialized();
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cant read \"{configObject.GetType().Name}\" config data, " +
                    $"file with path: \"{configFilePath}\" does not exist!");
            }
        }

        #endregion LoadConfig

        #region Common

        /// <summary>
        /// Throw <see cref="ArgumentException"/> if <paramref name="configTypeName"/>
        /// not contained in <see cref="RegisteredConfigs"/>
        /// </summary>
        private void CheckIsConfigTypeExist(string configTypeName)
        {
            if (!_registeredConfigs.ContainsKey(configTypeName))
            {
                throw new ArgumentException($"{this.GetType().Name}" +
                    $" does not contain registered config with type: \"{configTypeName}\"");
            }
        }

        private async Task<byte[]> SerializeConfigObjectAsync(object configObject)
        {
            Type configObjectType = configObject.GetType();

            if (typeof(ISerializationListner).IsAssignableFrom(configObjectType))
            {
                ISerializationListner serializableObject = (ISerializationListner)configObject;
                serializableObject.OnBeforeSerialize();
            }

            return await _serializationManager.SerializeAsync(configObject)
                .WaitAsync(SerializationTimeoutInMilliseconds);
        }

        private string GetFilePathForConfig(string configTypeName)
        {
            string appDirectory = _fileSystem.GetApplicationDirectory();
            string commonDictionary = CommonRelativeDirectoryPath == null ? string.Empty : CommonRelativeDirectoryPath;
            string configPath = ConfigInfoUtilities.GetRelativePathForConfigFile(
                _registeredConfigs[configTypeName]!.GetType(), _pathOverrideSettings[configTypeName]);

            return Path.Combine(appDirectory, commonDictionary, configPath);
        }

        #endregion Common
    }
}