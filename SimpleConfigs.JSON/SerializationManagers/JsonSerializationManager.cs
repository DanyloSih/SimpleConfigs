using System.Text;
using Newtonsoft.Json;
using SimpleConfigs.Core;

namespace SimpleConfigs.JSON.SerializationManagers
{
    public class JsonSerializationManager : ISerializationManager
    {
        public async Task DeserializeAsync(object populatingObject, byte[] serializationData)
        {
            string serializationDataString = Encoding.UTF8.GetString(serializationData);
            
            await Task.Run(() => JsonConvert.PopulateObject(
                serializationDataString, 
                populatingObject, 
                new JsonSerializerSettings
                {
                    ContractResolver = new CollectionClearingContractResolver(),
                }));
        }

        public async Task<byte[]> SerializeAsync(object serializableObject)
        {
            string serializationDataString = await Task.Run(
                () => JsonConvert.SerializeObject(serializableObject, Formatting.Indented));

            return Encoding.UTF8.GetBytes(serializationDataString);
        }
    }
}
