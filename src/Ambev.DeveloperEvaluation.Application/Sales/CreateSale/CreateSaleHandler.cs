using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler responsável por processar o comando de criação de venda.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CreateSaleHandler"/>.
    /// </summary>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IUserRepository userRepository)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Processa o comando de criação de venda.
    /// </summary>
    /// <param name="command">Comando de criação de venda.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da criação da venda.</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.Customer.Id, cancellationToken);
        if (user == null || user.Role != UserRole.Customer)
            throw new InvalidOperationException("O cliente informado não existe ou não possui o papel de Customer.");
        var customerInfo = new CustomerInfo(user.Id, user.Username, user.Email);
        var sale = new Sale(command.SaleNumber, command.Date, customerInfo, command.Branch);
        foreach (var item in command.Items)
        {
            var productInfo = new ProductInfo(item.Product.Id, item.Product.Name);
            sale.AddItem(productInfo, item.Quantity, item.UnitPrice);
        }

        await _saleRepository.AddAsync(sale);
        var result = _mapper.Map<CreateSaleResult>(sale);
        return result;
    }
}