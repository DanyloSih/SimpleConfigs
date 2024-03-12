## 🚀 About Me
Hello, my - [linkedin](https://www.linkedin.com/in/danylo-sihaiev-33118a21b/)   

## Documentation

* ### Target framework

&nbsp;&nbsp;&nbsp;&nbsp;  Releases only on: `.Net8`, but you can rebuild project on target framework what you want.

* ### Assemblies

<div id="SimpleConfigs"></div>&nbsp;&nbsp;&nbsp;&nbsp;  **SimpleConfigs** - The core library provides all necessary functional for configs creating, saving, loading etc. Don't have any external dependencies. But it not contains implementation for [`ISerializationManager`](#ISerializationManager) interface.

<div id="SimpleConfigs.JSON"></div>&nbsp;&nbsp;&nbsp;&nbsp;  **SimpleConfigs.JSON** - Contain implementation for [`ISerializationManager`](#ISerializationManager) interface, which give ability to serialize config in JSON format.
Depends on: `SimpleConfigs`, `Newtonsoft.JSON`.

<div id="SimpleConfigs.Test"></div>&nbsp;&nbsp;&nbsp;&nbsp;  **SimpleConfigs.Test** - Contain tests for all other assemblies and have using examples. 
Depends on all other assemblies in project.

* ### Using
For example, you have config class like this: <br>
```C#
public class MainConfig
{
		public string SomeUrl = "http://www.example.com/";
		public int SomeValue = 456;
}
```
To save this config as file, or populate this object with data from file, you should use `ConfigsService` class. It takes [`ISerializationManager`](#ISerializationManager) as the first parameter in the constructor and array of config types as second. You can implement [`ISerializationManager`](#ISerializationManager) interface by yourself or use an existing assembly, for example we take [`SimpleConfigs.JSON`](#SimpleConfigs.JSON) assembly and his [`ISerializationManager`](#ISerializationManager) implementation `JsonSerializationManager` <br>
Configs registration example: <br>

```C#
	// your function....
	
	// registering config type must have at least one parameterless constructor!
	var configsService = new ConfigsService(
			new JsonSerializationManager(), // ISerializationManager
			typeof(MainConfig) // registering config type
			);
			
	// this method will create config files that not exist now
	// and then load all existing config files data to configs instances.
	configsService.InitializeAllConfigs(); 
```
```C#
	// now you can get data from config file
	var mainConfigInstance = configsService.GetConfig<MainConfig>();
	Console.WriteLine($"{mainConfigInstance.SomeValue}"); // 456
	
	// change it (this example work only if config type is referenced type)
	mainConfigInstance.SomeValue = 15;
	
	// and save to file
	configsService.SaveConfigToFile<MainConfig>();
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

* ### Common information
<div id="ISerializationManager"></div>&nbsp;&nbsp;&nbsp;&nbsp; **`ISerializationManager`** - This interface is responsible for converting config object data into a byte array that's will be written to a file. The implementation of this interface is not included in the main [`SimpleConfigs`](#SimpleConfigs) assembly to avoid creating unnecessary dependencies. You can implement this interface by yourself or use an existing assembly, for example: [`SimpleConfigs.JSON`](#SimpleConfigs.JSON).
