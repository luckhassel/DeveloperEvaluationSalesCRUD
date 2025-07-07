using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact]
    public void AddItem_Applies10PercentDiscount_WhenQuantityIs4To9()
    {
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        var product = new ProductInfo(Guid.NewGuid(), "ProductA");
        sale.AddItem(product, 5, 100);
        Assert.Single(sale.Items);
        Assert.Equal(0.10m, sale.Items[0].Discount);
        Assert.Equal(450, sale.Items[0].Total); // 5*100*0.9
    }

    [Fact]
    public void AddItem_Applies20PercentDiscount_WhenQuantityIs10To20()
    {
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        var product = new ProductInfo(Guid.NewGuid(), "ProductA");
        sale.AddItem(product, 12, 50);
        Assert.Single(sale.Items);
        Assert.Equal(0.20m, sale.Items[0].Discount);
        Assert.Equal(480, sale.Items[0].Total); // 12*50*0.8
    }

    [Fact]
    public void AddItem_Throws_WhenQuantityAbove20()
    {
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        var product = new ProductInfo(Guid.NewGuid(), "ProductA");
        Assert.Throws<ArgumentException>(() => sale.AddItem(product, 21, 10));
    }

    [Fact]
    public void AddItem_NoDiscount_WhenQuantityBelow4()
    {
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        var product = new ProductInfo(Guid.NewGuid(), "ProductA");
        sale.AddItem(product, 2, 100);
        Assert.Single(sale.Items);
        Assert.Equal(0, sale.Items[0].Discount);
        Assert.Equal(200, sale.Items[0].Total);
    }

    [Fact]
    public void Cancel_SetsIsCancelledToTrue()
    {
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        sale.Cancel();
        Assert.True(sale.IsCancelled);
    }

    [Fact]
    public void TotalAmount_IsCorrect_AfterMultipleItems()
    {
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        var productA = new ProductInfo(Guid.NewGuid(), "ProductA");
        var productB = new ProductInfo(Guid.NewGuid(), "ProductB");
        sale.AddItem(productA, 2, 100); // 200
        sale.AddItem(productB, 10, 50); // 10*50*0.8 = 400
        Assert.Equal(600, sale.TotalAmount);
    }

    [Fact]
    public void Cancel_AddsSaleCancelledDomainEvent()
    {
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        sale.Cancel();
        Assert.Contains(sale.DomainEvents, e => e is Ambev.DeveloperEvaluation.Domain.Events.SaleCancelledEvent);
    }
}

public class SaleItemTests
{
    [Fact]
    public void CalculateTotal_ReturnsCorrectValue_WithDiscount()
    {
        var product = new ProductInfo(Guid.NewGuid(), "ProductA");
        var item = new SaleItem(product, 5, 100, 0.10m); // 5*100*0.9 = 450
        Assert.Equal(450, item.Total);
    }

    [Fact]
    public void Cancel_SetsIsCancelledAndAddsDomainEvent()
    {
        var product = new ProductInfo(Guid.NewGuid(), "ProductA");
        var item = new SaleItem(product, 2, 100, 0);
        var saleId = Guid.NewGuid();
        item.Cancel(saleId);
        Assert.True(item.IsCancelled);
        Assert.Contains(item.DomainEvents, e => e is ItemCancelledEvent ev && ev.SaleId == saleId && ev.ItemId == item.Id);
    }
}