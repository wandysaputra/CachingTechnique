using Domain.Data.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.Data;

public class InMemoryRepository : IInMemoryRepository
{
    private readonly IProductRepository _productRepository;
    private readonly IMemoryCache _memoryCache;

    public InMemoryRepository(IProductRepository productRepository, IMemoryCache memoryCache)
    {
        _productRepository = productRepository;
        _memoryCache = memoryCache;
    }

    public async Task<List<Product>> GetProductsAsync(string category)
    {
        var cacheKey = $"Products_{category}";
        if (_memoryCache.TryGetValue<List<Product>>(cacheKey, out var products))
        {
            return products ?? new List<Product>();
        }

        products = await _productRepository.GetProductsAsync(category);
        _memoryCache.Set(cacheKey, products, TimeSpan.FromMinutes(2));

        return products;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"product_{id}";
        if (_memoryCache.TryGetValue<Product?>(cacheKey, out var product))
        {
            return product;
        }

        product = await _productRepository.GetProductByIdAsync(id);
        _memoryCache.Set(cacheKey, product, TimeSpan.FromMinutes(2));

        return product;
    }

    public List<Product> GetProducts(string category)
    {
        var cacheKey = $"Products_{category}";
        if (_memoryCache.TryGetValue<List<Product>>(cacheKey, out var products))
        {
            return products;
        }

        products = _productRepository.GetProducts(category);
        _memoryCache.Set(cacheKey, products, TimeSpan.FromMinutes(2));

        return products;
    }

    public Product? GetProductById(int id)
    {
        var cacheKey = $"product_{id}";
        if (_memoryCache.TryGetValue<Product?>(cacheKey, out var product))
        {
            return product;
        }

        product = _productRepository.GetProductById(id);
        _memoryCache.Set(cacheKey, product, TimeSpan.FromMinutes(2));

        return product;
    }

    public async Task<Product?> AddNewProductAsync(Product product, bool invalidateCache)
    {
        var productCreated = await _productRepository.AddNewProductAsync(product, invalidateCache);
        
        if (!invalidateCache || productCreated == null)
        {
            return productCreated;
        }

        var cacheKey = $"Products_{product.Category}";
        _memoryCache.Remove(cacheKey);

        return productCreated;
    }
}