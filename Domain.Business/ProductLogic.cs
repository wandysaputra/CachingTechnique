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
    private readonly IDistributeCacheRepository _distributeCacheRepository;

    public ProductLogic(ILogger<ProductLogic> logger, IInMemoryRepository inMemoryRepository, IDistributeCacheRepository distributeCacheRepository)
    {
        _logger = logger;
        _inMemoryRepository = inMemoryRepository;
        _distributeCacheRepository = distributeCacheRepository;
    }

    public async Task<IEnumerable<ProductModel>> GetProductsForCategoryAsync(string category)
    {
        _logger.LogInformation("Getting products in logic for {category}", category);

        Activity.Current?.AddEvent(new ActivityEvent("Getting products from repository"));
        var products = await _inMemoryRepository.GetProductsAsync(category);
        // var products = await _distributeCacheRepository.GetProductsAsync(category);

        var results = products.Select(ConvertToProductModel).ToList();

        Activity.Current?.AddEvent(new ActivityEvent("Retrieved products from repository"));

        return results;
    }

    public async Task<ProductModel?> GetProductByIdAsync(int id)
    {
        var product = await _inMemoryRepository.GetProductByIdAsync(id);
        // var product = await _distributeCacheRepository.GetProductByIdAsync(id);
        return product != null ? ConvertToProductModel(product) : null;
    }

    public IEnumerable<ProductModel> GetProductsForCategory(string category)
    {
        var products = _inMemoryRepository.GetProducts(category);
        // var products =  _distributeCacheRepository.GetProducts(category);

        return products.Select(ConvertToProductModel).ToList();
    }

    public ProductModel? GetProductById(int id)
    {
        var product = _inMemoryRepository.GetProductById(id);
        // var product = _distributeCacheRepository.GetProductById(id);
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

    public async Task<ProductModel?> AddNewProductAsync(ProductModel productToAdd, bool invalidateCache)
    {
        var product = new Product
        {
            Category = productToAdd.Category,
            Description = productToAdd.Description,
            ImgUrl = productToAdd.ImgUrl,
            Name = productToAdd.Name,
            Price = productToAdd.Price
        };

        var addedProduct = await _inMemoryRepository.AddNewProductAsync(product, invalidateCache);
        // var addedProduct = await _distributeCacheRepository.AddNewProductAsync(product, invalidateCache);
        
        return addedProduct == null ? null : ConvertToProductModel(addedProduct);
    }
}