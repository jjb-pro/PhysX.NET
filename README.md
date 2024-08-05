# PhysX.NET

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

1. **Download**: Download the latest version from the [Releases](https://github.com/jjb-pro/PhysX.NET/releases/) page (there is no NuGet package). 
2. Ensure the following DLLs are in the same directory as your executable:
   - `PhysX_64.dll`
   - `PhysXCommon_64.dll`
   - `PhysXCooking_64.dll`
   - `PhysXFoundation_64.dll`
   - `Ijwhost.dll`
3. **Add Reference**: Add a reference to the `PhysX.NET` assembly in your .NET project.

4. **Initialize PhysX**: Follow the example provided in the repository to set up and initialize PhysX in your project.

## Example

A sample implementation is available in the **Samples** directory of the repository. This example demonstrates how to use the wrapper and its features.

## Development

### Feature Requests

Feature requests are welcome! If you have specific needs or suggestions, please open an issue on the [GitHub Issues page](https://github.com/jjb-pro/PhysX.NET/issues) to discuss your ideas.