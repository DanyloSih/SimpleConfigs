using System.Collections.Immutable;
using System.Threading.Tasks;
using SimpleConfigs.Core.ConfigsServiceInterfaces;
using SimpleConfigs.Utilities;

namespace SimpleConfigs.Core
{
    /// <summary>
    /// This object takes full control over config files paths.
    /// </summary>
    public class ConfigsServicesHub
    {
        private IFileSystem _fileSystem;
        private ISerializationManager _serializer;
        private ConfigsServicesPathsFormater _pathsFormater;
        private List<ConfigsService> _configsServices;

        public IReadOnlyList<IConfigsServicesHubMember> ConfigsServices
            => _configsServices;

        public ConfigsServicesHub(
            IFileSystem fileSystem,
            ISerializationManager serializer,
            ConfigsServicesPathsFormater pathsFormater,
            int servicesCount)
        {
            _fileSystem = fileSystem;
            _serializer = serializer;
            _pathsFormater = pathsFormater;
            _configsServices = new();

            for (int i = 0; i < servicesCount; i++)
            {
                CreateNewConfigsService();
            } 
        }

        public async Task InitializeTypeForAllAsync(Type configType, bool inParallel = false)
        {
            await AsyncUtilities.ForEachAsync<ConfigsService, string>(
                   _configsServices,
                   configType.FullName!,
                   async (value, context) => await value.InitializeConfigAsync(context),
                   inParallel);
        }

        public async Task SaveTypeForAllAsync(Type configType, bool inParallel = false)
        {
            await AsyncUtilities.ForEachAsync<ConfigsService, string>(
                   _configsServices,
                   configType.FullName!,
                   async (value, context) => await value.SaveConfigToFileAsync(context),
                   inParallel);
        }

        public async Task LoadTypeForAllAsync(Type configType, bool checkDataCorrectness = true, bool inParallel = false)
        {
            await AsyncUtilities.ForEachAsync<ConfigsService, string>(
                   _configsServices,
                   configType.FullName!,
                   async (value, context) => await value.LoadConfigFromFileAsync(context, checkDataCorrectness),
                   inParallel);
        }

        public async Task CreateTypeFileForAllAsync(Type configType, bool inParallel = false)
        {
            await AsyncUtilities.ForEachAsync<ConfigsService, string>(
                   _configsServices,
                   configType.FullName!,
                   async (value, context) => await value.CreateConfigFileAsync(context),
                   inParallel);
        }

        public async Task DeleteTypeFileForAllAsync(Type configType, bool inParallel = false)
        {
            await AsyncUtilities.ForEachAsync<ConfigsService, string>(
                   _configsServices,
                   configType.FullName!,
                   async (value, context) => await value.DeleteConfigFileAsync(context),
                   inParallel);
        }

        public async Task CheckDataCorrectnessForAllAsync(Type configType, bool inParallel = false)
        {
            await AsyncUtilities.ForEachAsync<ConfigsService, string>(
                   _configsServices,
                   configType.FullName!,
                   async (value, context) => await value.CheckDataCorrectnessAsync(context),
                   inParallel);
        }

        public List<T?> GetTypeInstances<T>()
        {
            List<T?> instances = new List<T?>(_configsServices.Count);
            for (int i = 0; i < _configsServices.Count; i++)
            {
                instances.Add(
                    _configsServices[i].IsConfigExist<T>() ? _configsServices[i].GetConfig<T>() : default);
            }

            return instances;
        }

        #region ServicesInstancesManagement

        public IConfigsServicesHubMember CreateNewConfigsService()
        {
            var configsService = new ConfigsService(_serializer, _fileSystem, new Type[] { });
            int id = _configsServices.Count;

            configsService.CommonRelativeDirectoryPath = Path.Combine(
                    _pathsFormater.CommonRelativeDirectory,
                    _pathsFormater.GetFormatedSubdirectory(id));

            _configsServices.Add(configsService);

            return configsService;
        }

        public void RemoveConfigsService(IConfigsServicesHubMember configsService)
        {
            int index = _configsServices.FindIndex(x => x == configsService);

            if (index >= 0)
            {
                RemoveConfigsServiceAt(index);
            }
            else
            {
                throw new ArgumentException($"\"{nameof(configsService)}\" does not " +
                    $"contained in \"{nameof(ConfigsServices)}\" list!");
            }
        }

        public void RemoveConfigsServiceAt(int configsServiceId)
        {
            _configsServices.RemoveAt(configsServiceId);
        }

        #endregion

        #region Registering

        public void RegisterType(int configsServiceId, Type configType, string filename)
        {
            string formatedName = _pathsFormater.GetFormatedFileName(filename, configsServiceId);

            _configsServices[configsServiceId]
                .RegisterConfigType(configType, new PathSettings(null, formatedName));
        }

        public void UnregisterType(int configsServiceId, Type configType)
        {
            _configsServices[configsServiceId].UnregisterConfigType(configType);
        }

        public void RegisterTypeForAll(Type configType, string filename)
        {
            for (int i = 0; i < _configsServices.Count; i++)
            {
                if (!_configsServices[i].IsConfigExist(configType))
                {
                    RegisterType(i, configType, filename);
                }
            }
        }

        public void UnregisterTypeForAll(Type configType)
        {
            for (int i = 0; i < _configsServices.Count; i++)
            {
                if (_configsServices[i].IsConfigExist(configType))
                {
                    UnregisterType(i, configType);
                }
            }
        }

        #endregion
    }
}