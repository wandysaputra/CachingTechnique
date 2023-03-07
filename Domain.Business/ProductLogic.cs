using System.Diagnostics;
using Domain.Model;
using Domain.Data;
using Domain.Data.Entities;
using Microsoft.Extensions.Logging;

namespace Domain.Business;

public class ProductLogic : IProductLogic
{
    private readonly ILogger<ProductLogic> _logger;
    private readonly IInMemoryRepository _inMemoryRepository;

    public ProductLogic(ILogger<ProductLogic> logger, IInMemoryRepository inMemoryRepository)
    {
        _logger = logger;
        _inMemoryRepository = inMemoryRepository;
    }

    public async Task<IEnumerable<ProductModel>> GetProductsForCategoryAsync(string category)
    {
        _logger.LogInformation("Getting products in logic for {category}", category);

        Activity.Current?.AddEvent(new ActivityEvent("Getting products from repository"));
        var products = await _inMemoryRepository.GetProductsAsync(category);

        var results = products.Select(ConvertToProductModel).ToList();

        Activity.Current?.AddEvent(new ActivityEvent("Retrieved products from repository"));

        return results;
    }

    public async Task<ProductModel?> GetProductByIdAsync(int id)
    {
        var product = await _inMemoryRepository.GetProductByIdAsync(id);
        return product != null ? ConvertToProductModel(product) : null;
    }

    public IEnumerable<ProductModel> GetProductsForCategory(string category)
    {
        var products =  _inMemoryRepository.GetProducts(category);

        return products.Select(ConvertToProductModel).ToList();
    }

    public ProductModel? GetProductById(int id)
    {
        var product = _inMemoryRepository.GetProductById(id);
        return product != null ? ConvertToProductModel(product) : null;
    }

    private static ProductModel ConvertToProductModel(Product product)
    {
        var productToAdd = new ProductModel
        {
            Id = product.Id,
            Category = product.Category,
            Description = product.Description,
            ImgUrl = product.ImgUrl,
            Name = product.Name,
            Price = product.Price
        };
        var rating = product.Rating;
        
        if (rating == null)
        {
            return productToAdd;
        }

        productToAdd.Rating = rating.AggregateRating;
        productToAdd.NumberOfRatings = rating.NumberOfRatings;

        return productToAdd;
    }
}