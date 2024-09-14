using Serilog;

namespace Rcwowbagger.BlazorCrud.DbPersistence;

public class DbHandler
{
    private readonly ILogger _logger;

    public DbHandler(DbSettings settings)
    {
        _logger = Log.ForContext<DbHandler>();
    }


}
