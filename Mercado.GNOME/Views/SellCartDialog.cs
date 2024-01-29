using System;

using Mercado.GNOME.Helpers;
using Mercado.Shared.Controllers;

namespace Mercado.GNOME.Views;

public partial class SellCartDialog : Adw.PreferencesWindow {
    private readonly SellCartDialogController _controller;

#pragma warning disable CS0649
    [Gtk.Connect] private readonly Adw.EntryRow _payEntry;
    [Gtk.Connect] private readonly Gtk.Label _returnLabel;
    [Gtk.Connect] private readonly Gtk.Label _priceLabel;
    [Gtk.Connect] private readonly Gtk.Button _deleteButton;
    [Gtk.Connect] private readonly Gtk.Button _applyButton;
#pragma warning restore CS0649

    // public event EventHandler? OnApply;
    public event EventHandler? OnDelete;

    private SellCartDialog(Gtk.Builder builder, SellCartDialogController controller, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        //Window Settings
        _controller = controller;
        SetTransientFor(parent);
        //Build UI
        builder.Connect(this);

        _priceLabel.SetText(_controller.TotalAmount.ToString());
        //_payEntry.SetText(_controller.Product.Name);
        _returnLabel.SetText("0");

        _payEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") {
                _returnLabel.SetText((_controller.TotalAmount - decimal.Parse(_payEntry.GetText())).ToString());
            }
        };

        _applyButton.OnClicked += (sender, e) => {
            Close();
            OnDelete?.Invoke(this, EventArgs.Empty);
        };
        _deleteButton.OnClicked += (sender, e) => {
            Close();
            OnDelete?.Invoke(this, EventArgs.Empty);
        };
    }

    public SellCartDialog(SellCartDialogController controller, Gtk.Window parent) : this(Builder.FromFile("sell_cart_dialog.ui"), controller, parent) {
    }
}