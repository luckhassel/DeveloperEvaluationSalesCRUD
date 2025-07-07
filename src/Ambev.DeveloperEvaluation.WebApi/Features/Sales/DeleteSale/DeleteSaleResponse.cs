using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

public class DeleteSaleResponse
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
}