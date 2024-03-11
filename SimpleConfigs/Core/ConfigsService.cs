using SimpleConfigs.Core.SerializationManagers;
using SimpleConfigs.Utilities;

namespace SimpleConfigs.Core
{
    /// <summary>
    /// This object manage config objects life.
    /// </summary>
    public class ConfigsService
    {
        private Dictionary<Type, object> _registeredConfigs = new Dictionary<Type, object>();
        private ISerializationManager _serializationManager;

#region Constructors
        /// <summary>
        /// </summary>
        /// <param name="registeringConfigTypes">
        /// Unique types of config objects. <br/>
        /// Registering type should have at least one constructor without parameters.
        /// </param>
        public ConfigsService(ISerializationManager serializationManager, params Type[] registeringConfigTypes)
        {
            foreach (var registeringType in registeringConfigTypes)
            {
                if (_registeredConfigs.ContainsKey(registeringType))
                {
                    throw new ArgumentException("Registreing types should be unique!");
                }
                _registeredConfigs.Add(registeringType, Activator.CreateInstance(registeringType)!);
            }

            _serializationManager = serializationManager;
        }

        /// <summary>
        /// </summary>
        /// <param name="registeringConfigTypes">
        /// Unique types of config objects. <br/>
        /// Registering type should have at least one constructor without parameters.
        /// </param>
        public ConfigsService(params Type[] registeringConfigTypes)
            : this(new JsonSerializationManager(), registeringConfigTypes)
        {
        }
        #endregion

#region Initialization
        /// <summary>
        /// Creates non-existent config files with default values and loads data from existing config files.
        /// </summary>
        public void InitializeAllConfigs(bool checkDataCorrectness = true)
        {
            CreateConfigFiles();
            LoadConfigsFromFiles(checkDataCorrectness);
        }

        /// <summary>
        /// Creates non-existent config files with default values and after that
        /// loads data from existing config files. <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public void InitializeConfig(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            CreateConfigFile(configType);
            LoadConfigFromFile(configType);
        }

        /// <summary>
        /// Creates non-existent config files with default values and loads data from existing config files. <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public void InitializeConfig<T>()
        {
            CheckIsConfigTypeExist<T>();
            CreateConfigFile<T>();
            LoadConfigFromFile<T>();
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
        /// Returns all <see cref="_registeredConfigs"/> (Type, Object)
        /// </summary>
        public IEnumerable<KeyValuePair<Type, object>> GetRegisteredConfigs() => _registeredConfigs;

        /// <summary>
        /// Return true if <paramref name="configType"/> contained in <see cref="_registeredConfigs"/>
        /// </summary>
        public bool IsConfigExist(Type configType)
        {
            return _registeredConfigs.ContainsKey(configType);
        }

        /// <summary>
        /// Return true if type in <typeparamref name="T"/> contained in <see cref="_registeredConfigs"/>
        /// </summary>
        public bool IsConfigExist<T>()
        {
            return _registeredConfigs.ContainsKey(typeof(T));
        }
        #endregion

#region ConfigDataCorrectness
        /// <summary>
        /// Invoke <see cref="IDataCorrectnessChecker.CheckDataCorrectness"/> method in all config objects <br/> 
        /// with <see cref="IDataCorrectnessChecker"/> interface.
        /// </summary>
        public void CheckConfigsDataCorrectness()
        {
            foreach (var configObject in _registeredConfigs)
            {
                CheckDataCorrectness(configObject.Value, true);
            }
        }

        /// <summary>
        /// <inheritdoc cref="CheckDataCorrectness(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public void CheckDataCorrectness(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            CheckDataCorrectness(_registeredConfigs[configType], true);
        }

        /// <summary>
        /// <inheritdoc cref="CheckDataCorrectness(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public void CheckDataCorrectness<T>()
        {
            CheckIsConfigTypeExist(typeof(T));
            CheckDataCorrectness(_registeredConfigs[typeof(T)], true);
        }

        /// <summary>
        /// Invoke <see cref="IDataCorrectnessChecker.CheckDataCorrectness"/> method <br/>
        /// if config implement <see cref="IDataCorrectnessChecker"/> interface.
        /// </summary>
        private void CheckDataCorrectness(object configObject, bool checkDataCorrectness)
        {
            if (checkDataCorrectness
             && typeof(IDataCorrectnessChecker).IsAssignableFrom(configObject.GetType()))
            {
                IDataCorrectnessChecker dataCorrectnessChecker = (IDataCorrectnessChecker)configObject;
                dataCorrectnessChecker.CheckDataCorrectness();
            }
        }
#endregion

#region CreateConfig
        /// <summary>
        /// Create configuration files and specified directories along his paths, if they do not already exist.
        /// </summary>
        public void CreateConfigFiles()
        {
            foreach (var configObject in _registeredConfigs)
            {
                CreateConfigFile(configObject.Value);
            }
        }

        /// <summary>
        /// <inheritdoc cref="CreateConfigFile(object)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public void CreateConfigFile<T>()
        {
            CheckIsConfigTypeExist<T>();
            CreateConfigFile(_registeredConfigs[typeof(T)]);
        }

        /// <summary>
        /// <inheritdoc cref="CreateConfigFile(object)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public void CreateConfigFile(Type configType)
        {
            CheckIsConfigTypeExist(configType);
            CreateConfigFile(_registeredConfigs[configType]);
        }

        /// <summary>
        /// Create configuration file and specified directories along his path, if they do not already exist.
        /// </summary>
        private void CreateConfigFile(object configObject)
        {
            string configFilePath = ConfigInfoUtilities.GetFullPathForConfigFile(configObject);
            PathUtilities.CreateDirectoriesAlongPath(configFilePath);

            if (!File.Exists(configFilePath))
            {
                using (FileStream stream = File.Create(configFilePath))
                {
                    byte[] serializationData = SerializeConfigObject(configObject);
                    stream.Write(serializationData, 0, serializationData.Length);
                }
            }
        }
#endregion

#region SaveConfig
        /// <summary>
        /// Creates config files with serialization data, 
        /// if they do not already exist, or overwriting already exiting files.
        /// </summary>
        public void SaveConfigsToFiles(bool checkDataCorrectness = true)
        {
            foreach (var configObject in _registeredConfigs)
            {
                SaveConfigToFile(configObject.Value, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="SaveConfigToFile(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public void SaveConfigToFile(Type configType, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configType);
            SaveConfigToFile(_registeredConfigs[configType], checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="SaveConfigToFile(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public void SaveConfigToFile<T>(bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist<T>();
            SaveConfigToFile(typeof(T), checkDataCorrectness);
        }

        /// <summary>
        /// Creates config file with serialized data, 
        /// if it do not already exist, or overwriting already existing config file. <br/> <br/>
        /// Before serialization "<inheritdoc cref="CheckDataCorrectness(object, bool)"/>"
        /// </summary>
        private void SaveConfigToFile(object configObject, bool checkDataCorrectness)
        {
            CheckDataCorrectness(configObject, checkDataCorrectness);

            string configFilePath = ConfigInfoUtilities.GetFullPathForConfigFile(configObject);

            byte[] serializationData = SerializeConfigObject(configObject);

            if (File.Exists(configFilePath))
            {
                using (FileStream stream = File.OpenWrite(configFilePath))
                {
                    stream.Write(serializationData, 0, serializationData.Length);
                }
            }
            else
            {
                PathUtilities.CreateDirectoriesAlongPath(configFilePath);
                using (FileStream stream = File.Create(configFilePath))
                {
                    stream.Write(serializationData, 0, serializationData.Length);
                }
            }
        }
#endregion

#region LoadConfig
        /// <summary>
        /// Load deserialized data from config files, 
        /// if they exist, if not - throw <see cref="InvalidOperationException"/>.
        /// </summary>
        public void LoadConfigsFromFiles(bool checkDataCorrectness = true)
        {
            foreach (var configObject in _registeredConfigs)
            {
                LoadConfigFromFile(configObject.Value, checkDataCorrectness);
            }
        }

        /// <summary>
        /// <inheritdoc cref="LoadConfigFromFile(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist(Type)"/>
        /// </summary>
        public void LoadConfigFromFile(Type configType, bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist(configType);
            LoadConfigFromFile(_registeredConfigs[configType], checkDataCorrectness);
        }

        /// <summary>
        /// <inheritdoc cref="LoadConfigFromFile(object, bool)"/> <br/> <br/>
        /// <inheritdoc cref="CheckIsConfigTypeExist{T}()"/>
        /// </summary>
        public void LoadConfigFromFile<T>(bool checkDataCorrectness = true)
        {
            CheckIsConfigTypeExist<T>();
            LoadConfigFromFile(_registeredConfigs[typeof(T)], checkDataCorrectness);
        }

        /// <summary>
        /// Load deserealized config data from file, if file exist, if not - throw <see cref="InvalidOperationException"/>. <br/> <br/>
        /// After deserialization "<inheritdoc cref="CheckDataCorrectness(object, bool)"/>"
        /// </summary>
        private void LoadConfigFromFile(object configObject, bool checkDataCorrectness)
        {
            string configFilePath = ConfigInfoUtilities.GetFullPathForConfigFile(configObject);

            if (File.Exists(configFilePath))
            {
                DeserializeConfigObject(configObject, File.ReadAllBytes(configFilePath));
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cant read \"{configObject.GetType().Name}\" config data, " +
                    $"file with path: \"{configFilePath}\" does not exist!");
            }

            CheckDataCorrectness(configObject, checkDataCorrectness);
        }
#endregion

#region Common
        /// <summary>
        /// Throw <see cref="ArgumentException"/> if <paramref name="configType"/> 
        /// not contained in <see cref="_registeredConfigs"/>
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
        /// does not contained in <see cref="_registeredConfigs"/>
        /// </summary>
        private void CheckIsConfigTypeExist<T>()
        {
            CheckIsConfigTypeExist(typeof(T));
        }

        private byte[] SerializeConfigObject(object configObject)
        {
            Type configObjectType = configObject.GetType();

            if (typeof(ISerializationListner).IsAssignableFrom(configObjectType))
            {
                ISerializationListner serializableObject = (ISerializationListner)configObject;
                serializableObject.OnBeforeSerialize();
            }

            return _serializationManager.Serialize(configObject);
        }

        private void DeserializeConfigObject(object populatingConfigObject, byte[] serializationData)
        {
            Type configObjectType = populatingConfigObject.GetType();

            _serializationManager.Deserialize(populatingConfigObject, serializationData);

            if (typeof(ISerializationListner).IsAssignableFrom(configObjectType))
            {
                ISerializationListner serializableObject = (ISerializationListner)populatingConfigObject;
                serializableObject.OnAfterDeserialized();
            }
        }
#endregion
    }
}
