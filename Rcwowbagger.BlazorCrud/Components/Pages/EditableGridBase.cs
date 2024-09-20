using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Rcwowbagger.BlazorCrud.Interfaces;

namespace Rcwowbagger.BlazorCrud.Components.Pages;

public class EditableGridBase<T> : ComponentBase where T : new()
{
    [Inject] public IDataRepository RepositoryService { get; set; }

    internal RadzenDataGrid<T> ItemGrid;
    internal IEnumerable<T> Items;

    internal DataGridEditMode editMode = DataGridEditMode.Single;

    internal List<T> ItemsToInsert = new();
    internal List<T> ItemsToUpdate = new();

    protected void Reset()
    {
        ItemsToInsert.Clear();
        ItemsToUpdate.Clear();
    }

    protected void Reset(T item)
    {
        ItemsToInsert.Remove(item);
        ItemsToUpdate.Remove(item);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();


        Items = await RepositoryService.GetAsync<T>();
    }

    protected async Task EditRowAsync(T item)
    {
        if (editMode == DataGridEditMode.Single && ItemsToInsert.Any())
        {
            Reset();
        }

        ItemsToUpdate.Add(item);
        await ItemGrid.EditRow(item);
    }

    protected async Task OnUpdateRowAsync(T item)
    {
        Reset(item);

        await RepositoryService.UpdateAsync(item, (t) => t.GetHashCode().ToString());

    }

    protected async Task SaveRow(T item)
    {
        await ItemGrid.UpdateRow(item);
    }

    protected void CancelEdit(T item)
    {
        Reset(item);

        ItemGrid.CancelEditRow(item);

        //var ProductEntry = dbContext.Entry(Product);
        //if (ProductEntry.State == EntityState.Modified)
        //{
        //    ProductEntry.CurrentValues.SetValues(ProductEntry.OriginalValues);
        //    ProductEntry.State = EntityState.Unchanged;
        //}
    }

    protected async Task DeleteRow(T item)
    {
        Reset(item);

        if (Items.Contains(item))
        {
            await RepositoryService.DeleteAsync(item);

            await ItemGrid.Reload();
        }
        else
        {
            ItemGrid.CancelEditRow(item);
            await ItemGrid.Reload();
        }
    }

    protected async Task InsertRow()
    {
        if (editMode == DataGridEditMode.Single)
        {
            Reset();
        }

        T item = new T();
        ItemsToInsert.Add(item);
        await ItemGrid.InsertRow(item);
    }

    protected async Task OnCreateRow(T item)
    {
        RepositoryService.AddAsync(item);
        ItemsToInsert.Remove(item);
    }

}
