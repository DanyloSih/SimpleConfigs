using SimpleConfigs.Core;
using SimpleConfigs.Test.Configs;
using SimpleConfigs.JSON.SerializationManagers;

internal class Program
{
    private static ConfigsService? s_configsService;

    private static async Task Main(string[] args)
    {
        s_configsService = new ConfigsService(
            new JsonSerializationManager(),
            new LocalFileSystem(),
            typeof(TestConfigWithoutAnyAttributesAndInterfaces),
            typeof(TestConfigWithRelativePathAttribute),
            typeof(TestConfigWithRelativePathAndNameAttribute),
            typeof(TestConfigWithAttributesAndInterfaces));

        await s_configsService.InitializeConfigsAsync();

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

        await s_configsService.SaveConfigsToFilesAsync();

        var configsService2 = new ConfigsService(
            new JsonSerializationManager(),
            new LocalFileSystem(),
            (typeof(TestConfigWithoutAnyAttributesAndInterfaces), new PathSettings(null, "nameOverride 1.txt")),
            (typeof(TestConfigWithRelativePathAttribute), new PathSettings(null, null)),
            (typeof(TestConfigWithRelativePathAndNameAttribute), new PathSettings("PathOverride 1", null)),
            (typeof(TestConfigWithAttributesAndInterfaces), new PathSettings("PathOverride 2", "nameOverride 2.cfg")));

        configsService2.SetPathOverrideSettings<TestConfigWithoutAnyAttributesAndInterfaces>(new PathSettings(null, "overridedName.json"));

        configsService2.CommonRelativeDirectoryPath = Path.Combine("ConfigsService", "2");
        await configsService2.InitializeConfigsAsync();
        await Console.Out.WriteLineAsync($"Created {nameof(configsService2)} config files!");
        await configsService2.SaveConfigsToFilesAsync();

        await Console.Out.WriteLineAsync($"Press any key to delete all config files from {nameof(configsService2)}");
        var key = Console.ReadKey();
        await configsService2.DeleteAllConfigFilesAsync();
    }
}