using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Representa uma venda no sistema, incluindo regras de negócio e gerenciamento de itens.
/// </summary>
public class Sale : BaseEntity, ISale
{
    /// <summary>
    /// Obtém ou define o número da venda.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a data da venda.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Obtém ou define o cliente da venda.
    /// </summary>
    public CustomerInfo Customer { get; private set; }

    /// <summary>
    /// Obtém ou define a filial onde a venda foi realizada.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Obtém o valor total da venda.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Indica se a venda está cancelada.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Obtém a lista de itens da venda.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Implementação explícita de ISale.Items para expor os itens como somente leitura.
    /// </summary>
    IReadOnlyCollection<ISaleItem> ISale.Items => Items;

    /// <summary>
    /// Implementação explícita de ISale.Id para expor o identificador como string.
    /// </summary>
    string ISale.Id => Id.ToString();

    /// <summary>
    /// Implementação explícita de ISale.Customer para expor o nome do cliente como string.
    /// </summary>
    string ISale.Customer => Customer.Name;

    /// <summary>
    /// Inicializa uma nova instância da classe Sale.
    /// </summary>
    public Sale(string saleNumber, DateTime date, CustomerInfo customer, string branch)
    {
        SaleNumber = saleNumber;
        Date = date;
        Customer = customer;
        Branch = branch;
        IsCancelled = false;
        AddDomainEvent(new SaleCreatedEvent(Id));
    }

    /// <summary>
    /// Adiciona um item à venda, aplicando regras de negócio para descontos e limites.
    /// </summary>
    /// <param name="product">Nome do produto.</param>
    /// <param name="quantity">Quantidade do produto.</param>
    /// <param name="unitPrice">Preço unitário do produto.</param>
    public void AddItem(ProductInfo product, int quantity, decimal unitPrice)
    {
        if (quantity < 1)
            throw new ArgumentException("A quantidade deve ser pelo menos 1.");
        if (quantity > 20)
            throw new ArgumentException("Não é possível vender mais de 20 itens idênticos.");

        decimal discount = 0;
        if (quantity >= 10 && quantity <= 20)
            discount = 0.20m;
        else if (quantity >= 4)
            discount = 0.10m;

        var item = new SaleItem(product, quantity, unitPrice, discount);
        Items.Add(item);
        CalculateTotal();
    }

    /// <summary>
    /// Cancela a venda.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        AddDomainEvent(new SaleCancelledEvent(Id));
    }

    /// <summary>
    /// Calcula o valor total da venda com base nos itens.
    /// </summary>
    private void CalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.Total);
    }

    public void MarkModified()
    {
        AddDomainEvent(new SaleModifiedEvent(Id));
    }

    // Parameterless constructor for EF Core
    protected Sale() { }
}