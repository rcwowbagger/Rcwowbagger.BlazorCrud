using Rcwowbagger.BlazorCrud.Models;

namespace Rcwowbagger.BlazorCrud.Services;

public class ProductService
{
    private List<Product> products = new List<Product>
    {
        new Product{ Id = 1, Name = "Product A", Price = 10.00m, Quantity = 100 },
        new Product{ Id = 2, Name = "Product B", Price = 15.50m, Quantity = 200 },
        // Add more initial products as needed
    };

    public Task<List<Product>> GetProductsAsync()
    {
        return Task.FromResult(products);
    }
    
    public Task UpdateProductAsync(Product product)
    {
        var index = products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            products[index] = product;
        }
        return Task.CompletedTask;
    }

    public Task CreateProductAsync(Product product)
    {
        products.Add(product);
        return Task.CompletedTask;
    }

    public Task DeleteProductAsync(Product product)
    {
        products.Remove(product);

        return Task.CompletedTask;
    }
}

