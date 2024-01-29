using Mercado.GNOME.Views;
using Mercado.Shared.Controllers;

namespace Mercado.GNOME;

public partial class Program {
    private readonly Adw.Application _application;
    private MainWindow? _mainWindow = null;
    private readonly MainWindowController _mainWindowController;

    public static int Main() => new Program().Run();

    public Program() {
        _application = Adw.Application.New("uy.cyt.mercado", Gio.ApplicationFlags.NonUnique);
        _mainWindowController = new MainWindowController();
        _application.OnActivate += OnActivate;
    }

    public int Run() {
        try {
            return _application.RunWithSynchronizationContext([]);
        }
        catch (System.Exception ex) {
            System.Console.WriteLine(ex.Message);
            System.Console.WriteLine($"\n\n{ex.StackTrace}");
            return -1;
        }
    }

    private void OnActivate(Gio.Application sedner, System.EventArgs e) {
        //Main Window
        _mainWindow = new MainWindow(_mainWindowController, _application);
        _mainWindow.Start();
    }
}