using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Credfeto.Explorer.Ethereum.Services.LoggingExtensions;

internal static partial class NetworksLoggingExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Refreshing networks - last update: {elapsedTime} seconds ago")]
    [Conditional("DEBUG")]
    public static partial void RefreshingNetworks(this ILogger<Networks> logger, double elapsedTime);
}