using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Common;
using System;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Representa um item de venda, incluindo produto, quantidade, preço e status de cancelamento.
/// </summary>
public class SaleItem : BaseEntity, ISaleItem
{
    /// <summary>
    /// Obtém ou define o produto.
    /// </summary>
    public ProductInfo Product { get; private set; }

    /// <summary>
    /// Obtém ou define a quantidade do produto.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Obtém ou define o preço unitário do produto.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Obtém ou define o desconto aplicado ao item.
    /// </summary>
    public decimal Discount { get; set; } // 0.10 for 10%, 0.20 for 20%, etc.

    /// <summary>
    /// Obtém ou define o valor total do item.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Indica se o item está cancelado.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Implementação explícita de ISaleItem.Id para expor o identificador como string.
    /// </summary>
    string ISaleItem.Id => Id.ToString();

    /// <summary>
    /// Implementação explícita de ISaleItem.Product para expor o nome do produto como string.
    /// </summary>
    string ISaleItem.Product => Product.Name;

    /// <summary>
    /// Inicializa uma nova instância da classe SaleItem.
    /// </summary>
    /// <param name="product">Produto.</param>
    /// <param name="quantity">Quantidade do produto.</param>
    /// <param name="unitPrice">Preço unitário do produto.</param>
    /// <param name="discount">Desconto aplicado ao item.</param>
    public SaleItem(ProductInfo product, int quantity, decimal unitPrice, decimal discount)
    {
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        IsCancelled = false;
        Total = CalculateTotal();
    }

    /// <summary>
    /// Calcula o valor total do item considerando o desconto.
    /// </summary>
    /// <returns>Valor total do item.</returns>
    private decimal CalculateTotal()
    {
        var subtotal = Quantity * UnitPrice;
        var discountAmount = subtotal * Discount;
        return subtotal - discountAmount;
    }

    /// <summary>
    /// Cancela o item de venda.
    /// </summary>
    public void Cancel(Guid saleId)
    {
        IsCancelled = true;
        AddDomainEvent(new ItemCancelledEvent(saleId, Id));
    }

    // Parameterless constructor for EF Core
    protected SaleItem() { }
}