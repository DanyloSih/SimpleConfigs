using System.Text;
using Newtonsoft.Json;
using SimpleConfigs.Core;

namespace SimpleConfigs.JSON.SerializationManagers
{
    public class JsonSerializationManager : ISerializationManager
    {
        public void Deserialize(object populatingObject, byte[] serializationData)
        {
            string serializationDataString = Encoding.UTF8.GetString(serializationData);
            
            JsonConvert.PopulateObject(
                serializationDataString, 
                populatingObject, 
                new JsonSerializerSettings
                {
                    ContractResolver = new CollectionClearingContractResolver(),
                });
        }

        public byte[] Serialize(object serializableObject)
        {
            string serializationDataString = JsonConvert.SerializeObject(serializableObject, Formatting.Indented);

            return Encoding.UTF8.GetBytes(serializationDataString);
        }
    }
}
