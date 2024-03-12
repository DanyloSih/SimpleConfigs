## 🚀 About Me
Hello, my - [linkedin](https://www.linkedin.com/in/danylo-sihaiev-33118a21b/)   

## Documentation

### Assemblies

&nbsp;&nbsp;&nbsp;&nbsp;  **SimpleConfigs** - The core library provides all necessary functional for configs creating, updating, destroying etc. Don't have any external dependencies. But it not contains implementation for `ISerializationManager` interface. This interface is responsible for converting config object data into a byte array that's will be written to a file. You can make realization for this interface by yourself or using existing assemble like: `SimpleConfigs.JSON`.

&nbsp;&nbsp;&nbsp;&nbsp;  **SimpleConfigs.JSON** - Contain implementation for `ISerializationManager` interface, which give ability to serialize config in JSON format. Depends on: `SimpleConfigs`, `Newtonsoft.JSON`.

&nbsp;&nbsp;&nbsp;&nbsp;  **SimpleConfigs.Test** - Contain tests for all other assemblies and have using examples. Depends on all other assemblies.

### Using
&nbsp;&nbsp;&nbsp;&nbsp; For example, you have class 
