using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Rcwowbagger.BlazorCrud.Models;
using Rcwowbagger.BlazorCrud.Services;

namespace Rcwowbagger.BlazorCrud.Components.Pages;

public class EditableGridv2Base : ComponentBase
{
    [Inject] public ProductService ProductService { get; set; }

    internal RadzenDataGrid<Product> ProductsGrid;
    internal IEnumerable<Product> Products;
    //IEnumerable<Customer> customers;
    //IEnumerable<Employee> employees;

    internal DataGridEditMode editMode = DataGridEditMode.Single;

    internal List<Product> ProductsToInsert = new List<Product>();
    internal List<Product> ProductsToUpdate = new List<Product>();

    protected void Reset()
    {
        ProductsToInsert.Clear();
        ProductsToUpdate.Clear();
    }

    protected void Reset(Product Product)
    {
        ProductsToInsert.Remove(Product);
        ProductsToUpdate.Remove(Product);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();


        Products = await ProductService.GetProductsAsync();
    }

    protected async Task EditRowAsync(Product Product)
    {
        if (editMode == DataGridEditMode.Single && ProductsToInsert.Count() > 0)
        {
            Reset();
        }

        ProductsToUpdate.Add(Product);
        await ProductsGrid.EditRow(Product);
    }

    protected async Task OnUpdateRowAsync(Product Product)
    {
        Reset(Product);

        await ProductService.UpdateProductAsync(Product);

    }

    protected async Task SaveRow(Product Product)
    {
        await ProductsGrid.UpdateRow(Product);
    }

    protected void CancelEdit(Product Product)
    {
        Reset(Product);

        ProductsGrid.CancelEditRow(Product);

        //var ProductEntry = dbContext.Entry(Product);
        //if (ProductEntry.State == EntityState.Modified)
        //{
        //    ProductEntry.CurrentValues.SetValues(ProductEntry.OriginalValues);
        //    ProductEntry.State = EntityState.Unchanged;
        //}
    }

    protected async Task DeleteRow(Product Product)
    {
        Reset(Product);

        if (Products.Contains(Product))
        {
            await ProductService.DeleteProductAsync(Product);

            await ProductsGrid.Reload();
        }
        else
        {
            ProductsGrid.CancelEditRow(Product);
            await ProductsGrid.Reload();
        }
    }

    protected async Task InsertRow()
    {
        if (editMode == DataGridEditMode.Single)
        {
            Reset();
        }

        var Product = new Product();
        ProductsToInsert.Add(Product);
        await ProductsGrid.InsertRow(Product);
    }

    protected async Task OnCreateRow(Product Product)
    {
        ProductService.CreateProductAsync(Product);
        //dbContext.Add(Product);

        //dbContext.SaveChanges();

        ProductsToInsert.Remove(Product);
    }

}
