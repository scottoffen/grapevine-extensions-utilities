using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Grapevine;

/// <summary>
/// Provides methods for launching the default web browser.
/// </summary>
public static class BrowserLauncher
{
    public static Action<string>? Logger { get; set; }

    /// <summary>
    /// Opens the specified URL in the default web browser.
    /// </summary>
    /// <param name="url">The URL to open. If missing a scheme, "http://" is assumed.</param>
    /// <exception cref="ArgumentException">Thrown when the URL is null, empty, or whitespace.</exception>
    /// <exception cref="UriFormatException">Thrown if the constructed Uri is invalid.</exception>
    public static void OpenUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        // Ensure the URL starts with a valid scheme (http or https)
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = "http://" + url;
        }

        OpenUrl(new Uri(url));
    }

    /// <summary>
    /// Opens the specified URL in the default web browser.
    /// </summary>
    /// <param name="url">The fully formed <see cref="Uri"/> to open in the default browser.</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if the OS is not Windows, Linux, or macOS.</exception>
    /// <exception cref="System.ComponentModel.Win32Exception">Thrown when the process start fails on supported platforms.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no file name is specified on Windows.</exception>
    public static void OpenUrl(Uri url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url.ToString(),
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url.ToString());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url.ToString());
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported operating system");
            }

            Logger?.Invoke($"Opening {url} in the default browser...");
        }
        catch (Exception ex)
        {
            Logger?.Invoke($"An error occurred while opening the browser: {ex.Message}");
            throw;
        }
    }
}
