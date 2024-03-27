## 🚀 About Me
Hello, my - [linkedin](https://www.linkedin.com/in/danylo-sihaiev-33118a21b/)   

## Documentation

### Target framework

Releases only on: `.Net8`

### Assemblies

<div id="SimpleConfigs"></div>

**SimpleConfigs** - The core library provides all necessary functional for configs creating, saving, loading etc. Don't have any external dependencies. But it not contains implementation for [`ISerializationManager`](#ISerializationManager) interface.

<div id="SimpleConfigs.JSON"></div>

**SimpleConfigs.JSON** - Contain implementation for [`ISerializationManager`](#ISerializationManager) interface, which give ability to serialize config in JSON format.
Depends on: [`SimpleConfigs`](#SimpleConfigs), `Newtonsoft.JSON`.

<div id="SimpleConfigs.Examples"></div>

**SimpleConfigs.Examples** - Contain using examples.
Depends on all other assemblies in project exept [`SimpleConfigs.Test`](#SimpleConfigs.Test).

<div id="SimpleConfigs.Test"></div>

**SimpleConfigs.Test** - Contain tests for all other assemblies.
Depends on all other assemblies in project exept [`SimpleConfigs.Examples`](#SimpleConfigs.Examples).

### Using
For example, you have config class like this:

```C#
public class MainConfig
{
	public string SomeUrl = "http://www.example.com/";
	public int SomeValue = 456;
}
```
To save this config as file, or populate this object with data from file, you should use `ConfigsService` class. It takes [`ISerializationManager`](#ISerializationManager) as the first parameter. You can implement [`ISerializationManager`](#ISerializationManager) interface by yourself or use an existing assembly, for example we take [`SimpleConfigs.JSON`](#SimpleConfigs.JSON) assembly and his [`ISerializationManager`](#ISerializationManager) implementation `JsonSerializationManager`

Configs registration example:

```C#
// your function....

// registering config type must have at least one parameterless constructor!
var configsService = new ConfigsService(
	new JsonSerializationManager(), // ISerializationManager
	new LocalFileSystem(), // IFileSystem
	typeof(MainConfig) // registering config type
	);
	
// this method will create config files that not exist now
// and then load all existing config files data to configs instances.
await configsService.InitializeAllConfigsAsync(); 
```
```C#
// now you can get data from config file
var mainConfigInstance = configsService.GetConfig<MainConfig>();
Console.WriteLine($"{mainConfigInstance.SomeValue}"); // 456

// change it (this example work only if config type is referenced type)
mainConfigInstance.SomeValue = 15;

// and save to file
await configsService.SaveConfigToFileAsync<MainConfig>();
```
---

You can specify config file relative directory path via `RelativePathAttribute`.
If you don't specify an attribute for the config type, the default file folder will be the folder of your *running assembly*.
```C#
// config file will be generated in "running assembly folder"/Configs/Main
[RelativePath("Configs/Main")] 
public class MainConfig 
{
	// your fields 
}
```
Also you can specify config file name via `ConfigNameAttribute`.
If you don't specify it, the default file name will be: "Type name" + ".cfg"

```C#
// config file will have path:
// "running assembly folder"/Configs/Main/MyConfigName.myFileExtension
[RelativePath("Configs/Main"), ConfigName("MyConfigName.myFileExtension")]
public class MainConfig 
{
	// your fields 
}
```

And you can override all path data above using `ConfigsService.SetPathOverrideSettings` method.

```C#
// config file will have path:
// "running assembly folder"/Configs/Overrided/MyConfigName.myFileExtension
configsService.SetPathOverrideSettings<MainConfig>(
	new PathSettings("Configs/Overrided", "MyConfigName.myFileExtension"));

// config file will have path:
// "running assembly folder"/"default MainConfig filename"
configsService.SetPathOverrideSettings<MainConfig>(
	new PathSettings(null, ""));
```

### Common information

<div id="ISerializationManager"></div>

**`ISerializationManager`** - This interface is responsible for converting config object data into a byte array that's will be written to a file. The implementation of this interface is not included in the main [`SimpleConfigs`](#SimpleConfigs) assembly to avoid creating unnecessary dependencies. You can implement this interface by yourself or use an existing assembly, for example: [`SimpleConfigs.JSON`](#SimpleConfigs.JSON).
