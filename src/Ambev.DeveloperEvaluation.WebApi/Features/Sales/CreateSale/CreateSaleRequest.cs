using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.WebApi.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public CustomerRequest Customer { get; set; } = new();
    public string Branch { get; set; } = string.Empty;
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}

public class CreateSaleItemRequest
{
    public ProductRequest Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CustomerRequest
{
    public Guid Id { get; set; }
}