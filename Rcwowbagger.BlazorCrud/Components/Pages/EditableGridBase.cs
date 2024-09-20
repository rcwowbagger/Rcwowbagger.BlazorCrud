using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor;
using Rcwowbagger.BlazorCrud.Shared.Interfaces;

namespace Rcwowbagger.BlazorCrud.Components.Pages;

public class EditableGridBase<T> : ComponentBase where T : new()
{
    [Inject] public IDataRepository RepositoryService { get; set; }
    [CascadingParameter] protected Task<AuthenticationState>? AuthState { get; set; }

    internal RadzenDataGrid<T> ItemGrid;
    internal IEnumerable<T> Items;
    internal string User = string.Empty;

    internal int? IdValue { get; set; }

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

        var getTask = RepositoryService.GetAsync<T>();

        await Task.WhenAll(getTask, AuthState);

        Items = await RepositoryService.GetAsync<T>();
        User = (await AuthState).User.Identity?.Name ?? string.Empty;
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

        await RepositoryService.UpdateAsync(item);

    }

    protected async Task SaveRowAsync(T item)
    {
        await ItemGrid.UpdateRow(item);
    }

    protected void CancelEdit(T item)
    {
        Reset(item);
        ItemGrid.CancelEditRow(item);
    }

    protected async Task DeleteRowAsync(T item)
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

    protected async Task InsertRowAsync()
    {
        if (editMode == DataGridEditMode.Single)
        {
            Reset();
        }

        T item = new T();
        ItemsToInsert.Add(item);
        await ItemGrid.InsertRow(item);
    }

    protected async Task OnCreateRowAsync(T item)
    {
        RepositoryService.AddAsync(item);
        ItemsToInsert.Remove(item);
    }

}