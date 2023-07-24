using System.Diagnostics;

namespace Credfeto.Explorer.Server.Models;

[DebuggerDisplay("RequestId: {RequestId}")]
public readonly record struct ErrorViewModel(string? RequestId)
{
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}