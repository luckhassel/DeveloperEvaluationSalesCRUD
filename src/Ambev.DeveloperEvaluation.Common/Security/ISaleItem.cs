namespace Ambev.DeveloperEvaluation.Common.Security;

/// <summary>
/// Define o contrato para representação de um item de venda no sistema.
/// </summary>
public interface ISaleItem
{
    /// <summary>
    /// Obtém o identificador único do item de venda.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Obtém o nome do produto.
    /// </summary>
    string Product { get; }

    /// <summary>
    /// Obtém a quantidade do produto.
    /// </summary>
    int Quantity { get; }

    /// <summary>
    /// Obtém o preço unitário do produto.
    /// </summary>
    decimal UnitPrice { get; }

    /// <summary>
    /// Obtém o desconto aplicado ao item.
    /// </summary>
    decimal Discount { get; }

    /// <summary>
    /// Obtém o valor total do item.
    /// </summary>
    decimal Total { get; }

    /// <summary>
    /// Indica se o item está cancelado.
    /// </summary>
    bool IsCancelled { get; }
}