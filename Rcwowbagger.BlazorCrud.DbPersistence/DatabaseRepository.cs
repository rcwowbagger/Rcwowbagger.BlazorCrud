using Dapper;
using Microsoft.Data.SqlClient;
using Rcwowbagger.BlazorCrud.DbPersistence.Mappings;
using Rcwowbagger.BlazorCrud.Shared.Interfaces;
using Serilog;
using static Dapper.SqlMapper;

namespace Rcwowbagger.BlazorCrud.DbPersistence;

public class DatabaseRepo : IDataRepository
{
    private readonly ILogger _logger;
    private readonly DatabaseSettings _settings;
    private static readonly List<AbstractMapper> _mappers = new()
    {
        new ProductMapper()
    };
    static DatabaseRepo()
    {
        _mappers.ForEach(x => x.ConfigureMapping());
    }

    public DatabaseRepo(DatabaseSettings settings)
    {
        _logger = Log.ForContext<DatabaseRepo>();
        _settings = settings;
    }

    public async Task AddAsync<T>(T entity)
    {
        try
        {
            var mapper = GetMapper<T>();
            var sql = mapper.InsertStatement;
            var result = await ExecuteAsync(sql, entity);

            if (result > 0 && mapper.HasIdentity)
            {
                var identity = await GetIdentity();
                mapper.AssignIdentity(entity, identity);
            }

            _logger.Information($"Inserted: {{@result}}", result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"{{@entity}}", entity);
        }
    }

    public async Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> keySelector = null)
    {
        try
        {
            var mapper = GetMapper<T>();
            using var connection = new SqlConnection(_settings.ConnectionString);
            await connection.OpenAsync();

            var results = await connection.QueryAsync<T>($"select * from {mapper.TableName}");
            return keySelector is not null
                ? (results?.Where(x => keySelector(x)))
                : results;

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "");
            return [];
        }
    }

    public async Task DeleteAsync<T>(T entity)
    {
        try
        {
            var mapper = GetMapper<T>();
            var sql = $"DELETE FROM {mapper.TableName} WHERE {mapper.WhereClause}";

            var result = await ExecuteAsync(sql, entity);

            _logger.Information($"Deleted: {{@result}}", result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"{{@entity}}", entity);
        }
    }


    public async Task UpdateAsync<T>(T entity)
    {
        try
        {
            var mapper = GetMapper<T>();

            var sql = $"UPDATE {mapper.TableName} SET {mapper.UpdateAssignment} WHERE {mapper.WhereClause}";

            var result = await ExecuteAsync(sql, entity);

            _logger.Information($"Updated {{@result}}", result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"{{@entity}}", entity);
        }
    }

    private async Task<int> ExecuteAsync<T>(string sql, T entity)
    {
        try
        {
            using var connection = new SqlConnection(_settings.ConnectionString);
            await connection.OpenAsync();

            var result = await connection.ExecuteAsync(sql, entity);

            _logger.Information($"Result: {{@result}}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"{sql} {{@entity}}", entity);
            return 0;
        }
    }

    private AbstractMapper GetMapper<T>()
    {
        return _mappers.FirstOrDefault(x => x.Accepts == typeof(T)) ?? throw new NotImplementedException();
    }

    private async Task<long?> GetIdentity()
    {
        try
        {
            using var connection = new SqlConnection(_settings.ConnectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<long?>("select SCOPE_IDENTITY()");
            return result.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "");
            return null;
        }
    }

}