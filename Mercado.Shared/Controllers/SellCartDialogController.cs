namespace Mercado.Shared.Controllers;

public class SellCartDialogController(decimal totalAmount) {
    public decimal TotalAmount { get; init; } = totalAmount;
}