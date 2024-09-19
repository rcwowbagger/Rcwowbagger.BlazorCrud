using Rcwowbagger.BlazorCrud.Interfaces;
using Serilog;

namespace Rcwowbagger.BlazorCrud.DbPersistence;

public class DatabaseRepository : IDataRepository
{
    private readonly ILogger _logger;

    public DatabaseRepository(DatabaseSettings settings)
    {
        _logger = Log.ForContext<DatabaseRepository>();
    }

    public Task AddAsync<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> GetAsync<T>()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync<T>(T entity, Func<T, string> keySelector)
    {
        throw new NotImplementedException();
    }
}
