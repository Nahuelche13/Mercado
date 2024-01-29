using System;
using System.Collections.Generic;

using Mercado.Shared.Models;

namespace Mercado.Shared.Controllers;

public class ManageProductsViewController {
    private uint Offset = 0;
    private string SearchText = "";
    
    public const uint Limit = 30;

    private IEnumerable<Product> _products { get; set; }
    public IEnumerable<Product> Products {
        get => _products;

        private set {
            if (_products != value) {
                _products = value;
                OnProductsChanged?.Invoke(this, _products);
            }
        }
    }

    public uint Page => Offset / Limit;

    public ManageProductsViewController() {
        Products = Product.Read(Offset, Limit);
    }

    public event EventHandler<IEnumerable<Product>>? OnProductsChanged;

    public void UpdateSearchText(object sender, StringEventArgs args) {
        SearchText = args.StringArg;
        Products = Product.Search(SearchText, Offset, Limit);
    }

    public void ReloadPage(object sender, EventArgs args) {
        Products = Product.Search(SearchText, Offset, Limit);
    }
    public void PreviousPage(object sender, EventArgs args) {
        try {
            checked {
                Offset -= Limit;
                Products = Product.Search(SearchText, Offset, Limit);
            }
        }
        catch (OverflowException) { }
    }
    public void NextPage(object sender, EventArgs args) {
        Offset += Limit;
        Products = Product.Search(SearchText, Offset, Limit);
    }
}

public class StringEventArgs : EventArgs
{
    public string StringArg { get; }

    public StringEventArgs(string stringArg)
    {
        StringArg = stringArg;
    }
}