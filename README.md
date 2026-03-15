# Age

Another Game Engine

## Vulkan SDK & .NET 10 AOT Installation Guide

This guide outlines the environment setup for Vulkan development and .NET 10 Native AOT toolchains, grouped by platform.

## 1. Windows Setup

Platform Installation (via winget)

Run these commands in an Administrator terminal to install the Vulkan SDK and .NET 10:

```cmd
winget install KhronosGroup.VulkanSDK
winget install Microsoft.DotNet.SDK.10
```

## 2. Linux Setup

Ubuntu / Debian

```sh
# Update and install Vulkan SDK components
sudo apt update
sudo apt install vulkan-tools libvulkan-dev vulkan-validationlayers-dev spirv-tools

# Install .NET 10 and Native AOT dependencies
sudo apt install dotnet-sdk-10.0 dotnet-sdk-aot-10.0
```

Fedora / RHEL

```sh
# Update and install Vulkan SDK components
sudo dnf install vulkan-tools vulkan-loader-devel vulkan-validation-layers-devel mesa-vulkan-drivers

# Install .NET 10 and Native AOT dependencies
sudo dnf install dotnet-sdk-10.0 dotnet-sdk-aot-10.0
sudo dnf groupinstall "Development Tools"
```

## Publishing

```*
dotnet publish -c Release -r <RID>
```

RIDs: win-x64, linux-x64.
