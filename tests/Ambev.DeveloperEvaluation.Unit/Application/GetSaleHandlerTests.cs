using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contém testes unitários para a classe <see cref="GetSaleHandler"/>.
/// </summary>
public class GetSaleHandlerTests
{
    /// <summary>
    /// Testa se uma venda é retornada quando encontrada no repositório.
    /// </summary>
    [Fact]
    public async Task Handle_ReturnsSale_WhenFound()
    {
        // Arrange: cria mocks/fakes para dependências
        var repo = Substitute.For<ISaleRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new GetSaleHandler(repo, mapper);
        var sale = new Sale("S1", DateTime.Now, new CustomerInfo(Guid.NewGuid(), "Customer", "customer@email.com"), "Branch");
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns(sale);
        mapper.Map<GetSaleResult>(Arg.Any<Sale>()).Returns(new GetSaleResult());

        // Act: executa o handler
        var result = await handler.Handle(new GetSaleCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert: verifica se o resultado não é nulo
        Assert.NotNull(result);
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
        var handler = new GetSaleHandler(repo, mapper);
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns((Sale)null);

        // Act & Assert: espera uma exceção
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new GetSaleCommand(Guid.NewGuid()), CancellationToken.None));
    }
}