using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contém testes unitários para a classe <see cref="DeleteSaleHandler"/>.
/// </summary>
public class DeleteSaleHandlerTests
{
    /// <summary>
    /// Testa se uma venda é deletada com sucesso.
    /// </summary>
    [Fact]
    public async Task Handle_DeletesSaleSuccessfully()
    {
        // Arrange: cria mocks/fakes para dependências
        var repo = Substitute.For<ISaleRepository>();
        var handler = new DeleteSaleHandler(repo);
        var command = new DeleteSaleCommand(Guid.NewGuid());

        // Act: executa o handler
        await handler.Handle(command, CancellationToken.None);

        // Assert: verifica se o método DeleteAsync foi chamado uma vez
        await repo.Received(1).DeleteAsync(command.Id);
    }

    [Fact]
    public async Task Handle_DeletesSaleThatIsAlreadyCancelled()
    {
        var repo = Substitute.For<ISaleRepository>();
        var handler = new DeleteSaleHandler(repo);
        var sale = new Ambev.DeveloperEvaluation.Domain.Entities.Sale("S1", DateTime.Now, new Ambev.DeveloperEvaluation.Domain.ValueObjects.CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        sale.Cancel();
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns(sale);
        var command = new DeleteSaleCommand(Guid.NewGuid());
        await handler.Handle(command, CancellationToken.None);
        await repo.Received(1).DeleteAsync(command.Id);
        Assert.True(sale.IsCancelled);
    }

    [Fact]
    public async Task Handle_DeletesSaleThatDoesNotExist()
    {
        var repo = Substitute.For<ISaleRepository>();
        var handler = new DeleteSaleHandler(repo);
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns((Ambev.DeveloperEvaluation.Domain.Entities.Sale?)null);
        var command = new DeleteSaleCommand(Guid.NewGuid());
        var result = await handler.Handle(command, CancellationToken.None);
        await repo.Received(1).DeleteAsync(command.Id);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handle_AddsSaleCancelledDomainEvent()
    {
        var repo = Substitute.For<ISaleRepository>();
        var handler = new DeleteSaleHandler(repo);
        var sale = new Ambev.DeveloperEvaluation.Domain.Entities.Sale("S1", DateTime.Now, new Ambev.DeveloperEvaluation.Domain.ValueObjects.CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns(sale);
        var command = new DeleteSaleCommand(Guid.NewGuid());
        await handler.Handle(command, CancellationToken.None);
        Assert.Contains(sale.DomainEvents, e => e is Ambev.DeveloperEvaluation.Domain.Events.SaleCancelledEvent);
    }
}