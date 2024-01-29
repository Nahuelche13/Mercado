using System.Collections.Generic;

using Mercado.GNOME.Controls;
using Mercado.GNOME.Helpers;
using Mercado.Shared.Controllers;
using Mercado.Shared.Models;

namespace Mercado.GNOME.Views;

class ManageProductsView : Adw.Bin {
    private readonly ManageProductsViewController _controller;
    private readonly Gtk.Window _parentWindow;

#pragma warning disable CS0649
    [Gtk.Connect] private readonly Gtk.SearchEntry _searchEntry;
    [Gtk.Connect] private readonly Gtk.FlowBox _flowBox;
    [Gtk.Connect] private readonly Gtk.Label _currentPageLabel;
    [Gtk.Connect] private readonly Gtk.Button _previousPageButton;
    [Gtk.Connect] private readonly Gtk.Button _nextPageButton;
#pragma warning restore CS0649

    private ManageProductsView(Gtk.Builder builder, ManageProductsViewController controller, Gtk.Window parentWindow) : base(builder.GetPointer("_root"), false) {
        _controller = controller;
        _parentWindow = parentWindow;

        builder.Connect(this);

        _searchEntry.OnSearchChanged += (sender, e) => _controller.UpdateSearchText(this, new(_searchEntry.GetText()));
        _previousPageButton.OnClicked += _controller.PreviousPage;
        _nextPageButton.OnClicked += _controller.NextPage;

        _controller.OnProductsChanged += RenderFlowBox;

        RenderFlowBox(this, _controller.Products);
    }

    private void RenderFlowBox(object sender, IEnumerable<Product> products) {
        _currentPageLabel.SetText(_controller.Page.ToString());

        _flowBox.RemoveAll();
        foreach (Product product in products) {
            ProductRow productRow = new(product);
            productRow.EditTriggered += (sender, e) => {
                CreateProductDialogController createProductController = new(product);
                CreateProductDialog createProductDialog = new(createProductController, _parentWindow);
                createProductDialog.Present();

                createProductDialog.OnApply += async (s, ex) => {
                    (int _, int error) = await Product.Update(createProductController.Product);
                    Adw.MessageDialog dialog = SQLErrorCodes.FillDialogSQLErrorCodes(_parentWindow, error, "Producto actualizado");
                    dialog.Present();

                    _controller.ReloadPage(this, null);
                    createProductDialog.Destroy();
                };
                createProductDialog.OnDelete += async (s, ex) => {
                    (int _, int error) = await Product.Delete(createProductController.Product.Id);
                    Adw.MessageDialog dialog = SQLErrorCodes.FillDialogSQLErrorCodes(_parentWindow, error, "Producto eliminado");
                    dialog.Present();

                    _controller.ReloadPage(this, null);
                    createProductDialog.Destroy();
                };
            };
            _flowBox.Append(productRow);
        }
    }

    public ManageProductsView(ManageProductsViewController controller, Gtk.Window parentWindow) : this(Builder.FromFile("manage_products_view.ui"), controller, parentWindow) { }
}