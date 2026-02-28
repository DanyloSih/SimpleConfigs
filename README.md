# SimpleConfigs

A lightweight library for creating, loading, and saving application configuration files with support for multiple serialization formats via the `ISerializationManager` interface.

**NuGet:** https://www.nuget.org/packages/SimpleConfigs  
**Target framework:** `.NET 8`

---

## Installation

Install the core package:

```bash
dotnet add package SimpleConfigs
```

For JSON support:

```bash
dotnet add package SimpleConfigs.JSON
```

---

## Project Structure

### SimpleConfigs
Core library that provides functionality for creating, saving, and loading configs.  
Has no external dependencies.  
Does not include an implementation of the `ISerializationManager` interface.

### SimpleConfigs.JSON
Provides a JSON implementation of `ISerializationManager`.  
Dependencies: `SimpleConfigs`, `Newtonsoft.Json`.

### SimpleConfigs.Examples
Contains usage examples.  
Depends on all project assemblies except `SimpleConfigs.Test`.

### SimpleConfigs.Test
Contains tests for all assemblies.  
Depends on all assemblies except `SimpleConfigs.Examples`.

---

## Usage

Example configuration class:

```csharp
public class MainConfig
{
	public string SomeUrl = "http://www.example.com/";
	public int SomeValue = 456;
}
```

To save this config to a file or populate the object from a file, use `ConfigsService`.  
It requires an `ISerializationManager` as the first parameter.

You can implement the interface yourself or use an existing implementation, such as `SimpleConfigs.JSON`.

### Config Registration

`ConfigsService` can register one or multiple config types (passed as `Type` arguments).

```csharp
var configsService = new ConfigsService(
	new JsonSerializationManager(), // ISerializationManager
	new LocalFileSystem(),          // IFileSystem
	typeof(MainConfig),             // registered config type
	typeof(SomeOtherConfig)         // another registered config type
);

// Creates missing config files
// and loads data from existing files
await configsService.InitializeAllConfigsAsync();
```

### Working with Configs

```csharp
var mainConfigInstance = configsService.GetConfig<MainConfig>();
Console.WriteLine($"{mainConfigInstance.SomeValue}"); // 456

mainConfigInstance.SomeValue = 15;

await configsService.SaveConfigToFileAsync<MainConfig>();
```

---

## File Path and Name Configuration

### Relative Path

If not specified, the default folder is the directory of the running assembly.

```csharp
[RelativePath("Configs/Main")]
public class MainConfig
{
}
```

File path:  
`<application folder>/Configs/Main/MainConfig.cfg`

If the attribute is not specified, the file will be created in the running assembly directory with the default name `MainConfig.cfg`.

---

### File Name

Default: `TypeName.cfg`

```csharp
[RelativePath("Configs/Main"), ConfigName("MyConfigName.myFileExtension")]
public class MainConfig
{
}
```

File path:  
`<application folder>/Configs/Main/MyConfigName.myFileExtension`

---

### Programmatic Path Override

```csharp
configsService.SetPathOverrideSettings<MainConfig>(
	new PathSettings("Configs/Overrided", "MyConfigName.myFileExtension"));

// Reset to default
configsService.SetPathOverrideSettings<MainConfig>(
	new PathSettings(null, ""));
```

---

## General Information

### ISerializationManager

Responsible for converting a config object into a byte array to be written to a file.

The implementation is not included in `SimpleConfigs` to avoid unnecessary dependencies.  
You can implement it yourself or use an existing one, such as `SimpleConfigs.JSON`.

