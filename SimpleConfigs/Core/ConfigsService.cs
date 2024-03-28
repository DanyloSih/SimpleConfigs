using System.Collections.Concurrent;
using System.Reflection;
using SimpleConfigs.Core.ConfigsServiceInterfaces;
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
        
        public async Task CheckDataCorrectnessAsync(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);
            await CheckDataCorrectnessAsync(_registeredConfigs[configTypeName], true);
        }

        private async Task CheckDataCorrectnessAsync(object configObject, bool checkDataCorrectness)
        {
            if (checkDataCorrectness
             && typeof(IDataCorrectnessChecker).IsAssignableFrom(configObject.GetType()))
            {
                IDataCorrectnessChecker dataCorrectnessChecker = (IDataCorrectnessChecker)configObject;
                try
                {
                    await dataCorrectnessChecker.CheckDataCorrectnessAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        #endregion ConfigDataCorrectness

        #region CreateConfig
          
        public async Task CreateConfigFileAsync(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);

            object configObject = _registeredConfigs[configTypeName];
            string configFilePath = GetFilePathForConfig(configObject.GetType().FullName!);

            await _fileSystem.CreateDirectoriesAlongPathAsync(configFilePath);

            if (!_fileSystem.IsFileExist(configFilePath))
            {
                using (IFileStream stream = _fileSystem.Create(configFilePath))
                {
                    byte[] serializationData = await SerializeConfigObjectAsync(configObject);
                    await stream.WriteAsync(serializationData, 0, serializationData.Length);
                }
            }
        }

        #endregion CreateConfig

        #region DeleteConfig
        
        public async Task DeleteConfigFileAsync(string configTypeName)
        {
            CheckIsConfigTypeExist(configTypeName);
            string configFilePath = GetFilePathForConfig(configTypeName);

            if (_fileSystem.IsFileExist(configFilePath))
            {
                await _fileSystem.DeleteAsync(configFilePath);
            }
        }

        #endregion

        #region SaveConfig     
      
        public async Task SaveConfigToFileAsync(string configTypeName, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configTypeName);

            object configObject = _registeredConfigs[configTypeName];

            await CheckDataCorrectnessAsync(configObject, checkDataCorrectness);

            string configFilePath = GetFilePathForConfig(configObject.GetType().FullName!);

            byte[] serializationData = await SerializeConfigObjectAsync(configObject);

            if (_fileSystem.IsFileExist(configFilePath))
            {
                using (IFileStream stream = _fileSystem.OpenWrite(configFilePath))
                {
                    await stream.WriteAsync(serializationData, 0, serializationData.Length);
                }
            }
            else
            {
                await _fileSystem.CreateDirectoriesAlongPathAsync(configFilePath);
                using (IFileStream stream = _fileSystem.Create(configFilePath))
                {
                    await stream.WriteAsync(serializationData, 0, serializationData.Length);
                }
            }
        }

        #endregion SaveConfig

        #region LoadConfig
           
        public async Task LoadConfigFromFileAsync(string configTypeName, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configTypeName!);
            object configObject = _registeredConfigs[configTypeName];

            string configFilePath = GetFilePathForConfig(configTypeName);

            if (_fileSystem.IsFileExist(configFilePath))
            {
                var data = await _fileSystem.ReadAllBytesAsync(configFilePath);
                await DeserializeConfigObjectAsync(configObject, data, checkDataCorrectness);
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

            return await _serializationManager.SerializeAsync(configObject);
        }

        private async Task DeserializeConfigObjectAsync(object populatingConfigObject, byte[] serializationData, bool checkDataCorrectness = true)
        {
            Type configObjectType = populatingConfigObject.GetType();

            await _serializationManager.DeserializeAsync(populatingConfigObject, serializationData);
            await CheckDataCorrectnessAsync(populatingConfigObject, checkDataCorrectness);

            if (typeof(ISerializationListner).IsAssignableFrom(configObjectType))
            {
                ISerializationListner serializableObject = (ISerializationListner)populatingConfigObject;
                serializableObject.OnAfterDeserialized();
            }
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