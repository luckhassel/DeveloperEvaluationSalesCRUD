using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Application.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public CustomerInfoDto Customer { get; set; } = new();
    public string Branch { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<UpdateSaleItemResult> Items { get; set; } = new();
}

public class UpdateSaleItemResult
{
    public Guid? Id { get; set; }
    public ProductDto Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}