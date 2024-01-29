using System;

using Mercado.GNOME.Helpers;
using Mercado.Shared.Models;

namespace Mercado.GNOME.Controls;

public partial class ProductRow3 : Gtk.FlowBoxChild {
    private Product _product;

#pragma warning disable CS0649
    [Gtk.Connect] private readonly Adw.ActionRow _row;
    [Gtk.Connect] private readonly Gtk.Label _amountLabel;
    [Gtk.Connect] private readonly Gtk.Button _editButton;
    [Gtk.Connect] private readonly Gtk.SpinButton _ammountSpinner;
    // [Gtk.Connect] private readonly Gtk.Box _suffixBox;
#pragma warning restore CS0649

    public int Id => _product.Id;

    public event EventHandler<int>? EditTriggered;
    public event EventHandler<double>? SpinnerTriggered;

    private ProductRow3(Gtk.Builder builder, Product product, decimal ammount) : base(builder.GetPointer("_root"), false) {
        _product = product;
        //Build UI
        builder.Connect(this);
        _editButton.OnClicked += Edit;
        //Group Settings
        UpdateRow(product);

        _ammountSpinner.SetValue((double)ammount);
        _ammountSpinner.OnValueChanged += Spinner;
    }

    public ProductRow3(Product product, decimal ammount) : this(Builder.FromFile("product_row_3.ui"), product, ammount) { }

    public void UpdateRow(Product product) {
        _product = product;
        //Row Settings
        _row.SetTitle(_product.Name);
        _row.SetSubtitle(_product.Code.ToString() ?? "Sin código de barras");
        //Amount Label
        _amountLabel.SetLabel($"${_product.Price}");
    }

    private void Edit(Gtk.Button sender, EventArgs e) => EditTriggered?.Invoke(this, Id);
    private void Spinner(Gtk.SpinButton sender, EventArgs e) {
        double number = _ammountSpinner.GetValue();
        SpinnerTriggered?.Invoke(this, number);
    }
}