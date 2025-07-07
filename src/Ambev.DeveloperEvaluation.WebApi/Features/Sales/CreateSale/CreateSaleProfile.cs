using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Dtos;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Application.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemDto>();
        CreateMap<CreateSaleResult, CreateSaleResponse>();
        CreateMap<CreateSaleItemResult, CreateSaleItemResponse>();
        CreateMap<CustomerRequest, CustomerInfo>()
            .ConstructUsing(src => new CustomerInfo(src.Id, string.Empty, string.Empty));
        CreateMap<ProductRequest, ProductDto>()
            .ConstructUsing(src => new ProductDto { Id = src.Id, Name = src.Name });
        CreateMap<ProductRequest, ProductDto>();
        CreateMap<ProductInfo, ProductDto>();
    }
}