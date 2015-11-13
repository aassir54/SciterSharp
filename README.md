ACTUAL SCITER VERSION: 3.3.0.6

**Sciter Bootstrap**: for quick starting your desktop app, [download](http://misoftware.rs/Bootstrap/Download) a package with a pre-made IDE project for the C# language, which comes with this library already configured.

##About

This library is a port of Sciter headers to the C# language. [Sciter](http://sciter.com/download/) is a multi-platform HTML engine. So with this library you can create D desktop application using not just HTML, but all the features of Sciter: CSS3, SVG, scripintg, AJAX, ... Sciter is free for commercial use.

Nuget package: https://www.nuget.org/packages/SciterSharp/

License: GNU GENERAL PUBLIC LICENSE Version 3

##Requirements

Requires at least .NET 4.5.

##Usage

In Visual Studio, install it via Nuget package or manually adding a reference to the assembly binary.

Then, for running you desktop app, you need to make sure that your program can find sciter32/64.dll. I recommend putting a copy of each DLL in the bin/Debug/ and bin/Release/ folders.