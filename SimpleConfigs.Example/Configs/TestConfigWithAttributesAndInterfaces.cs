using Newtonsoft.Json;
using SimpleConfigs.Attributes;
using SimpleConfigs.Core;

namespace SimpleConfigs.Example.Configs
{
    [RelativePath("RelativePath2"), ConfigName("DataStampts.txt")]
    public class TestConfigWithAttributesAndInterfaces : IDataCorrectnessChecker, ISerializationListner
    {
        [JsonProperty("just_int")] public int JustInt = -1;
        [JsonProperty("application_start_ticks")] private long _applicationStartTicks = -1;
        [JsonProperty("application_stop_ticks")] private long _applicationStopTicks = -1;

        [JsonIgnore()] public DateTime ApplicationStartTime;
        [JsonIgnore()] public DateTime ApplicationStopTime;
        

        public TestConfigWithAttributesAndInterfaces()
        {
            ApplicationStartTime = DateTime.Now;
            ApplicationStopTime = DateTime.MinValue;

            _applicationStartTicks = ApplicationStartTime.Ticks;
            _applicationStopTicks = ApplicationStopTime.Ticks;
        }

        public Task CheckDataCorrectnessAsync()
        {
            if (_applicationStartTicks < 0 || _applicationStopTicks < 0)
            {
                throw new ConfigDataIncorrectException("Time ticks should not be less then zero.");
            }

            return Task.CompletedTask;
        }

        public void OnAfterDeserialized()
        {
            ApplicationStartTime = new DateTime(_applicationStartTicks);
            ApplicationStopTime = new DateTime(_applicationStopTicks);
        }

        public void OnBeforeSerialize()
        {
            _applicationStartTicks = ApplicationStartTime.Ticks;
            _applicationStopTicks = ApplicationStopTime.Ticks;
        }
    }
}
