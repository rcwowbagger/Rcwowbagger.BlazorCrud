using Dapper;
using Rcwowbagger.BlazorCrud.Interfaces.Models;
using System.Reflection;


namespace Rcwowbagger.BlazorCrud.DbPersistence.Mappings;

public static class ProductMapper
{

    public static void ConfigureTaskMasterDtoMapping()
    {
        var columnPropertyMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "product_id", "Id" },
            { "name", "Name" },
            { "price", "price" },
            { "quantity", "quantity" },
            { "date", "date" }
        };

        SqlMapper.SetTypeMap(typeof(ProductDto), new CustomPropertyTypeMap(
            typeof(ProductDto),
            (type, columnName) =>
            {
                if (columnPropertyMappings.TryGetValue(columnName, out var propertyName))
                {
                    return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                }
                return null;
            }));
    }

}
