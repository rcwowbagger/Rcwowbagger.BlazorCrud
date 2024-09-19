using Dapper;
using Microsoft.Data.SqlClient;
using Rcwowbagger.BlazorCrud.DbPersistence.Mappings;
using Rcwowbagger.BlazorCrud.Interfaces;
using Serilog;

namespace Rcwowbagger.BlazorCrud.DbPersistence;

public class DatabaseRepository : IDataRepository
{
    private readonly ILogger _logger;
    private readonly DatabaseSettings _settings;
    private static readonly List<IMapper> _mappers = new List<IMapper>
    {
        new ProductMapper()
    };
    static DatabaseRepository()
    {
        _mappers.ForEach(x => x.ConfigureMapping());
    }

    public DatabaseRepository(DatabaseSettings settings)
    {
        _logger = Log.ForContext<DatabaseRepository>();
        _settings = settings;
    }

    public async Task AddAsync<T>(T entity)
    {
        try
        {
            var mapper = GetMapper<T>();
            using var connection = new SqlConnection(_settings.ConnectionString);
            await connection.OpenAsync();
            var sql = mapper.InsertStatement;
            var result = await connection.ExecuteAsync(sql, entity);

            _logger.Information($"Inserted {{@result}}", result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "");
        }
    }

    public async Task DeleteAsync<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T>> GetAsync<T>()
    {
        try
        {
            var mapper = GetMapper<T>();
            using var connection = new SqlConnection(_settings.ConnectionString);
            await connection.OpenAsync();

            var results = await connection.QueryAsync<T>($"select * from {mapper.TableName}");

            return results;

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "");
            return [];
        }
    }

    public async Task UpdateAsync<T>(T entity, Func<T, string> keySelector)
    {
        throw new NotImplementedException();
    }

    private IMapper GetMapper<T>()
    {
        return _mappers.FirstOrDefault(x => x.Accepts == typeof(T)) ?? throw new NotImplementedException();
    }
}
