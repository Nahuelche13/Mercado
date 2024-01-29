using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

using Mercado.GNOME.Helpers;
using Mercado.Shared.Controllers;
using Mercado.Shared.Models;

namespace Mercado.GNOME.Views;

public partial class MainWindow : Adw.ApplicationWindow {
    private readonly MainWindowController _controller;
    private readonly Adw.Application _application;

#pragma warning disable CS0649
    [Gtk.Connect] private readonly Gtk.Button _createProductButton;
    [Gtk.Connect] private readonly Gtk.Button _startManageButton;
    [Gtk.Connect] private readonly Adw.Bin _sellPage;
    [Gtk.Connect] private readonly Adw.Bin _managePage;
#pragma warning restore CS0649

    private MainWindow(Gtk.Builder builder, MainWindowController controller, Adw.Application application) : base(builder.GetPointer("_root"), false) {
        //Window Settings
        _controller = controller;
        _application = application;

        SetTitle(AppInfo.ShortName);
        SetIconName(AppInfo.ID);
        if (AppInfo.IsDevVersion) {
            AddCssClass("devel");
        }
        //Build UI
        builder.Connect(this);

        #region Register Events 
        //Register Events 
        OnCloseRequest += OnCloseRequested;
        //Preferences Action
        // Gio.SimpleAction actPreferences = Gio.SimpleAction.New("preferences", null);
        // actPreferences.OnActivate += Preferences;
        // AddAction(actPreferences);
        // application.SetAccelsForAction("win.preferences", ["<Ctrl>comma"]);
        //Keyboard Shortcuts Action
        Gio.SimpleAction actKeyboardShortcuts = Gio.SimpleAction.New("keyboardShortcuts", null);
        actKeyboardShortcuts.OnActivate += KeyboardShortcuts;
        AddAction(actKeyboardShortcuts);
        application.SetAccelsForAction("win.keyboardShortcuts", ["<Ctrl>question"]);
        //Quit Action
        Gio.SimpleAction actQuit = Gio.SimpleAction.New("quit", null);
        actQuit.OnActivate += Quit;
        AddAction(actQuit);
        application.SetAccelsForAction("win.quit", ["<Ctrl>q"]);
        //About Action
        Gio.SimpleAction actAbout = Gio.SimpleAction.New("about", null);
        actAbout.OnActivate += About;
        AddAction(actAbout);
        application.SetAccelsForAction("win.about", ["F1"]);
        #endregion

        SellProductsViewController sellProductsViewController = new();
        _sellPage.SetChild(new SellProductsView(sellProductsViewController, this));

        ManageProductsViewController manageProductsViewController = new();
        _startManageButton.OnClicked += (sender, e) => {
            _managePage.SetChild(new ManageProductsView(manageProductsViewController, this));
            _managePage.OnShow += (sender, e) => manageProductsViewController.ReloadPage(this, null);
        };

        _createProductButton.OnClicked += (sender, e) => {
            CreateProductDialogController createProductController = new();
            CreateProductDialog createProductDialog = new(createProductController, this);
            createProductDialog.Present();

            createProductDialog.OnApply += async (s, ex) => {
                (int rows, int error) = await Product.Create(createProductController.Product);

                Adw.MessageDialog dialog = SQLErrorCodes.FillDialogSQLErrorCodes(this, error, "Producto creado");
                dialog.Present();

                manageProductsViewController.ReloadPage(this, null);
                createProductDialog.Destroy();
            };
        };
    }

    public MainWindow(MainWindowController controller, Adw.Application application) : this(Builder.FromFile("window.ui"), controller, application) { }

    public void Start() {
        _application.AddWindow(this);
        Present();
    }

    private bool OnCloseRequested(Gtk.Window sender, EventArgs e) {
        _controller.Dispose();
        return false;
    }

    private void Quit(Gio.SimpleAction sender, EventArgs e) {
        if (!OnCloseRequested(this, EventArgs.Empty)) {
            _application.Quit();
        }
    }

    private void About(Gio.SimpleAction sender, EventArgs e) {
        StringBuilder debugInfo = new();
        debugInfo.AppendLine(AppInfo.ID);
        debugInfo.AppendLine(AppInfo.Version);
        debugInfo.AppendLine($"GTK {Gtk.Functions.GetMajorVersion()}.{Gtk.Functions.GetMinorVersion()}.{Gtk.Functions.GetMicroVersion()}");
        debugInfo.AppendLine($"libadwaita {Adw.Functions.GetMajorVersion()}.{Adw.Functions.GetMinorVersion()}.{Adw.Functions.GetMicroVersion()}");
        if (File.Exists("/.flatpak-info")) {
            debugInfo.AppendLine("Flatpak");
        }
        else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SNAP"))) {
            debugInfo.AppendLine("Snap");
        }
        debugInfo.AppendLine(CultureInfo.CurrentCulture.ToString());
        Process localeProcess = new() {
            StartInfo = new ProcessStartInfo {
                FileName = "locale",
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };
        try {
            localeProcess.Start();
            string localeString = localeProcess.StandardOutput.ReadToEnd().Trim();
            localeProcess.WaitForExit();
            debugInfo.AppendLine(localeString);
        }
        catch {
            debugInfo.AppendLine("Unknown locale");
        }
        Adw.AboutWindow dialog = Adw.AboutWindow.New();
        dialog.SetTransientFor(this);
        dialog.SetIconName(AppInfo.ID);
        dialog.SetApplicationName(AppInfo.ShortName);
        dialog.SetApplicationIcon(AppInfo.ID + (AppInfo.IsDevVersion ? "-devel" : ""));
        dialog.SetVersion(AppInfo.Version);
        dialog.SetDebugInfo(debugInfo.ToString());
        dialog.SetComments(AppInfo.Description);
        dialog.SetDeveloperName("Nahuelche13");
        dialog.SetLicenseType(Gtk.License.Gpl30);
        dialog.SetCopyright("© Nahuelche13 2024-2024");
        // dialog.SetWebsite("https://");
        dialog.SetIssueUrl(AppInfo.IssueTracker.ToString());
        dialog.SetSupportUrl(AppInfo.SupportUrl.ToString());
        dialog.AddLink("GitHub Repo", AppInfo.SourceRepo.ToString());
        foreach (System.Collections.Generic.KeyValuePair<string, Uri> pair in AppInfo.ExtraLinks) {
            dialog.AddLink(pair.Key, pair.Value.ToString());
        }
        dialog.SetDevelopers(AppInfo.ConvertURLDictToArray(AppInfo.Developers));
        dialog.SetDesigners(AppInfo.ConvertURLDictToArray(AppInfo.Designers));
        dialog.SetArtists(AppInfo.ConvertURLDictToArray(AppInfo.Artists));
        dialog.SetTranslatorCredits(AppInfo.TranslatorCredits);
        dialog.Present();
    }

    private void KeyboardShortcuts(Gio.SimpleAction sender, EventArgs e) {
        Gtk.Builder builder = Builder.FromFile("shortcuts_dialog.ui");
        Gtk.ShortcutsWindow shortcutsWindow = (Gtk.ShortcutsWindow)builder.GetObject("_shortcuts")!;
        shortcutsWindow.SetTransientFor(this);
        shortcutsWindow.SetIconName(AppInfo.ID);
        shortcutsWindow.Present();
    }
}