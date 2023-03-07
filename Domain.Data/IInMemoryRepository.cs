﻿using Domain.Data.Entities;

namespace Domain.Data;

public interface IInMemoryRepository
{
    Task<List<Product>> GetProductsAsync(string category);
    Task<Product?> GetProductByIdAsync(int id);

    List<Product> GetProducts(string category);
    Product? GetProductById(int id);
}