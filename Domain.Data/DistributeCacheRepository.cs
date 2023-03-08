using System.Text;
using System.Text.Json;
using Domain.Data.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Domain.Data;

public class DistributeCacheRepository : IDistributeCacheRepository
{
    private readonly IProductRepository _productRepository;
    
    // Implementation of IDistributeCache - Redis, SQL Server, NCache, SQLite
    /*
    Redis via docker
    - docker pull redis
    - docker run -d --name redisDev -p 6379:6379 redis
     
     */
    private readonly IDistributedCache _distributedCache;

    public DistributeCacheRepository(IProductRepository productRepository, IDistributedCache distributedCache)
    {
        _productRepository = productRepository;
        _distributedCache = distributedCache;
    }

    public async Task<List<Product>> GetProductsAsync(string category)
    {
        var cacheKey = $"Products_{category}";
        var productsInBytes = await _distributedCache.GetAsync(cacheKey);
        if (productsInBytes != null)
        {
            return JsonSerializer.Deserialize(Encoding.UTF8.GetString(productsInBytes),
                       CacheSourceGenerationContext.Default.ListProduct)
                   ?? new List<Product>();
        }

        var products = await _productRepository.GetProductsAsync(category);
        var serialized = JsonSerializer.Serialize(products, CacheSourceGenerationContext.Default.ListProduct);

        await _distributedCache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(serialized),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

        return products;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"product_{id}";
        var productInBytes = await _distributedCache.GetAsync(cacheKey);

        if (productInBytes != null)
        {
            return JsonSerializer.Deserialize(Encoding.UTF8.GetString(productInBytes),
                CacheSourceGenerationContext.Default.Product);
        }

        var product = await _productRepository.GetProductByIdAsync(id);

        if (product == null)
        {
            return product;
        }

        var serialized = JsonSerializer.Serialize(product, CacheSourceGenerationContext.Default.Product);

        await _distributedCache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(serialized),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

        return product;
    }

    public List<Product> GetProducts(string category)
    {
        var cacheKey = $"Products_{category}";
        var productsInBytes = _distributedCache.Get(cacheKey);
        if (productsInBytes != null)
        {
            return JsonSerializer.Deserialize(Encoding.UTF8.GetString(productsInBytes),
                       CacheSourceGenerationContext.Default.ListProduct)
                   ?? new List<Product>();
        }

        var products = _productRepository.GetProducts(category);
        var serialized = JsonSerializer.Serialize(products, CacheSourceGenerationContext.Default.ListProduct);

        _distributedCache.Set(cacheKey, Encoding.UTF8.GetBytes(serialized),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

        return products;
    }

    public Product? GetProductById(int id)
    {
        var cacheKey = $"product_{id}";
        var productInBytes = _distributedCache.Get(cacheKey);

        if (productInBytes != null)
        {
            return JsonSerializer.Deserialize(Encoding.UTF8.GetString(productInBytes),
                CacheSourceGenerationContext.Default.Product);
        }


        var product = _productRepository.GetProductById(id);
        if (product == null)
        {
            return product;
        }

        var serialized = JsonSerializer.Serialize(product, CacheSourceGenerationContext.Default.Product);

        _distributedCache.Set(cacheKey, Encoding.UTF8.GetBytes(serialized),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

        return product;
    }

    public async Task<Product?> AddNewProductAsync(Product product, bool invalidateCache)
    {
        var productCreated = await _productRepository.AddNewProductAsync(product, invalidateCache);
        
        if (!invalidateCache || productCreated == null)
        {
            return productCreated;
        }

        var cacheKey = $"products_{product.Category}";
        await _distributedCache.RemoveAsync(cacheKey);

        return productCreated;
    }
}