using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.WebApi.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequest
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Branch { get; set; } = string.Empty;
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}

public class UpdateSaleItemRequest
{
    public Guid? Id { get; set; }
    public ProductRequest Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}