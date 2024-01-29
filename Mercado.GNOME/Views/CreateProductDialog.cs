using System;

using Mercado.GNOME.Helpers;
using Mercado.Shared.Controllers;

namespace Mercado.GNOME.Views;

public partial class CreateProductDialog : Adw.PreferencesWindow {
    private readonly CreateProductDialogController _controller;

#pragma warning disable CS0649
    [Gtk.Connect] private readonly Adw.EntryRow _nameEntry;
    [Gtk.Connect] private readonly Adw.EntryRow _codeEntry;
    [Gtk.Connect] private readonly Adw.EntryRow _priceEntry;
    [Gtk.Connect] private readonly Gtk.Button _deleteButton;
    [Gtk.Connect] private readonly Gtk.Button _applyButton;
#pragma warning restore CS0649

    public event EventHandler? OnApply;
    public event EventHandler? OnDelete;

    private CreateProductDialog(Gtk.Builder builder, CreateProductDialogController controller, Gtk.Window parent) : base(builder.GetPointer("_root"), false) {
        //Window Settings
        _controller = controller;
        SetTransientFor(parent);
        //Build UI
        builder.Connect(this);

        _nameEntry.SetText(_controller.Product.Name);
        _codeEntry.SetText(_controller.Product.Code.ToString());
        _priceEntry.SetText(_controller.Product.Price.ToString());

        _nameEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") { Validate(); }
        };
        _codeEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") { Validate(); }
        };
        _priceEntry.OnNotify += (sender, e) => {
            if (e.Pspec.GetName() == "text") { Validate(); }
        };

        _applyButton.OnClicked += (sender, e) => OnApply?.Invoke(this, null);

        _deleteButton.SetVisible(_controller.IsEditing);
        _deleteButton.OnClicked += (sender, e) => {
            Close();
            OnDelete?.Invoke(this, EventArgs.Empty);
        };
    }

    public CreateProductDialog(CreateProductDialogController controller, Gtk.Window parent) : this(Builder.FromFile("create_product_dialog.ui"), controller, parent) {
    }

    private void Validate() {
        ProductCheckStatus checkStatus = _controller.UpdateProduct(_nameEntry.GetText(), _codeEntry.GetText(), _priceEntry.GetText());

        _nameEntry.RemoveCssClass("error");
        _codeEntry.RemoveCssClass("error");
        _priceEntry.RemoveCssClass("error");

        if (checkStatus == ProductCheckStatus.Valid) {
            _applyButton.SetSensitive(true);
        }
        else {
            if (checkStatus.HasFlag(ProductCheckStatus.InvalidName)) {
                _nameEntry.AddCssClass("error");
            }
            if (checkStatus.HasFlag(ProductCheckStatus.InvalidCode)) {
                _codeEntry.AddCssClass("error");
            }
            if (checkStatus.HasFlag(ProductCheckStatus.InvalidPrice)) {
                _priceEntry.AddCssClass("error");
            }
            _applyButton.SetSensitive(false);
        }
    }
}