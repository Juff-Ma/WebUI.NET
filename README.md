<div align="center">

![Logo](https://github.com/Juff-Ma/WebUI.NET/blob/main/assets/webui_net.png)

# WebUI.NET

[last-commit]: https://img.shields.io/github/last-commit/Juff-Ma/WebUI.NET?style=for-the-badge&logo=github
[release-version]: https://img.shields.io/github/v/release/Juff-Ma/WebUI.NET?sort=date&display_name=tag&style=for-the-badge
[license]: https://img.shields.io/github/license/Juff-Ma/WebUI.NET?style=for-the-badge&label=license

[![][last-commit]](https://github.com/Juff-Ma/WebUI.NET/pulse)
[![][release-version]](https://github.com/Juff-Ma/WebUI.NET/releases/latest)
[![][license]](https://github.com/Juff-Ma/WebUI.NET/blob/main/LICENSE)

[download]: https://img.shields.io/badge/download-004880?style=flat-square&logo=nuget
[download-debug]: https://img.shields.io/badge/debug_build-004880?style=flat-square&logo=github

[![][download]](https://www.nuget.org/packages/WebUI.NET/)
[![][download-debug]](https://github.com/Juff-Ma/WebUI.NET/pkgs/nuget/WebUI.NET)

</div>

WebUI is a minimal GUI library that uses a Browser already installed instead of bundling a large browser engine or requiring support for a native WebView.

This library contains high level bindings and precompiled binaries for the native WebUI library.

### Features
- Portable (*Needs only a web browser and .NET at runtime*)
- Fast binary communication protocol
- Multi-platform & Multi-Browser
- Using private profile for safety

### Screenshots
If you want to see whats possible here are screenshots of the text_editor example (originally ported from the C version) on Windows, MacOS and Linux (Gnome)
<p align="middle">
  <img src="https://github.com/Juff-Ma/WebUI.NET/blob/main/assets/Windows-Screenshot.png" width="33%" />
  <img src="https://github.com/Juff-Ma/WebUI.NET/blob/main/assets/MacOS-Screenshot.png" width="33%" /> 
  <img src="https://github.com/Juff-Ma/WebUI.NET/blob/main/assets/Linux-Screenshot.png" width="33%" /> 
</p>

### More Info
If you are not yet convinced or need more informations about WebUI take a look at it's [homepage](https://webui.me) or it's [repository](https://github.com/webui-dev/webui/)

## Contents
- [Features](#webuinet)
- [Installation](#installation)
- [Usage](#usage)
- [Building](#building)
- [Licensing](#licensing)

## Installation
You can install WebUI.NET using nuget by using the nuget explorer or the following command: 
```bash
dotnet add package WebUI.NET
```
In addition, to use prebuild versions of WebUI you'll need to download WebUI.NET.Natives
```bash
dotnet add package WebUI.NET.Natives
```
or the secure natives (OpenSSL support)
```bash
dotnet add package WebUI.NET.Natives.Secure
```
### Debug builds
The debug builds of WebUI.NET are provided using github packages and contain debug builds of the library and the underlying natives. WebUI will ONLY provide debug logging on such builds. If you did install the nuget.org variant first you might need to clear your nuget cache and bin/obj directories in order for nuget to download and use those builds.

## Usage
A basic example (.NET 7.0+) of how to open a Window and display a basic message (taken from *basic_window* example)
```csharp
﻿using WebUI;

Window window = new();
window.Show("""
<html>
    <script src="webui.js"></script> 
    <head>
        <title>WebUI</title>
    </head>
    <body>
        <h1>WebUI</h1>
        <p>It works!</p>
    </body>
</html>
""");

Utils.WaitForExit();
```
For more examples you can look at the [examples](https://github.com/Juff-Ma/WebUI.NET/tree/main/examples) directory.

### Deployment Options
If you finished writing your app and now want to ship it to your users you have multiple options, of course you can package the app in whatever way you want but here are some tested recommendations. 
For Windows and Linux deployment take a look at [PupNet-Deploy](https://github.com/kuiperzone/PupNet-Deploy) and for MacOS you can write a little startup bash script and then use [Platypus](https://sveinbjorn.org/platypus) and [create-dmg](https://github.com/create-dmg/create-dmg) to build the package.

## Building
You can build WebUI.NET itself like every other .NET project by using ```dotnet build```. When buildung the .nupkg and .snupkg are put into the repective configuration's directory inside the nupkgs toplevel directory.


In contrast the examples require the library and natives provided by _WebUI.NET_ and _WebUI.NET.Natives_. Because of a limitation of .NET natives can only be provided by a nuget package and not a package reference. The examples take the nuget binaries from the nupkgs directory, this leads to some problems while development (like requiring to clear the nuget cache every now and then). For you this will mean that if you want to build the examples or the full solution you will need to build the _WebUI.NET_ and _WebUI.NET.Natives_ packages at least once so they provide those packages locally.

## Licensing
WebUI.NET is licensed under the [MPL-2.0](https://github.com/Juff-Ma/WebUI.NET/blob/main/LICENSE) license, more information about dos and don'ts can be found on the Mozilla MPL [FAQ](https://www.mozilla.org/en-US/MPL/2.0/FAQ/) but in short you can use this library in binary form in proprietary and open-source projects as long as you keep the copyright and tell people where to download the library. If you want to include the library in source code form you will need to keep every file that contains MPL code under the MPL. Notable exception are the LGPL2.1+ GPL2+ or AGPL3+ licenses that the MPL also allows you to use. **DISCLAIMER: I'm not a lawyer and this is not legal advice, if you want more specific info please read the FAQ and the license**<br/>
The examples are NOT part of the main project and can be used under the [CC0](https://creativecommons.org/public-domain/cc0/) license for that matter as long as you respect the license of this library and the other libraries used by this project.

WebUI and its logo (on which the WebUI.NET logo is based) are licensed under the [MIT](https://github.com/webui-dev/webui/blob/main/LICENSE) license.

All libraries used in this projects are under Copyright (C) of their respective owners. You can find more information on the copyright notice of the _WebUI.NET.Natives_ and _WebUI.NET.Natives.Secure_ nuget packages.

### Alternatives
If you want a library that is not licensed under MPL but rather MIT and has similar functionality consider checking out [WebUI4CSharp](https://github.com/salvadordf/WebUI4CSharp)
