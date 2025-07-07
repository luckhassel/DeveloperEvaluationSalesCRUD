using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IUserRepository userRepository)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id);
        if (sale == null)
            throw new Exception($"Sale with id {command.Id} not found");
        sale.SaleNumber = command.SaleNumber;
        sale.Date = command.Date;
        sale.Branch = command.Branch;
        sale.Items.Clear();
        foreach (var item in command.Items)
        {
            var productInfo = new ProductInfo(item.Product.Id, item.Product.Name);
            sale.AddItem(productInfo, item.Quantity, item.UnitPrice);
            if (item is { Id: not null } && item is { IsCancelled: true })
            {
                var addedItem = sale.Items.Last();
                addedItem.Cancel(sale.Id);
            }
        }
        sale.MarkModified();
        await _saleRepository.UpdateAsync(sale);
        var result = _mapper.Map<UpdateSaleResult>(sale);
        return result;
    }
}