# Grapevine.Extensions.Utilities

This library supports Grapevine and other embedded HTTP services by simplifying port allocation and browser launching across platforms.

## Compatibility

![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-blue)

- ✅ .NET Standard 2.0
- ✅ Works with .NET Framework, .NET Core, .NET 5+, Mono
- ✅ Supports Windows, Linux, and macOS

## Installation

You can install via NuGet:

```bash
dotnet add package Grapevine.Extensions.Utilities
```

## PortFinder

The PortFinder class helps you find available TCP ports for running servers, tests, or local tools. It scans efficiently and verifies availability with a socket bind.

### Constants

| Name               | Description                                   |
| ------------------ | --------------------------------------------- |
| `FirstPort`        | 1 (lowest valid port)                         |
| `LastPort`         | 65535 (highest valid port)                    |
| `FirstServicePort` | 1024 (start of IANA-registered service range) |
| `LastServicePort`  | 49151 (end of IANA-registered service range)  |


### Methods

|  Signature                                                                                    | Description                                                                                                 |
| --------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| `int FindNextLocalOpenPort()`                                                                 | Finds the first available TCP port between `1024` and `49151`, scanning in ascending order.                 |
| `int FindNextLocalOpenPort(int startIndex)`                                                   | Finds the first available TCP port from `startIndex` up to `49151`, scanning in ascending order.            |
| `int FindLastLocalOpenPort()`                                                                 | Finds the first available TCP port between `1024` and `49151`, scanning in descending order.                |
| `int FindLastLocalOpenPort(int endIndex)`                                                     | Finds the first available TCP port from `49151` down to `endIndex`, scanning in descending order.           |
| `bool TryFindAvailablePort(out int port, int startIndex, int endIndex, bool reverse = false)` | Attempts to find an available TCP port within a specified range and direction. Returns `true` if found.     |
| `int? FindAvailablePort(int startIndex, int endIndex, bool reverse = false)`                  | Returns the first available TCP port within the given range and direction, or `null` if none are available. |

### Code Samples

Find the next available TCP port within the standard service range:

```csharp
int port = PortFinder.FindNextLocalOpenPort();
Console.WriteLine($"Found port: {port}");
```

Find the next available TCP port starting at a specific index:

```csharp
int port = PortFinder.FindNextLocalOpenPort(3000);
Console.WriteLine($"Found port starting from 3000: {port}");
```

Find the last available TCP port within the standard service range, scanning in reverse:

```csharp
int port = PortFinder.FindLastLocalOpenPort();
Console.WriteLine($"Found last open port: {port}");
```

Find the last available TCP port ending at a specific index:

```csharp
int port = PortFinder.FindLastLocalOpenPort(5000);
Console.WriteLine($"Found last open port down to 5000: {port}");
```

Try to find an available port within a specific range (ascending order):

```csharp
if (PortFinder.TryFindAvailablePort(out int port, 4000, 4100))
{
    Console.WriteLine($"Available port: {port}");
}
else
{
    Console.WriteLine("No available port found in the range.");
}
```

Try to find an available port within a specific range (descending order):

```csharp
if (PortFinder.TryFindAvailablePort(out int port, 4000, 4100, reverse: true))
{
    Console.WriteLine($"Available port (scanning in reverse): {port}");
}
```

Find an available port using a nullable return value (ascending order):

```csharp
int? port = PortFinder.FindAvailablePort(8000, 8100);

if (port.HasValue)
{
    Console.WriteLine($"Found open port: {port.Value}");
}
else
{
    Console.WriteLine("No available port found.");
}
```

Find an available port using a nullable return value (descending order):

```csharp
int? port = PortFinder.FindAvailablePort(8000, 8100, reverse: true);

if (port.HasValue)
{
    Console.WriteLine($"Found open port (reverse): {port.Value}");
}
```

## BrowserLauncher

### Methods

| Signature                  | Description                                                                                              |
| -------------------------- | -------------------------------------------------------------------------------------------------------- |
| `void OpenUrl(string url)` | Opens a URL in the system browser. Prepends `http://` if missing. May throw on unsupported platforms.    |
| `void OpenUrl(Uri url)`    | Opens a Uri in the system browser. May throw on unsupported platforms or if the browser fails to launch. |

### Code Samples

Open a URL string in the default browser
```csharp
BrowserLauncher.OpenUrl("localhost:5000");
```

> [!TIP]
> If the string does not start with `http://` or `https://`, it will default to `http://`.

Open a Uri in the default browser
```csharp
var uri = new Uri("https://example.com");
BrowserLauncher.OpenUrl(uri);
```

Set a logger callback for status messages or errors
```csharp
BrowserLauncher.Logger = Console.WriteLine;
BrowserLauncher.OpenUrl("http://localhost:3000");
```
> [!TIP]
> The logger will receive messages such as "Opening http://localhost:3000 in the default browser..." or error details if the launch fails.