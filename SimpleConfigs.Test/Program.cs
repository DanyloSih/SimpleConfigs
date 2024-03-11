using SimpleConfigs.Core;
using SimpleConfigs.Core.SerializationManagers;
using SimpleConfigs.Test.Configs;

internal class Program
{
    private static ConfigsService? s_configsService;

    private static async Task Main(string[] args)
    {
        s_configsService = new ConfigsService(
            typeof(TestConfigWithoutAnyAttributesAndInterfaces),
            typeof(TestConfigWithRelativePathAttribute),
            typeof(TestConfigWithRelativePathAndNameAttribute),
            typeof(TestConfigWithAttributesAndInterfaces));

        s_configsService.InitializeAllConfigs();

       var consoleInfo = (TestConfigWithRelativePathAttribute)s_configsService
            .GetConfig(typeof(TestConfigWithRelativePathAttribute));
        Console.ForegroundColor = consoleInfo.ForegroundColor;
        Console.BackgroundColor = consoleInfo.BackgroundColor;

        var timestampts = s_configsService.GetConfig<TestConfigWithAttributesAndInterfaces>();
        Console.WriteLine($"Previous timestampts: " +
            $"{Environment.NewLine}previous start app time -> {timestampts.ApplicationStartTime.ToString("dd:hh:mm:ss")}"+
            $"{Environment.NewLine}previous stop app time -> {timestampts.ApplicationStopTime.ToString("dd:hh:mm:ss")}"
            );

        timestampts.ApplicationStartTime = DateTime.Now;

        var timeInfo = s_configsService.GetConfig<TestConfigWithRelativePathAndNameAttribute>();
        Console.WriteLine("Loading game assets...");
        await Task.Delay(timeInfo.LoadingAssetsDelay);
        Console.WriteLine($"Assets loaded in {timeInfo.LoadingAssetsDelay} ms.");

        Console.WriteLine($"Trying connect to server...");
        for (int i = 1; i <= timeInfo.AttempsCount; i++)
        {
            Console.WriteLine($"Trying to upload data on server, attempt {i}");
            await Task.Delay(timeInfo.LoadingAssetsDelay);
        }
        Console.WriteLine($"Data successfully uploaded on server!");

        var character = s_configsService.GetConfig<TestConfigWithoutAnyAttributesAndInterfaces>();
        Console.WriteLine($"Created character with name: {character.CharacterName} " +
            $"and HP: {new Random().Next(1, character.ChraracterMaxHP)}");
        timestampts.ApplicationStopTime = DateTime.Now;

        s_configsService.SaveConfigsToFiles();
    }
}