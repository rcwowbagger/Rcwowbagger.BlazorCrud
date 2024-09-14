using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Rcwowbagger.BlazorCrud.Models;
using Rcwowbagger.BlazorCrud.Services;

namespace Rcwowbagger.BlazorCrud.Components.Pages;

public class EditableGridBase : ComponentBase
{
    [Inject] public ProductService ProductService { get; set; }
    internal RadzenDataGrid<Product> Grid { get; set; }
    internal List<Product> Products { get; private set; }
    internal DataGridEditMode editMode = DataGridEditMode.Single;

    protected override async Task OnInitializedAsync()
    {
        Products = await ProductService.GetProductsAsync();
    }

    protected async Task OnUpdateRow(Product product)
    {
        // Validate the product (optional)

        // Update the product in the data store
        await ProductService.UpdateProductAsync(product);

        // Optionally, refresh the grid or handle any post-update logic
    }

    protected async Task OnCreateRow(Product product)
    {
        // Validate the product (optional)
        await ProductService.CreateProductAsync(product);

    }

}
