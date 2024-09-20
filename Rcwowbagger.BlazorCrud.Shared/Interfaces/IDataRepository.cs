namespace Rcwowbagger.BlazorCrud.Shared.Interfaces;

public interface IDataRepository
{
    Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> keySelector = null);
    Task AddAsync<T>(T entity);
    Task UpdateAsync<T>(T entity);
    Task DeleteAsync<T>(T entity);
}
