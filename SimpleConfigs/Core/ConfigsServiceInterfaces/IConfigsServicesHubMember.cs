
using SimpleConfigs.Utilities;

namespace SimpleConfigs.Core.ConfigsServiceInterfaces
{
    public interface IConfigsServicesHubMember
        : IConfigDataCorrectnessChecker, IConfigFileCreator, IConfigFileDeleter,
        IConfigFromFileLoader, IConfigToFileSaver, IConfigsContainer, IConfigInitializer
    {

    }
}