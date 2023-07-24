using System;
using Microsoft.Extensions.Logging;

namespace Credfeto.Explorer.Ethereum.Startup.LoggingExtensions;

internal static partial class EthereumProxyStartupLoggingExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "There are {networkCount} networks enabled in proxy.")]
    public static partial void FoundNetworks(this ILogger<EthereumProxyStartup> logger, int networkCount);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Error getting networks from proxy: {message}")]
    public static partial void FailedToGetNetworks(this ILogger<EthereumProxyStartup> logger, string message, Exception exception);
}