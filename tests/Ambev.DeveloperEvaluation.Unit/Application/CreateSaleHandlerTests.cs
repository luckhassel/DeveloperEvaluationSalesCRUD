using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Application.Dtos;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation.TestHelper;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Dtos;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contém testes unitários para a classe <see cref="CreateSaleHandler"/>.
/// </summary>
public class CreateSaleHandlerTests
{
    /// <summary>
    /// Testa se uma venda é criada com sucesso quando os dados são válidos.
    /// </summary>
    [Fact]
    public async Task Handle_CreatesSaleSuccessfully()
    {
        // Arrange: cria mocks/fakes para dependências
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new CreateSaleHandler(repo, mapper, userRepo);
        var command = new Ambev.DeveloperEvaluation.Application.Sales.CreateSale.CreateSaleCommand
        {
            SaleNumber = "S1",
            Date = DateTime.Now,
            Customer = new CustomerInfo(Guid.NewGuid(), "Test User", "test@email.com"),
            Branch = "Branch",
            Items = new List<CreateSaleItemDto> { new() { Product = new ProductDto { Id = Guid.NewGuid(), Name = "P1" }, Quantity = 5, UnitPrice = 10 } }
        };
        userRepo.GetByIdAsync(command.Customer.Id, Arg.Any<CancellationToken>()).Returns(new Ambev.DeveloperEvaluation.Domain.Entities.User { Id = command.Customer.Id, Username = "Test User", Email = "test@email.com", Role = UserRole.Customer });
        mapper.Map<CreateSaleResult>(Arg.Any<Ambev.DeveloperEvaluation.Domain.Entities.Sale>()).Returns(new CreateSaleResult());

        // Act: executa o handler
        await handler.Handle(command, CancellationToken.None);

        // Assert: verifica se o método AddAsync foi chamado uma vez
        await repo.Received(1).AddAsync(Arg.Any<Sale>());
    }

    [Fact]
    public async Task Handle_Throws_WhenCustomerNotFoundOrNotCustomerRole()
    {
        // Arrange
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new CreateSaleHandler(repo, mapper, userRepo);
        var command = new Ambev.DeveloperEvaluation.Application.Sales.CreateSale.CreateSaleCommand
        {
            SaleNumber = "S1",
            Date = DateTime.Now,
            Customer = new CustomerInfo(Guid.NewGuid(), "Test User", "test@email.com"),
            Branch = "Branch",
            Items = new List<CreateSaleItemDto> { new() { Product = new ProductDto { Id = Guid.NewGuid(), Name = "P1" }, Quantity = 5, UnitPrice = 10 } }
        };
        userRepo.GetByIdAsync(command.Customer.Id, Arg.Any<CancellationToken>()).Returns((Ambev.DeveloperEvaluation.Domain.Entities.User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        // Testa caso o usuário existe mas não é Customer
        userRepo.GetByIdAsync(command.Customer.Id, Arg.Any<CancellationToken>()).Returns(new Ambev.DeveloperEvaluation.Domain.Entities.User { Id = command.Customer.Id, Username = "Test User", Email = "test@email.com", Role = UserRole.Admin });
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenItemQuantityIsInvalid()
    {
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        var handler = new CreateSaleHandler(repo, mapper, userRepo);
        var command = new Ambev.DeveloperEvaluation.Application.Sales.CreateSale.CreateSaleCommand
        {
            SaleNumber = "S1",
            Date = DateTime.Now,
            Customer = new CustomerInfo(Guid.NewGuid(), "Test User", "test@email.com"),
            Branch = "Branch",
            Items = new List<CreateSaleItemDto> { new() { Product = new ProductDto { Id = Guid.NewGuid(), Name = "P1" }, Quantity = 25, UnitPrice = 10 } }
        };
        userRepo.GetByIdAsync(command.Customer.Id, Arg.Any<CancellationToken>()).Returns(new Ambev.DeveloperEvaluation.Domain.Entities.User { Id = command.Customer.Id, Username = "Test User", Email = "test@email.com", Role = UserRole.Customer });
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AddsSaleCreatedDomainEvent()
    {
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var userRepo = Substitute.For<IUserRepository>();
        Sale? capturedSale = null;
        repo.AddAsync(Arg.Do<Sale>(s => capturedSale = s)).Returns(Task.CompletedTask);
        var handler = new CreateSaleHandler(repo, mapper, userRepo);
        var command = new Ambev.DeveloperEvaluation.Application.Sales.CreateSale.CreateSaleCommand
        {
            SaleNumber = "S1",
            Date = DateTime.Now,
            Customer = new CustomerInfo(Guid.NewGuid(), "Test User", "test@email.com"),
            Branch = "Branch",
            Items = new List<CreateSaleItemDto> { new() { Product = new ProductDto { Id = Guid.NewGuid(), Name = "P1" }, Quantity = 5, UnitPrice = 10 } }
        };
        userRepo.GetByIdAsync(command.Customer.Id, Arg.Any<CancellationToken>()).Returns(new Ambev.DeveloperEvaluation.Domain.Entities.User { Id = command.Customer.Id, Username = "Test User", Email = "test@email.com", Role = UserRole.Customer });
        mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(new CreateSaleResult());
        await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(capturedSale);
        Assert.Contains(capturedSale.DomainEvents, e => e is Ambev.DeveloperEvaluation.Domain.Events.SaleCreatedEvent);
    }
}

public class CreateSaleRequestValidatorTests
{
    private readonly CreateSaleRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_SaleNumber_Is_Empty()
    {
        var model = new CreateSaleRequest { SaleNumber = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SaleNumber);
    }

    [Fact]
    public void Should_Have_Error_When_Items_Is_Empty()
    {
        var model = new CreateSaleRequest { SaleNumber = "S1", Items = new List<CreateSaleItemRequest>() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Should_Have_Error_When_Quantity_Is_Invalid()
    {
        var model = new CreateSaleRequest
        {
            SaleNumber = "S1",
            Items = new List<CreateSaleItemRequest> { new() { Product = new ProductRequest { Id = Guid.NewGuid(), Name = "P1" }, Quantity = 0, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Should_Have_Error_When_UnitPrice_Is_Invalid()
    {
        var model = new CreateSaleRequest
        {
            SaleNumber = "S1",
            Items = new List<CreateSaleItemRequest> { new() { Product = new ProductRequest { Id = Guid.NewGuid(), Name = "P1" }, Quantity = 5, UnitPrice = 0 } }
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveAnyValidationError();
    }
}