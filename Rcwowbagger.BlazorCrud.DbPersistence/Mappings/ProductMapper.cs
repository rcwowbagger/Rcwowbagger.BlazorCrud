using Rcwowbagger.BlazorCrud.Shared.Models;

namespace Rcwowbagger.BlazorCrud.DbPersistence.Mappings;

public class ProductMapper : AbstractMapper
{
    public override string TableName => "[dbo].[Products]";
    public override Type Accepts => typeof(ProductDto);

    public ProductMapper()
        : base(columnPropertyMappings: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "product_id", "Id" },
                { "name", "Name" },
                { "price", "price" },
                { "quantity", "quantity" },
                { "date", "date" }
            },
            keyColumns:
            [
                "product_id"
            ],
            columnsWithIdentity: new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "product_id"
            })
    {
    }

    public override void AssignIdentity(object obj, long? identity)
    {
        if (obj is not ProductDto product)
        {
            throw new InvalidCastException($"Object is not a {nameof(ProductDto)}");
        }
        product.Id = (int)(identity ?? 0);
    }
}