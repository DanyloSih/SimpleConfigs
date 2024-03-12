namespace SimpleConfigs.Core
{
    public interface ISerializationListner
    {
        public void OnBeforeSerialize();

        public void OnAfterDeserialized();
    }
}