using Domain.Data.Entities;

namespace Domain.Data;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync(string category);
    Task<Product?> GetProductByIdAsync(int id);

    List<Product> GetProducts(string category);
    Product? GetProductById(int id);
    Task<Product?> AddNewProductAsync(Product product, bool invalidateCache);
}