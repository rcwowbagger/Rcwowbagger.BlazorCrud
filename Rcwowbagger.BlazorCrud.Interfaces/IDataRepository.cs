namespace Rcwowbagger.BlazorCrud.Interfaces
{
    public interface IDataRepository
    {
        Task<IEnumerable<T>> GetAsync<T>();
        Task UpdateAsync<T>(T entity, Func<T,string> keySelector);
        Task DeleteAsync<T>(T entity);
        Task AddAsync<T>(T entity);
    }
}
