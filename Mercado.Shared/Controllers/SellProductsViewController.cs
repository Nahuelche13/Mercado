using System;
using System.Collections.Generic;
using System.Linq;

using Mercado.Shared.Models;

namespace Mercado.Shared.Controllers;

public class SellProductsViewController {
    private string _searchText = "";
    private IEnumerable<Product> _searchCandidates = [];
    private readonly IDictionary<Product, decimal> _cartDict = new Dictionary<Product, decimal>();

    public string SearchText {
        get => _searchText;

        set {
            _searchText = value;
            UpdateSearch();
        }
    }
    public IEnumerable<Product> SearchCandidates => _searchCandidates;
    public decimal TotalPrice => _cartDict.Sum((k) => k.Key.Price * k.Value);

    public event EventHandler<IEnumerable<Product>> OnUpdadeSearchBox;
    public event EventHandler<IDictionary<Product, decimal>> OnUpdadeCartBox;
    public event EventHandler<decimal> OnUpdadeTotalLabel;

    private void UpdateSearch() {
        try {
            _searchCandidates = Product.Search(long.Parse(_searchText));
        }
        catch (Exception) {
            _searchCandidates = Product.Search(_searchText);
        }
        OnUpdadeSearchBox?.Invoke(this, _searchCandidates);
    }

    public void OnClearButton(object sender, EventArgs args) {
        _cartDict.Clear();
        OnUpdadeCartBox?.Invoke(this, _cartDict);
    }

    public void RemoveProductOfCart(Product product) {
        _cartDict.Remove(product);
        OnUpdadeCartBox?.Invoke(this, _cartDict);
    }
    public void AddProductToCart(Product product) {
        if (_cartDict.TryGetValue(product, out decimal value)) {
            _cartDict[product] = ++value;
        }
        else {
            _cartDict.Add(product, 1);
        }
        OnUpdadeCartBox?.Invoke(this, _cartDict);
    }
    public void ChangeAmmountOfProduct(Product product, decimal ammount) {
        _cartDict[product] = ammount;
        // OnUpdadeCartBox?.Invoke(this, _cartDict);
        OnUpdadeTotalLabel?.Invoke(this, TotalPrice);
    }
}