# Grapevine.Extensions.Utilities

A lightweight utility library for .NET that provides common functionality used when developing embedded HTTP servers or developer tools.

## Features

### PortFinder

Easily locate an available TCP port on the local machine - useful for tests, diagnostics, and dynamic service startup.

#### Find an open port

```csharp
int port = PortFinder.FindNextLocalOpenPort();
```

#### Try a port range:

```csharp
if (PortFinder.TryFindAvailablePort(out var port, 3000, 4000))
{
    ...
}
```

### BrowserLauncher

Open a local or remote URL in the default system browser - cross-platform support for Windows, macOS, and Linux.

#### Launch a URL with optional logging:

```csharp
BrowserLauncher.Logger = Console.WriteLine;
BrowserLauncher.OpenUrl("http://localhost:5000");
```

## Compatibility
- ✅ .NET Standard 2.0
- ✅ Works with .NET Framework, .NET Core, .NET 5+, Mono
- ✅ Supports Windows, Linux, and macOS