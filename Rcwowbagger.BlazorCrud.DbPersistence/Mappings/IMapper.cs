namespace Rcwowbagger.BlazorCrud.DbPersistence.Mappings;

public interface IMapper
{
    Type Accepts { get; }
    string InsertStatement { get; }
    string TableName { get; }
    void ConfigureMapping();
}