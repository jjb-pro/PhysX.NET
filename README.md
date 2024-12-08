![blue PhysX icon](https://github.com/jjb-pro/PhysX.NET/blob/main/PhysX.NET/assets/icon.png)
# PhysX.NET

[![NuGet](https://img.shields.io/nuget/v/PhysX4.NET.svg)](https://www.nuget.org/packages/PhysX4.NET)

PhysX.NET is a .NET wrapper for NVIDIA PhysX 4.1.2, implemented using C++/CLI. This project is a fork of [PhysX.Net](https://github.com/stilldesign/PhysX.Net) and includes a fully functional vehicle sample.

## Features

- **Vehicle Physics**: A fully functional vehicle simulation example. This wrapper is the only one that offers the option of implementing vehicle physics with C#. 
- **Scene Management**: Create and manage physics scenes.
- **Actor Management**: Handle actors, their properties, and physics interactions.
- **Material Properties**: Define and manage physical materials.
- **Raycasting and Queries**: Perform raycasts and queries.
- **Debugging Tools**: Visualize and debug physics interactions with the PhysX Visual Debugger.
- **Remaining Features**:
  - Attach/Detach shape from an actor
  - Serialization
  - Broad Phase
  - Deletion Listener

## Usage

To use PhysX.NET, follow these steps:

1. **Install via NuGet**: Add the package to your project using the .NET CLI:
   ```bash
   dotnet add package PhysX4.NET
   ```
   Or, search for `PhysX4.NET` in the NuGet Package Manager in Visual Studio.
2. **Initialize PhysX**: Follow the examples provided in the repository to set up and initialize PhysX in your project.

## Example

Sample implementations are available in the **Samples** directory of the repository. This examples demonstrate how to use the wrapper and its features.

## Development

### Feature Requests

Feature requests are welcome! If you have specific needs or suggestions, please open an issue on the [GitHub Issues page](https://github.com/jjb-pro/PhysX.NET/issues) to discuss your ideas.