using Domain.Model;

namespace Domain.Business;

public interface IProductLogic
{
        
    Task<IEnumerable<ProductModel>> GetProductsForCategoryAsync(string category);
    Task<ProductModel?> GetProductByIdAsync(int id);
    IEnumerable<ProductModel> GetProductsForCategory(string category);
    ProductModel? GetProductById(int id);

}