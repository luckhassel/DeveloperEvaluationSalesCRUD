namespace Ambev.DeveloperEvaluation.Common.Security;

/// <summary>
/// Define o contrato para representação de uma venda no sistema.
/// </summary>
public interface ISale
{
    /// <summary>
    /// Obtém o identificador único da venda.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Obtém o número da venda.
    /// </summary>
    string SaleNumber { get; }

    /// <summary>
    /// Obtém a data da venda.
    /// </summary>
    DateTime Date { get; }

    /// <summary>
    /// Obtém o nome do cliente.
    /// </summary>
    string Customer { get; }

    /// <summary>
    /// Obtém a filial onde a venda foi realizada.
    /// </summary>
    string Branch { get; }

    /// <summary>
    /// Obtém o valor total da venda.
    /// </summary>
    decimal TotalAmount { get; }

    /// <summary>
    /// Indica se a venda está cancelada.
    /// </summary>
    bool IsCancelled { get; }

    /// <summary>
    /// Itens da venda.
    /// </summary>
    IReadOnlyCollection<ISaleItem> Items { get; }
}