using SimpleConfigs.Utilities;

namespace SimpleConfigs.Core
{
    /// <summary>
    /// This object manage config objects life.
    /// </summary>
    public class ConfigsService
    {
        private Dictionary<Type, object> _registeredConfigs = new();
        private Dictionary<Type, PathSettings?> _pathOverrideSettings = new();
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
                RegisterConfigType(registeringType, null);
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
                RegisterConfigType(registeringType.Item1, registeringType.Item2);
            }

            _serializationManager = serializationManager;
            _fileSystem = fileSystem;
        }

        #endregion Constructors

        #region Initialization

        /// <summary>
        /// Creates non-existent config files with default values and loads data from existing config files.
        /// </summary>
        public async Task InitializeConfigsAsync(bool checkDataCorrectness = true)
        {
            await CreateConfigFilesAsync();
            await LoadConfigsFromFilesAsync(checkDataCorrectness);
        }

        /// <summary>
        /// Creates non-existent config files with default values and after that
        /// loads data from existing config files. <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public async Task InitializeConfigAsync(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            await CreateConfigFileAsync(configType);
            await LoadConfigFromFileAsync(configType);
        }

        /// <summary>
        /// Creates non-existent config files with default values and loads data from existing config files. <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public async Task InitializeConfigAsync<T>()
        {
            CheckIsConfigTypeExist<T>();
            await CreateConfigFileAsync<T>();
            await LoadConfigFromFileAsync<T>();
        }

        #endregion Initialization

        #region Registration

        /// <summary>
        /// Create instance of <paramref name="configType"/> and create with it necessary data associations. <br/>
        /// After registration you be able to create, load, save config files for  <paramref name="configType"/>. <br/>
        /// Throw <see cref="ArgumentException"/> if <paramref name="configType"/> already registered.
        /// </summary>
        public void RegisterConfigType(Type configType, PathSettings? pathOverrideSettigns = null)
        {
            if (_registeredConfigs.ContainsKey(configType))
            {
                throw new ArgumentException(
                    $"Registering types should be unique! \n" +
                    $"\"{configType.Name}\" type already registered!");
            }
            _registeredConfigs.Add(configType, Activator.CreateInstance(configType)!);
            _pathOverrideSettings.Add(configType, pathOverrideSettigns);
        }

        /// <summary>
        /// After unregistering all data associations for <paramref name="configType"/> will be removed. <br/>
        /// You no longer be able to create, load, save config files for <paramref name="configType"/>. <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist"/>
        /// </summary>
        public void UnregisterConfigType(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            _registeredConfigs.Remove(configType);
            _pathOverrideSettings.Remove(configType);
        }

        #endregion

        #region PathOverrideSettings

        /// <summary>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public bool IsPathOverrideSettingsNull<T>()
        {
            CheckIsConfigTypeExist<T>();
            return _pathOverrideSettings[typeof(T)] == null;
        }

        /// <summary>
        /// <inheritdoc cref="CheckIsConfigTypeExist"/>
        /// </summary>
        public bool IsPathOverrideSettingsNull(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            return _pathOverrideSettings[configType] == null;
        }

        /// <summary>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public void SetPathOverrideSettings<T>(PathSettings pathOverrideSettings)
        {
            CheckIsConfigTypeExist<T>();
            _pathOverrideSettings[typeof(T)] = pathOverrideSettings;
        }

        /// <summary>
        /// <inheritdoc cref="CheckIsConfigTypeExist"/>
        /// </summary>
        public void SetPathOverrideSettings(Type configType, PathSettings pathOverrideSettings)
        {
            CheckIsConfigTypeExist(configType);
            _pathOverrideSettings[configType] = pathOverrideSettings;
        }

        /// <summary>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public PathSettings? GetPathOverrideSettings<T>()
        {
            CheckIsConfigTypeExist<T>();
            return _pathOverrideSettings[typeof(T)];
        }

        /// <summary>
        /// <inheritdoc cref="CheckIsConfigTypeExist"/>
        /// </summary>
        public PathSettings? GetPathOverrideSettings(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            return _pathOverrideSettings[configType];
        }

        #endregion

        #region RegisteredConfigsInfo

        /// <summary>
        /// <inheritdoc cref="GetConfig(Type)"/>
        /// </summary>
        public T GetConfig<T>()
        {
            return (T)GetConfig(typeof(T));
        }

        /// <summary>
        /// Return one config object from all registered objects via type.
        /// </summary>
        public object GetConfig(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            return _registeredConfigs[configType];
        }

        /// <summary>
        /// Returns all registered configs via constructor or 
        /// <see cref="RegisterConfigType(Type, PathSettings?)"/> method.
        /// </summary>
        public IReadOnlyDictionary<Type, object> RegisteredConfigs => _registeredConfigs;

        /// <summary>
        /// Return true if <paramref name="configType"/> contained in <see cref="RegisteredConfigs"/>
        /// </summary>
        public bool IsConfigExist(Type configType)
        {
            return _registeredConfigs.ContainsKey(configType);
        }

        /// <summary>
        /// Return true if type in <typeparamref name="T"/> contained in <see cref="RegisteredConfigs"/>
        /// </summary>
        public bool IsConfigExist<T>()
        {
            return _registeredConfigs.ContainsKey(typeof(T));
        }

        #endregion RegisteredConfigsInfo

        #region ConfigDataCorrectness

        /// <summary>
        /// Invoke <see cref="IDataCorrectnessChecker.CheckDataCorrectnessAsync"/> method in all config objects <br/>
        /// with <see cref="IDataCorrectnessChecker"/> interface.
        /// </summary>
        public async Task CheckConfigsDataCorrectnessAsync()
        {
            foreach (var configObject in _registeredConfigs)
            {
                await CheckDataCorrectnessAsync(configObject.Value, true);
            }
        }

        /// <summary>
        /// <inheritdoc cref="CheckDataCorrectnessAsync(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public async Task CheckDataCorrectnessAsync(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            await CheckDataCorrectnessAsync(_registeredConfigs[configType], true);
        }

        /// <summary>
        /// <inheritdoc cref="CheckDataCorrectnessAsync(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public async Task CheckDataCorrectnessAsync<T>()
        {
            CheckIsConfigTypeExist(typeof(T));
            await CheckDataCorrectnessAsync(_registeredConfigs[typeof(T)], true);
        }

        /// <summary>
        /// Invoke <see cref="IDataCorrectnessChecker.CheckDataCorrectnessAsync"/> method <br/>
        /// if config implement <see cref="IDataCorrectnessChecker"/> interface.
        /// </summary>
        private async Task CheckDataCorrectnessAsync(object configObject, bool checkDataCorrectness)
        {
            if (checkDataCorrectness
             && typeof(IDataCorrectnessChecker).IsAssignableFrom(configObject.GetType()))
            {
                IDataCorrectnessChecker dataCorrectnessChecker = (IDataCorrectnessChecker)configObject;
                await dataCorrectnessChecker.CheckDataCorrectnessAsync();
            }
        }

        #endregion ConfigDataCorrectness

        #region CreateConfig

        /// <summary>
        /// Create configuration files and specified directories along his paths, if they do not already exist.
        /// </summary>
        public async Task CreateConfigFilesAsync()
        {
            foreach (var configObject in _registeredConfigs)
            {
                await CreateConfigFileAsync(configObject.Value);
            }
        }

        /// <summary>
        /// <inheritdoc cref="CreateConfigFileAsync(object)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public async Task CreateConfigFileAsync<T>()
        {
            CheckIsConfigTypeExist<T>();
            await CreateConfigFileAsync(_registeredConfigs[typeof(T)]);
        }

        /// <summary>
        /// <inheritdoc cref="CreateConfigFileAsync(object)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public async Task CreateConfigFileAsync(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            await CreateConfigFileAsync(_registeredConfigs[configType]);
        }

        /// <summary>
        /// Create configuration file and specified directories along his path, if they do not already exist.
        /// </summary>
        private async Task CreateConfigFileAsync(object configObject)
        {
            string configFilePath = GetFilePathForConfig(configObject.GetType());
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

        /// <summary>
        /// Delete config files for all <see cref="RegisteredConfigs"/>
        /// </summary>
        public async Task DeleteAllConfigFilesAsync()
        {
            foreach (var config in _registeredConfigs)
            {
                await DeleteConfigFileAsync(config.Key);
            }
        }

        /// <summary>
        /// <inheritdoc cref="DeleteConfigFileBaseAsync(Type)"/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public async Task DeleteConfigFileAsync<T>()
        {
            CheckIsConfigTypeExist<T>();
            await DeleteConfigFileBaseAsync(typeof(T));
        }

        /// <summary>
        /// <inheritdoc cref="DeleteConfigFileBaseAsync(Type)"/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public async Task DeleteConfigFileAsync(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            await DeleteConfigFileBaseAsync(configType);
        }

        /// <summary>
        /// Deletes a config file if it exists.
        /// </summary>
        private async Task DeleteConfigFileBaseAsync(Type configType)
        {
            string configFilePath = GetFilePathForConfig(configType);

            if (_fileSystem.IsFileExist(configFilePath))
            {
                await _fileSystem.DeleteAsync(configFilePath);
            }
        }

        #endregion

        #region SaveConfig

        /// <summary>
        /// Creates config files with serialization data if they do not already exist, <br/> 
        /// or overwriting already exiting files.
        /// </summary>
        public async Task SaveConfigsToFilesAsync(bool checkDataCorrectness = true)
        {
            foreach (var configObject in _registeredConfigs)
            {
                await SaveConfigToFileAsync(configObject.Value, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="SaveConfigToFileAsync(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public async Task SaveConfigToFileAsync(Type configType, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configType);
            await SaveConfigToFileAsync(_registeredConfigs[configType], checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="SaveConfigToFileAsync(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public async Task SaveConfigToFileAsync<T>(bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist<T>();
            await SaveConfigToFileAsync(typeof(T), checkDataCorrectness);
        }

        /// <summary>
        /// Creates config file with serialized data,
        /// if it do not already exist, or overwriting already existing config file. <br/> <br/>
        /// Before serialization "<inheritdoc cref="CheckDataCorrectnessAsync(object, bool)"/>"
        /// </summary>
        private async Task SaveConfigToFileAsync(object configObject, bool checkDataCorrectness)
        {
            await CheckDataCorrectnessAsync(configObject, checkDataCorrectness);

            string configFilePath = GetFilePathForConfig(configObject.GetType());

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

        /// <summary>
        /// Load deserialized data from config files,
        /// if they exist, if not - throw <see cref="InvalidOperationException"/>.
        /// </summary>
        public async Task LoadConfigsFromFilesAsync(bool checkDataCorrectness = true)
        {
            foreach (var configObject in _registeredConfigs)
            {
                await LoadConfigFromFileAsync(configObject.Value, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="LoadConfigFromFileAsync(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public async Task LoadConfigFromFileAsync(Type configType, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configType);
            await LoadConfigFromFileAsync(_registeredConfigs[configType], checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="LoadConfigFromFileAsync(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public async Task LoadConfigFromFileAsync<T>(bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist<T>();
            await LoadConfigFromFileAsync(_registeredConfigs[typeof(T)], checkDataCorrectness);
        }

        /// <summary>
        /// Load deserealized config data from file, if file exist, if not - throw <see cref="InvalidOperationException"/>. <br/> <br/>
        /// After deserialization "<inheritdoc cref="CheckDataCorrectnessAsync(object, bool)"/>"
        /// </summary>
        private async Task LoadConfigFromFileAsync(object configObject, bool checkDataCorrectness)
        {
            string configFilePath = GetFilePathForConfig(configObject.GetType());

            if (_fileSystem.IsFileExist(configFilePath))
            {
                var data = await _fileSystem.ReadAllBytesAsync(configFilePath);
                await DeserializeConfigObjectAsync(configObject, data);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cant read \"{configObject.GetType().Name}\" config data, " +
                    $"file with path: \"{configFilePath}\" does not exist!");
            }

            await CheckDataCorrectnessAsync(configObject, checkDataCorrectness);
        }

        #endregion LoadConfig

        #region Common

        /// <summary>
        /// Throw <see cref="ArgumentException"/> if <paramref name="configType"/>
        /// not contained in <see cref="RegisteredConfigs"/>
        /// </summary>
        private void CheckIsConfigTypeExist(Type configType)
        {
            if (!_registeredConfigs.ContainsKey(configType))
            {
                throw new ArgumentException($"{this.GetType().Name}" +
                    $" does not contain registered config with type: \"{configType.Name}\"");
            }
        }

        /// <summary>
        /// Throw <see cref="ArgumentException"/> if config with <typeparamref name="T"/> type
        /// does not contained in <see cref="RegisteredConfigs"/>
        /// </summary>
        private void CheckIsConfigTypeExist<T>()
        {
            CheckIsConfigTypeExist(typeof(T));
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

        private async Task DeserializeConfigObjectAsync(object populatingConfigObject, byte[] serializationData)
        {
            Type configObjectType = populatingConfigObject.GetType();

            await _serializationManager.DeserializeAsync(populatingConfigObject, serializationData);

            if (typeof(ISerializationListner).IsAssignableFrom(configObjectType))
            {
                ISerializationListner serializableObject = (ISerializationListner)populatingConfigObject;
                serializableObject.OnAfterDeserialized();
            }
        }

        private string GetFilePathForConfig(Type configType)
        {
            string appDirectory = _fileSystem.GetApplicationDirectory();
            string commonDictionary = CommonRelativeDirectoryPath == null ? string.Empty : CommonRelativeDirectoryPath;
            string configPath = ConfigInfoUtilities.GetRelativePathForConfigFile(
                configType, _pathOverrideSettings[configType]);

            return Path.Combine(appDirectory, commonDictionary, configPath);
        }

        #endregion Common
    }
}