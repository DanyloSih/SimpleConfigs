using SimpleConfigs.Attributes;

namespace SimpleConfigs.Test.Configs
{
    [RelativePath("RelativePath"), ConfigName("TestConfigName.cfg")]
    public class TestConfigWithRelativePathAndNameAttribute
    {
        public int LoadingAssetsDelay = 1500;
        public int AttempsCount = 6;
        public int AttempsDelay = 300;
    }
}
