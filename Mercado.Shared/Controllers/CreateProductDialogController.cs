using System;

using Mercado.Shared.Models;

namespace Mercado.Shared.Controllers;

[Flags]
public enum ProductCheckStatus {
    Valid = 1,
    InvalidName = 2,
    InvalidCode = 4,
    InvalidPrice = 8,
}

public class CreateProductDialogController {
    public Product Product { get; set; }
    public bool IsEditing { get; init; }

    public CreateProductDialogController() {
        Product = new Product(0, "", null, 0);
        IsEditing = false;
    }
    public CreateProductDialogController(Product product) {
        Product = product;
        IsEditing = true;
    }

    public ProductCheckStatus UpdateProduct(string name, string codeString, string priceString) {
        ProductCheckStatus result = 0;
        long code = 0L;
        decimal price = 0m;
        if (string.IsNullOrWhiteSpace(name)) {
            result |= ProductCheckStatus.InvalidName;
        }
        try {
            code = long.Parse(codeString);
        }
        catch {
            if (!string.IsNullOrWhiteSpace(codeString)) {
                result |= ProductCheckStatus.InvalidCode;
            }
        }
        try {
            price = decimal.Parse(priceString);
        }
        catch {
            result |= ProductCheckStatus.InvalidPrice;
        }
        if (result != 0) {
            return result;
        }
        Product = new Product(Product.Id, name, code, price);
        return ProductCheckStatus.Valid;
    }
}