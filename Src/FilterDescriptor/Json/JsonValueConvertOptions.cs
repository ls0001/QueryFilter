using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Globalization;


namespace DynamicQuery.Descriptor.Json;

#region Configuration Options

/// <summary>
/// 时间跨度转换单位
/// </summary>
public enum TimeSpanConversionUnit
{
    Ticks,
    Milliseconds,
    Seconds,
    Minutes,
    Hours,
    Days
}

/// <summary>
/// Unix时间戳单位
/// </summary>
public enum UnixTimestampUnit
{
    Seconds,
    Milliseconds
}
#endregion


/// <summary>
/// 查询配置选项
/// </summary>
public sealed class JsonValueConvertOptions
{

    private IReadOnlyDictionary<string, string> _namespaceAliases =
        ImmutableDictionary<string, string>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);

    private IReadOnlyList<string> _dateTimeFormats =
        ImmutableList.Create(
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy年MM月dd日 HH时mm分ss秒",
            "yyyyMMddHHmmss"
        );

    #region Get Type From Name Cache and MappingList
    private readonly ConcurrentDictionary<string, Type> _typeCache = new(Environment.ProcessorCount, 23);
    internal readonly IReadOnlyDictionary<string, string> _typeNameMapping = new Dictionary<string, string>()
    {
        ["object"] = "object",
        ["obj"] = "object",
        ["null"] = "object",
        ["int"] = "int32",
        ["uint"] = "uint32",
        ["long"] = "int64",
        ["ulong"] = "uint64",
        ["double"] = "double",
        ["decimal"] = "decimal",
        ["float"] = "float",
        ["byte"] = "int8",
        ["sbyte"] = "uint8",
        ["short"] = "int16",
        ["ushort"] = "uint16",
        ["int32"] = "int32",
        ["int64"] = "int64",
        ["uint32"] = "uint32",
        ["uint64"] = "uint64",
        ["int8"] = "int8",
        ["int16"] = "int16",
        ["uint8"] = "uint8",
        ["uint16"] = "uint16",
        ["int128"] = "int128",
        ["uint128"] = "uint128",
        ["bigint"] = "biginteger",
        ["datetime"] = "datetime",
        ["datetimeoffset"] = "datetimeoffset",
        ["timespan"] = "timespan",
        ["dateonly"] = "dateonly",
        ["timeonly"] = "timeonly",
        ["bool"] = "boolean",
        ["boolean"] = "boolean",
        ["string"] = "string",
        ["str"] = "string",
    }.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
    #endregion

    #region ctor

    public JsonValueConvertOptions() { }

    public JsonValueConvertOptions(IReadOnlyList<string> dateTimeFormats)
    {
        _dateTimeFormats = dateTimeFormats;
    }
    public JsonValueConvertOptions(IReadOnlyDictionary<string, string> namespaceAliases)
    {
        _namespaceAliases = namespaceAliases;
    }
    public JsonValueConvertOptions(IReadOnlyDictionary<string, string> namespaceAliases, IReadOnlyList<string> dateTimeFormats)
    {
        _namespaceAliases = namespaceAliases;
        _dateTimeFormats = dateTimeFormats;
    }

    #endregion ctor

    #region Configuration Options
    /// <summary>
    /// 命名空间别名映射（键：别名，值：完整命名空间）
    /// </summary>
    public IReadOnlyDictionary<string, string> NamespaceAliases { get => _namespaceAliases; init => _namespaceAliases = value; }

    /// <summary>
    /// 日期时间解析格式列表
    /// </summary>
    public IReadOnlyList<string> DateTimeFormats { get => _dateTimeFormats; init => _dateTimeFormats = value; }

    /// <summary>
    /// 时间跨度转换单位（默认Ticks）
    /// </summary>
    public TimeSpanConversionUnit TimeSpanUnit { get; set; } = TimeSpanConversionUnit.Ticks;

    /// <summary>
    /// Unix时间戳单位（默认毫秒）
    /// </summary>
    public UnixTimestampUnit UnixTimestampUnit { get; set; } = UnixTimestampUnit.Milliseconds;

    public CultureInfo? CultureInfo { get; set; } = null;

    public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;


    public NumberStyles NumberStyles { get; set; } = NumberStyles.Any;
    /// <summary>
    /// 是否允许自动类型提升（如int到long）
    /// </summary>
    public bool EnableImplicitTypePromotion { get; set; } = true;

    #endregion

    /// <summary>
    /// 更新命名空间别名
    /// </summary>
    public JsonValueConvertOptions AddNamespaceAlias(string alias, string fullNamespace)
    {
        var b = _namespaceAliases.ToDictionary();
        b[alias] = fullNamespace;
        _namespaceAliases = b.ToImmutableDictionary();
        return this;
    }

    /// <summary>
    /// 添加日期时间格式
    /// </summary>
    public JsonValueConvertOptions AddDateTimeFormat(string format)
    {
        var tmpFormats = _dateTimeFormats.ToList();
        tmpFormats.Add(format);
        _dateTimeFormats = tmpFormats.ToImmutableList();
        return this;
    }

    public Type GetTypeWinthName(string typeName)
    {

        var cacheKey = typeName;//$"{typeName}_{options.GetHashCode()}";

        return _typeCache.GetOrAdd(cacheKey, (k) => GetTypeInstanct(typeName));

        Type GetTypeInstanct(string typeName)
        {
            var (namespacePart, namePart) = SplitTypeName(typeName, _typeNameMapping);
            var resolvedNamespace = GetFullNamespace(namespacePart, _namespaceAliases);
            var fullName = $"{resolvedNamespace}.{namePart}";
            var type = Type.GetType(fullName, false, true);
            return type ?? throw new TypeLoadException($"GetTypeWithAliasName: Could not resolve type: {typeName}");
        }

        static (string Namespace, string Name) SplitTypeName(string typeName, IReadOnlyDictionary<string, string> nameMapping)
        {
            var lastDot = typeName.LastIndexOf('.');
            return lastDot == -1
                ? ("System", (string?)nameMapping[typeName] ?? typeName)
                : (typeName[..lastDot], typeName[(lastDot + 1)..]);
        }

        static string GetFullNamespace(string namespacePart, IReadOnlyDictionary<string, string> namespaceAlias)
        {
            if ( string.IsNullOrEmpty(namespacePart) ) return "System";
            return namespaceAlias.TryGetValue(namespacePart, out var fullNs)
                ? fullNs
                : namespacePart;
        }


    }

    public bool TryGetTypeMappedName(string alaisNme, out string? typeName)
    {
        return _typeNameMapping.TryGetValue(alaisNme, out typeName);
    }

}
