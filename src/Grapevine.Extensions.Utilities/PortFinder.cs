using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Grapevine;

/// <summary>
/// Provides utility methods for finding available TCP ports on the local machine.
/// </summary>
/// <remarks>
/// All returned ports are verified to be currently unbound. However, availability is not guaranteed
/// until the port is actually bound to a socket. This is known as a time-of-check to time-of-use (TOCTOU) condition.
/// </remarks>
public static class PortFinder
{
    internal static string OutOfRangeMsg { get; } = $"Value must be an integer between {FirstPort} and {LastPort}.";

    /// <summary>
    /// The lowest valid TCP port number (inclusive).
    /// </summary>
    public const int FirstPort = 1;

    /// <summary>
    /// The highest valid TCP port number (inclusive).
    /// </summary>
    public const int LastPort = 65535;

    /// <summary>
    /// The first port number in the IANA-registered service port range (inclusive).
    /// </summary>
    public const int FirstServicePort = 1024;

    /// <summary>
    /// The last port number in the IANA-registered service port range (inclusive).
    /// </summary>
    public const int LastServicePort = 49151;

    /// <summary>
    /// Finds the first available TCP port within the typical service port range (1024–49151), scanning in ascending order.
    /// </summary>
    /// <returns>The available port number, or -1 if none is found.</returns>
    public static int FindNextLocalOpenPort() => FindAvailablePort(FirstServicePort, LastServicePort) ?? -1;

    /// <summary>
    /// Finds the first available TCP port starting at the specified index and scanning up to 49151.
    /// </summary>
    /// <param name="startIndex">The port number at which to begin the search.</param>
    /// <returns>The available port number, or -1 if none is found.</returns>
    public static int FindNextLocalOpenPort(int startIndex) => FindAvailablePort(startIndex, LastServicePort) ?? -1;

    /// <summary>
    /// Finds the first available TCP port within the typical service port range (1024–49151), scanning in descending order.
    /// </summary>
    /// <returns>The available port number, or -1 if none is found.</returns>
    public static int FindLastLocalOpenPort() => FindAvailablePort(FirstServicePort, LastServicePort, true) ?? -1;

    /// <summary>
    /// Finds the first available TCP port scanning in descending order from 49151 to the specified end index.
    /// </summary>
    /// <param name="endIndex">The port number at which to end the search.</param>
    /// <returns>The available port number, or -1 if none is found.</returns>
    public static int FindLastLocalOpenPort(int endIndex) => FindAvailablePort(FirstServicePort, endIndex, true) ?? -1;

    /// <summary>
    /// Attempts to find an available TCP port within the specified range.
    /// </summary>
    /// /// <param name="port">When this method returns, contains the available port number if found; otherwise, -1.</param>
    /// <param name="startIndex">The starting port number for the search (inclusive).</param>
    /// <param name="endIndex">The ending port number for the search (inclusive).</param>
    /// <param name="reverse">If true, scans in descending order; otherwise, scans ascending.</param>
    /// <returns>True if an available port is found; otherwise, false.</returns>
    public static bool TryFindAvailablePort(out int port, int startIndex = FirstServicePort, int endIndex = LastServicePort, bool reverse = false)
    {
        port = FindAvailablePort(startIndex, endIndex, reverse) ?? -1;
        return port.IsValidPortNumber();
    }

    /// <summary>
    /// Finds an available TCP port in the specified range.
    /// </summary>
    /// <param name="startIndex">The starting port number (inclusive).</param>
    /// <param name="endIndex">The ending port number (inclusive).</param>
    /// <param name="reverse">If true, scans in descending order; otherwise, scans ascending.</param>
    /// <returns>The available port number, or null if none are found.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if either startIndex or endIndex is outside the valid port range (1–65535).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if startIndex is greater than endIndex.
    /// </exception>
    public static int? FindAvailablePort(int startIndex, int endIndex, bool reverse = false)
    {
        if (!startIndex.IsValidPortNumber())
            throw new ArgumentOutOfRangeException(nameof(startIndex), OutOfRangeMsg);

        if (!endIndex.IsValidPortNumber())
            throw new ArgumentOutOfRangeException(nameof(endIndex), OutOfRangeMsg);

        if (startIndex > endIndex)
            throw new ArgumentException($"Start index {startIndex} must be less than or equal to end index {endIndex}.");

        var inUsePorts = GetUsedPorts();
        var step = reverse ? -1 : 1;
        var start = reverse ? endIndex : startIndex;
        var end = reverse ? startIndex : endIndex;

        for (var port = start; reverse ? port >= end : port <= end; port += step)
        {
            if (!inUsePorts.Contains(port) && port.IsPortAvailable())
                return port;
        }

        return null;
    }

    /// <summary>
    /// Gets the set of TCP and UDP ports currently in use on the local machine.
    /// This snapshot may not reflect real-time availability and should be used only as a pre-check.
    /// </summary>
    internal static HashSet<int> GetUsedPorts()
    {
        var props = IPGlobalProperties.GetIPGlobalProperties();
        var ports = new HashSet<int>();

        ports.UnionWith(props.GetActiveTcpListeners().Select(p => p.Port));
        ports.UnionWith(props.GetActiveTcpConnections().Select(c => c.LocalEndPoint.Port));
        ports.UnionWith(props.GetActiveUdpListeners().Select(p => p.Port));

        return ports;
    }

    /// <summary>
    /// Determines whether a TCP port is currently available for binding.
    /// </summary>
    /// <param name="port">The port number to check.</param>
    /// <returns>True if the port is available; otherwise, false.</returns>
    internal static bool IsPortAvailable(this int port)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Loopback, port)
            {
                ExclusiveAddressUse = true
            };

            listener.Start();
            listener.Stop();

            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the specified integer is a valid TCP port number (1–65535).
    /// </summary>
    /// <param name="value">The port number to validate.</param>
    /// <returns>True if the port number is valid; otherwise, false.</returns>
    internal static bool IsValidPortNumber(this int value) => (value >= FirstPort && value <= LastPort);
}
