using Dapper;
using Rcwowbagger.BlazorCrud.Interfaces.Models;
using System.Reflection;


namespace Rcwowbagger.BlazorCrud.DbPersistence.Mappings;

public class ProductMapper : IMapper
{
    private static Dictionary<string, string> _columnPropertyMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { "product_id", "Id" },
            { "name", "Name" },
            { "price", "price" },
            { "quantity", "quantity" },
            { "date", "date" }
        };

    private static HashSet<string> _defaults = new()
    {
        "product_id",
        "Id"
    };

    public string TableName => "dbo.Products";
    public void ConfigureMapping()
    {
        SqlMapper.SetTypeMap(typeof(ProductDto), new CustomPropertyTypeMap(
            typeof(ProductDto),
            (type, columnName) =>
            {
                if (_columnPropertyMappings.TryGetValue(columnName, out var propertyName))
                {
                    return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                }
                return null;
            }));
    }

    public string InsertStatement => $"INSERT INTO {TableName} ({string.Join(",", _columnPropertyMappings.Keys.Where(x => !_defaults.Contains(x)))}) VALUES ({string.Join(",", _columnPropertyMappings.Values.Where(x => !_defaults.Contains(x)).Select(x => $"@{x}"))})";

    public Type Accepts => typeof(ProductDto);
}
