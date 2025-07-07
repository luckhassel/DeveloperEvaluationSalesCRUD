using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Application.Dtos;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contém testes unitários para a classe <see cref="UpdateSaleHandler"/>.
/// </summary>
public class UpdateSaleHandlerTests
{
    /// <summary>
    /// Testa se uma venda é atualizada com sucesso quando encontrada.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesSale_WhenFound()
    {
        // Arrange: cria mocks/fakes para dependências
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new UpdateSaleHandler(repo, mapper, userRepo);
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns(sale);
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S2",
            Date = DateTime.Now,
            Branch = "B2",
            Items = new List<UpdateSaleItemDto> { new() { Product = new ProductDto { Id = Guid.NewGuid(), Name = "P2" }, Quantity = 4, UnitPrice = 20 } }
        };
        mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(new UpdateSaleResult());

        // Act: executa o handler
        await handler.Handle(command, CancellationToken.None);

        // Assert: verifica se o método UpdateAsync foi chamado uma vez
        await repo.Received(1).UpdateAsync(Arg.Any<Sale>());
    }

    /// <summary>
    /// Testa se uma exceção é lançada quando a venda não é encontrada.
    /// </summary>
    [Fact]
    public async Task Handle_Throws_WhenNotFound()
    {
        // Arrange
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new UpdateSaleHandler(repo, mapper, userRepo);
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns((Sale)null);
        var command = new UpdateSaleCommand { Id = Guid.NewGuid() };

        // Act & Assert: espera uma exceção
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenItemQuantityIsInvalid()
    {
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new UpdateSaleHandler(repo, mapper, userRepo);
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns(sale);
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S2",
            Date = DateTime.Now,
            Branch = "B2",
            Items = new List<UpdateSaleItemDto> { new() { Product = new ProductDto { Id = Guid.NewGuid(), Name = "P2" }, Quantity = 25, UnitPrice = 20 } }
        };
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AddsSaleModifiedDomainEvent()
    {
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new UpdateSaleHandler(repo, mapper, userRepo);
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns(sale);
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S2",
            Date = DateTime.Now,
            Branch = "B2",
            Items = new List<UpdateSaleItemDto> { new() { Product = new ProductDto { Id = Guid.NewGuid(), Name = "P2" }, Quantity = 4, UnitPrice = 20 } }
        };
        mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(new UpdateSaleResult());
        await handler.Handle(command, CancellationToken.None);
        Assert.Contains(sale.DomainEvents, e => e is Ambev.DeveloperEvaluation.Domain.Events.SaleModifiedEvent);
    }
}