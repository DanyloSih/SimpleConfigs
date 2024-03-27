using SimpleConfigs.Core;
using SimpleConfigs.JSON.SerializationManagers;

namespace SimpleConfigs.JSON.Core
{
    public class LocalJSONConfigsService : ConfigsService
    {
        public LocalJSONConfigsService(params Type[] registeringConfigTypes) 
            : base(new JsonSerializationManager(), new LocalFileSystem(), registeringConfigTypes)
        {
        }

        public LocalJSONConfigsService(params (Type, PathSettings?)[] registeringConfigTypes) 
            : base(new JsonSerializationManager(), new LocalFileSystem(), registeringConfigTypes)
        {
        }
    }
}
