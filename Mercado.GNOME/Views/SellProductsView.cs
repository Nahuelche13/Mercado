using System;
using System.Collections.Generic;

using Mercado.GNOME.Controls;
using Mercado.GNOME.Helpers;
using Mercado.Shared.Controllers;
using Mercado.Shared.Models;

namespace Mercado.GNOME.Views;

class SellProductsView : Adw.Bin {
    private readonly SellProductsViewController _controller;
    private readonly Gtk.Window _parentWindow;

#pragma warning disable CS0649
    [Gtk.Connect] private readonly Gtk.SearchEntry _search_entry;
    [Gtk.Connect] private readonly Gtk.ListBox _search_box;
    [Gtk.Connect] private readonly Gtk.ListBox _cart_box;
    [Gtk.Connect] private readonly Gtk.Label _total_price_label;
    [Gtk.Connect] private readonly Gtk.Button _clear_button;
    [Gtk.Connect] private readonly Gtk.Button _pay_button;
#pragma warning restore CS0649

    private readonly Gtk.EventControllerKey _searchKeyController;

    private SellProductsView(Gtk.Builder builder, SellProductsViewController controller, Gtk.Window parentWindow) : base(builder.GetPointer("_root"), false) {
        _controller = controller;
        _parentWindow = parentWindow;

        builder.Connect(this);

        _search_entry.OnSearchChanged += (sender, e) => _controller.SearchText = _search_entry.GetText();
        _clear_button.OnClicked += _controller.OnClearButton;
        _pay_button.OnClicked += (sender, e) => {
            SellCartDialogController sellCartDialogController = new(_controller.TotalPrice);
            SellCartDialog sellCartDialog = new(sellCartDialogController, _parentWindow);
            sellCartDialog.Present();
        }; ;

        _controller.OnUpdadeSearchBox += UpdadeSearchBox;
        _controller.OnUpdadeCartBox += UpdadeCartBox;
        _controller.OnUpdadeTotalLabel += UpdadeTotalPriceLabel;

        _total_price_label.SetLabel($"${_controller.TotalPrice}");

        _search_entry.SetKeyCaptureWidget(this);

        _searchKeyController = Gtk.EventControllerKey.New();
        _searchKeyController.SetPropagationPhase(Gtk.PropagationPhase.Capture);
        _searchKeyController.OnKeyPressed += (sender, e) => {
            if (e.Keycode == 36) {
                foreach (Product item in _controller.SearchCandidates) {
                    Console.WriteLine(item.Name);
                }
                _controller.SearchText = _search_entry.GetText();
                foreach (Product item in _controller.SearchCandidates) {
                    Console.WriteLine(item.Name);
                }

                using IEnumerator<Product> enumer = _controller.SearchCandidates.GetEnumerator();
                if (enumer.MoveNext()) {
                    _controller.AddProductToCart(enumer.Current);
                    _search_entry.SetText("");
                }
                return true;
            }
            return false;
        };
        _search_entry.AddController(_searchKeyController);
    }

    private void UpdadeSearchBox(object? sender, IEnumerable<Product> products) {
        _search_box.RemoveAll();
        foreach (Product product in products) {
            ProductRow2 productRow = new(product);
            productRow.EditTriggered += (sender, e) => _controller.AddProductToCart(product);
            _search_box.Append(productRow);
        }
    }

    private void UpdadeCartBox(object? sender, IDictionary<Product, decimal> products) {
        _cart_box.RemoveAll();
        foreach ((Product product, decimal ammount) in products) {
            ProductRow3 productRow = new(product, ammount);
            productRow.EditTriggered += (sender, e) => _controller.RemoveProductOfCart(product);
            productRow.SpinnerTriggered += (sender, e) => _controller.ChangeAmmountOfProduct(product, (decimal)e);
            _cart_box.Append(productRow);
        }
        _total_price_label.SetLabel($"${_controller.TotalPrice}");
    }
    private void UpdadeTotalPriceLabel(object? sender, decimal totalPrice) {
        _total_price_label.SetLabel($"${_controller.TotalPrice}");
    }

    public SellProductsView(SellProductsViewController controller, Gtk.Window parentWindow) : this(Builder.FromFile("sell_products_view.ui"), controller, parentWindow) {
    }
}