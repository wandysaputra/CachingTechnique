using System.Diagnostics;
using Domain.Data.Context;
using Domain.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Data;

public class ProductRepository : IProductRepository
{
    private readonly LocalContext _ctx;
    private readonly ILogger<IProductRepository> _logger;        
    private readonly ILogger _factoryLogger;

    public ProductRepository(LocalContext ctx, ILogger<ProductRepository> logger, ILoggerFactory factoryLogger)
    {
        _ctx = ctx;
        _logger = logger;
        _factoryLogger = factoryLogger.CreateLogger("DataAccessLayer");
    }
    public async Task<List<Product>> GetProductsAsync(string category)
    {
        _logger.LogInformation("Getting products in repository for {category}", category);
        if (category == "clothing")
        {
            var ex = new ApplicationException("Database error occurred!!");
            ex.Data.Add("Category", category);
            throw ex;
        }
        if (category == "equip")
        {
            throw new SqliteException("Simulated fatal database error occurred!", 551);
        }

        try
        {

            Thread.Sleep(5000);  // simulates heavy query
            return await _ctx.Products.Where(p => p.Category == category || category == "all")
                .Include(p=> p.Rating).ToListAsync();
        } 
        catch (Exception ex)
        {
            var newEx = new ApplicationException("Something bad happened in database", ex);
            newEx.Data.Add("Category", category);
            throw newEx;
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _ctx.Products.FindAsync(id);
    }

    public List<Product> GetProducts(string category)
    {
        return _ctx.Products.Where(p => p.Category == category || category == "all").ToList();
    }

    public Product? GetProductById(int id)
    {
        var timer = new Stopwatch();  
        timer.Start();
            
        var product = _ctx.Products.Find(id);
        timer.Stop();

        _logger.LogDebug("Querying products for {id} finished in {milliseconds} milliseconds", 
            id, timer.ElapsedMilliseconds);	 

        _factoryLogger.LogInformation("(F) Querying products for {id} finished in {ticks} ticks", 
            id, timer.ElapsedTicks);           

        return product;
    }

    
    public async Task<Product?> AddNewProductAsync(Product product, bool invalidateCache)
    {
        _ctx.Products.Add(product);
        var resultSaveChange = await _ctx.SaveChangesAsync();

        return resultSaveChange <= 0 ? null : product;
    }
}