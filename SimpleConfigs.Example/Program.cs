using SimpleConfigs.Core;
using SimpleConfigs.Example.Configs;
using SimpleConfigs.JSON.SerializationManagers;
using SimpleConfigs.Core.ConfigsServiceInterfaces;

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

        await s_configsService.InitializeAllConfigsAsync();

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

        await s_configsService.SaveAllConfigsToFilesAsync();

        var configsService2 = new ConfigsService(
            new JsonSerializationManager(),
            new LocalFileSystem(),
            (typeof(TestConfigWithoutAnyAttributesAndInterfaces), new PathSettings(null, "nameOverride 1.txt")),
            (typeof(TestConfigWithRelativePathAttribute), new PathSettings(null, null)),
            (typeof(TestConfigWithRelativePathAndNameAttribute), new PathSettings("PathOverride 1", null)),
            (typeof(TestConfigWithAttributesAndInterfaces), new PathSettings("PathOverride 2", "nameOverride 2.cfg")));

        configsService2.SetPathOverrideSettings<TestConfigWithoutAnyAttributesAndInterfaces>(new PathSettings(null, "overridedName.json"));

        configsService2.CommonRelativeDirectoryPath = Path.Combine("ConfigsService", "2");
        await configsService2.InitializeAllConfigsAsync();
        await Console.Out.WriteLineAsync($"Created {nameof(configsService2)} config files!");
        await configsService2.SaveAllConfigsToFilesAsync();

        await Console.Out.WriteLineAsync($"Press any key to delete all config files from {nameof(configsService2)}");
        var key = Console.ReadKey();
        await configsService2.DeleteAllConfigFilesAsync();

        var formater = new ConfigsServicesPathsFormater("ConfigServicesHub", "TestSubdir", "copy ({id}){ex}");
        ConfigsServicesHub configsServicesHub = new ConfigsServicesHub(
            new LocalFileSystem(), new JsonSerializationManager(), formater, 10);
        var hubType = typeof(TestConfigWithAttributesAndInterfaces);
        configsServicesHub.RegisterTypeForAll(hubType, "configs hub type.json");
        await configsServicesHub.InitializeTypeForAllAsync(hubType, inParallel: true);

        var instances = configsServicesHub.GetTypeInstances<TestConfigWithAttributesAndInterfaces>();

        await Console.Out.WriteLineAsync($"instances count: {instances.Count}");
        Random rand = new Random();
        instances.ForEach(x => x.JustInt = rand.Next(2, 1000000));
        await configsServicesHub.SaveTypeForAllAsync(hubType, inParallel: true);

        await Console.Out.WriteLineAsync($"Press any key to delete all config files from {nameof(configsServicesHub)}");
        key = Console.ReadKey();
        await configsServicesHub.DeleteTypeFileForAllAsync(hubType, inParallel: true);

        await Console.Out.WriteLineAsync($"Press any key to create new config files for {nameof(configsServicesHub)}");
        key = Console.ReadKey();
        
        await configsServicesHub.CreateTypeFileForAllAsync(hubType, inParallel: true);

        await Console.Out.WriteLineAsync($"Press any key to load configs data for {nameof(configsServicesHub)}");
        key = Console.ReadKey();

        try
        {
            await configsServicesHub.LoadTypeForAllAsync(hubType, checkDataCorrectness: true, inParallel: true);
        }
        catch (Exception ex)
        {
            //Console.WriteLine(ex.Message);
        }

        await configsServicesHub.CheckDataCorrectnessForAllAsync(hubType, inParallel: true);

        await Console.Out.WriteLineAsync($"Press any key to end programm.");
        key = Console.ReadKey();
    }
}