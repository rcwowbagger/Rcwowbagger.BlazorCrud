using Dapper;
using System.Reflection;

namespace Rcwowbagger.BlazorCrud.DbPersistence.Mappings;

public abstract class AbstractMapper
{
    private readonly Dictionary<string, string> _columnPropertyMappings;
    private readonly HashSet<string> _columnsWithIdentity;
    private readonly List<string> _keyColumns;

    public abstract string TableName { get; }
    public abstract Type Accepts { get; }


    public string InsertStatement => $"INSERT INTO {TableName} ({string.Join(", ", _columnPropertyMappings.Where(x => !_columnsWithIdentity.Contains(x.Key)).Select(x => $"[{x.Key}]"))}) VALUES ({string.Join(",", _columnPropertyMappings.Where(x => !_columnsWithIdentity.Contains(x.Key)).Select(x => $"@{x.Value}"))})";
    public string WhereClause => $" {string.Join(" AND ", _keyColumns.Select(x => $"[{x}] = @{_columnPropertyMappings[x]}"))} ";
    public string UpdateAssignment => $" {string.Join(",", _columnPropertyMappings.Where(x => !_keyColumns.Contains(x.Key)).Select(x => $" [{x.Key}] = @{x.Value}"))} ";

    public AbstractMapper(Dictionary<string, string> columnPropertyMappings, List<string> keyColumns, HashSet<string> columnsWithIdentity)
    {
        _columnPropertyMappings = columnPropertyMappings;
        _keyColumns = keyColumns;
        _columnsWithIdentity = columnsWithIdentity;

        if (_keyColumns.Any(x => !_columnPropertyMappings.ContainsKey(x)))
        {
            throw new InvalidDataException("Key columns must be present in column property mappings");
        }
    }

    public void ConfigureMapping()
    {
        SqlMapper.SetTypeMap(Accepts, new CustomPropertyTypeMap(Accepts, (type, columnName) =>
        {
            if (_columnPropertyMappings.TryGetValue(columnName, out var propertyName))
            {
                return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            }
            return null;
        }));
    }

    public bool HasIdentity => _columnsWithIdentity.Any();

    public virtual void AssignIdentity(object obj, long? identity)
    {
        //do nothing
    }
}
