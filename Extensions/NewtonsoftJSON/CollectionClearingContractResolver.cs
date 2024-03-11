using System.Collections;
using Newtonsoft.Json.Serialization;

namespace SimpleConfigs.Extensions.NewtonsoftJSON
{
    public class CollectionClearingContractResolver : DefaultContractResolver
    {
        protected override JsonArrayContract CreateArrayContract(Type objectType)
        {
            var c = base.CreateArrayContract(objectType);
            c.OnDeserializingCallbacks.Add((obj, streamingContext) =>
            {
                if (obj == null)
                {
                    return;
                }

                IList list = (obj as IList)!;
                if (!list.IsReadOnly)
                {
                    list.Clear();
                    return;
                }

                IDictionary dictionary = (obj as IDictionary)!;
                if(!dictionary.IsReadOnly)
                {
                    dictionary.Clear();
                }

                if (obj.GetType().IsArray)
                {
                    Array array = (obj as Array)!;
                    Array.Clear(array);
                }
            });
            return c;
        }
    }
}
