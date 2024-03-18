using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Models;

namespace CatalogService.Profiles;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        // Products
        CreateMap<Product, ProductReadDto>();

        // Category
        CreateMap<Category, CategoryReadDto>();

        // Product Category
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryReadDto>();
        CreateMap<ProductCategoryReadDto, ProductCategory>();

        //Purchase
        CreateMap<PurchaseDto, Purchase>();
        CreateMap<Purchase, PurchaseDto>();
    }
}
