using System;

namespace ExpPanelRenderer.Common;

public class CustomEventArgs : EventArgs
{
    public object? CurrentDataContext { get; }

    public CustomEventArgs(object? currentDataContext)
    {
        CurrentDataContext = currentDataContext;
    }
}