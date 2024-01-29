using System;

namespace Mercado.Shared.Controllers;

public class MainWindowController() : IDisposable {
    private bool _disposed = false;

    ~MainWindowController() => Dispose(false);

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }
        _disposed = true;
    }
}