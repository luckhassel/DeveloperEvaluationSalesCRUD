using MediatR;
using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Application.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Comando para criar uma nova venda.
/// </summary>
public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    /// <summary>
    /// Número da venda.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Data da venda.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Nome do cliente.
    /// </summary>
    public CustomerInfo Customer { get; set; } = null!;

    /// <summary>
    /// Filial onde a venda foi realizada.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Lista de itens da venda.
    /// </summary>
    public List<CreateSaleItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO para item de venda no comando de criação.
/// </summary>
public class CreateSaleItemDto
{
    /// <summary>
    /// Produto do item.
    /// </summary>
    public ProductDto Product { get; set; } = new();
    /// <summary>
    /// Quantidade do produto.
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Preço unitário do produto.
    /// </summary>
    public decimal UnitPrice { get; set; }
}