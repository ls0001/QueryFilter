using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text.Json;


namespace DynamicQuery.Descriptor.Json;

/// <summary>
/// Json元素值类型处理类（线程安全）
/// </summary>
public sealed class JsonValueTypeParser
{
    #region Constants & Static Fields

    private static readonly JsonValueConvertOptions _defaultOptions = new();

    #endregion

    public static object? ConvertJsonElement(JsonElement element, Type targetType, JsonValueConvertOptions options)
    {
        //options ??= _defaultOptions;
        return element.ValueKind switch
        {
            JsonValueKind.String => HandleStringValue(element, targetType, options),
            JsonValueKind.Number => HandleNumericValue(element, targetType, options),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            //JsonValueKind.Array => ConvertArray(element, targetType, options),
            //JsonValueKind.Object => ConvertObject(element, targetType, options),
            _ => throw new NotSupportedException($"Unsupported JSON value kind: {element.ValueKind}")
        };
    }

    private static object HandleNumericValue(JsonElement element, Type targetType, JsonValueConvertOptions options)
    {
        return targetType switch
        {
            _ when targetType == typeof(bool) => element.GetDecimal() != 0,
            _ when targetType == typeof(DateTime) => ParseNumericDateTime(element, options),
            _ when targetType == typeof(DateTimeOffset) => ParseNumericDateTimeOffset(element, options),
            _ when targetType == typeof(TimeSpan) => ParseNumericTimeSpan(element, options),
            _ when targetType.IsEnum => ParseNumericEnum(element, targetType, options),
            _ => ParseNumeric(element, targetType)
        };
    }

    private static object HandleStringValue(JsonElement element, Type targetType, JsonValueConvertOptions options)
    {
        return ParseString(element.GetString(), targetType, options);
    }

    #region Value Parsing
    private static object ParseNumeric(JsonElement element, Type targetType)
    {
        try
        {
            return Type.GetTypeCode(targetType) switch
            {
                TypeCode.Int32 => element.GetInt32(),
                TypeCode.Int64 => element.GetInt64(),
                TypeCode.Int16 => element.GetInt16(),
                TypeCode.Byte => element.GetByte(),
                TypeCode.SByte => element.GetSByte(),
                TypeCode.UInt32 => element.GetUInt32(),
                TypeCode.UInt64 => element.GetUInt64(),
                TypeCode.UInt16 => element.GetUInt16(),
                TypeCode.Double => element.GetDouble(),
                TypeCode.Single => element.GetSingle(),
                TypeCode.Decimal => element.GetDecimal(),
                _ => ParseLargeNumbers(element, targetType)
            };
        }
        catch ( FormatException ex )
        {
            throw new InvalidOperationException($"Invalid numeric format for {targetType.Name}", ex);
        }
    }

    private static object ParseLargeNumbers(JsonElement element, Type targetType)
    {
        var rawText = element.GetRawText().Trim('"');
        return targetType switch
        {
            _ when targetType == typeof(Int128) => Int128.Parse(rawText, CultureInfo.InvariantCulture),
            _ when targetType == typeof(UInt128) => UInt128.Parse(rawText, CultureInfo.InvariantCulture),
            _ when targetType == typeof(string) => rawText,
            _ => throw new NotSupportedException($"Unsupported numeric type: {targetType.Name}")
        };
    }

    private static DateTime ParseNumericDateTime(JsonElement element, JsonValueConvertOptions options)
    {
        var value = element.GetInt64();
        return options.UnixTimestampUnit == UnixTimestampUnit.Milliseconds
            ? DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime
            : DateTimeOffset.FromUnixTimeSeconds(value).DateTime;
    }

    private static DateTimeOffset ParseNumericDateTimeOffset(JsonElement element, JsonValueConvertOptions options)
    {
        var value = element.GetInt64();
        return options.UnixTimestampUnit == UnixTimestampUnit.Milliseconds
            ? DateTimeOffset.FromUnixTimeMilliseconds(value)
            : DateTimeOffset.FromUnixTimeSeconds(value);
    }

    private static TimeSpan ParseNumericTimeSpan(JsonElement element, JsonValueConvertOptions options)
    {
        var value = element.GetInt64();
        return options.TimeSpanUnit switch
        {
            TimeSpanConversionUnit.Ticks => TimeSpan.FromTicks(value),
            TimeSpanConversionUnit.Milliseconds => TimeSpan.FromMilliseconds(value),
            TimeSpanConversionUnit.Seconds => TimeSpan.FromSeconds(value),
            TimeSpanConversionUnit.Minutes => TimeSpan.FromMinutes(value),
            TimeSpanConversionUnit.Hours => TimeSpan.FromHours(value),
            TimeSpanConversionUnit.Days => TimeSpan.FromDays(value),
            _ => throw new ArgumentOutOfRangeException(nameof(options.TimeSpanUnit))
        };
    }

    private static object ParseString(string value, Type targetType, JsonValueConvertOptions options)
    {
        return targetType switch
        {
            _ when targetType == typeof(bool) => ParseBoolean(value),
            _ when targetType == typeof(DateTime) => ParseDateTime(value, options),
            _ when targetType == typeof(DateTimeOffset) => ParseDateTimeOffset(value, options),
            _ when targetType == typeof(TimeSpan) => ParseTimeSpan(value, options),
            _ when targetType == typeof(Guid) => Guid.Parse(value),
            _ when targetType.IsEnum => ParseStringEnum(value, targetType),
            _ when IsNumericType(targetType) => ParseNumericString(value, targetType),
            _ => value
        };

        static object ParseNumericString(string value, Type targetType)
        {
            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch ( Exception ex )
            {
                throw new InvalidOperationException($"Failed to parse numeric string '{value}' as {targetType.Name}", ex);
            }
        }

        static object ParseStringEnum(string value, Type enumType)
        {
            if ( int.TryParse(value, out var numeric) )
                return Enum.ToObject(enumType, numeric);

            return Enum.GetNames(enumType)
                .FirstOrDefault(n => n.Equals(value, StringComparison.OrdinalIgnoreCase)) != null
                ? Enum.Parse(enumType, value, true)
                : throw new InvalidOperationException($"Invalid enum value '{value}' for type {enumType.Name}");
        }



    }

    private static object ParseNumericEnum(JsonElement valueElement, Type enumType, JsonValueConvertOptions options)
    {
        var underlyingType = enumType.GetEnumUnderlyingType();
        object num = underlyingType switch
        {
            _ when underlyingType == typeof(int) => valueElement.GetInt32(),
            _ when underlyingType == typeof(uint) => valueElement.GetUInt32(),
            _ when underlyingType == typeof(short) => valueElement.GetInt16(),
            _ when underlyingType == typeof(ushort) => valueElement.GetUInt16(),
            _ when underlyingType == typeof(sbyte) => valueElement.GetSByte(),
            _ when underlyingType == typeof(byte) => valueElement.GetByte(),
            _ when underlyingType == typeof(long) => valueElement.GetInt64(),
            _ when underlyingType == typeof(ulong) => valueElement.GetUInt64(),
            _ => throw new InvalidCastException($"The JsonElement:{valueElement} Invalid cast to type {enumType}")
        };

        return Enum.ToObject(enumType, num);

    }

    private static DateTime ParseDateTime(string value, JsonValueConvertOptions options)
    {
        // 尝试解析为Unix时间戳
        if ( long.TryParse(value, options.NumberStyles, options.CultureInfo, out var number) )
        {
            return options.UnixTimestampUnit == UnixTimestampUnit.Milliseconds
                ? DateTimeOffset.FromUnixTimeMilliseconds(number).DateTime
                : DateTimeOffset.FromUnixTimeSeconds(number).DateTime;
        }

        // 尝试所有配置的日期格式
        foreach ( var format in options.DateTimeFormats )
        {
            if ( DateTime.TryParseExact(value, format, options.CultureInfo,
                options.DateTimeStyles, out var result) )
            {
                return result;
            }
        }

        // 最后尝试通用解析
        return DateTime.Parse(value, options.CultureInfo, options.DateTimeStyles);
    }

    private static DateTimeOffset ParseDateTimeOffset(string value, JsonValueConvertOptions options)
    {
        if ( long.TryParse(value, out var number) )
        {
            return options.UnixTimestampUnit == UnixTimestampUnit.Milliseconds
                ? DateTimeOffset.FromUnixTimeMilliseconds(number)
                : DateTimeOffset.FromUnixTimeSeconds(number);
        }

        foreach ( var format in options.DateTimeFormats )
        {
            if ( DateTimeOffset.TryParseExact(value, format, options.CultureInfo,
                options.DateTimeStyles, out var result) )
            {
                return result;
            }
        }

        return DateTimeOffset.Parse(value, options.CultureInfo, options.DateTimeStyles);
    }

    private static TimeSpan ParseTimeSpan(string value, JsonValueConvertOptions options)
    {
        if ( long.TryParse(value, out var number) )
        {
            return options.TimeSpanUnit switch
            {
                TimeSpanConversionUnit.Ticks => TimeSpan.FromTicks(number),
                TimeSpanConversionUnit.Milliseconds => TimeSpan.FromMilliseconds(number),
                TimeSpanConversionUnit.Seconds => TimeSpan.FromSeconds(number),
                TimeSpanConversionUnit.Minutes => TimeSpan.FromMinutes(number),
                TimeSpanConversionUnit.Hours => TimeSpan.FromHours(number),
                TimeSpanConversionUnit.Days => TimeSpan.FromDays(number),
                _ => throw new ArgumentOutOfRangeException(nameof(options.TimeSpanUnit))
            };
        }

        return TimeSpan.Parse(value, options.CultureInfo);
    }

    private static bool ParseBoolean(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "true" or "1" or "yes" => true,
            "false" or "0" or "no" => false,
            _ => throw new FormatException($"Invalid boolean value: {value}")
        };
    }
    #endregion

    #region Helper Methods

    private static bool IsNumericType(Type type)
    {
        return type switch
        {
            Type t when IsNumeric(t) => true,
            Type t when Nullable.GetUnderlyingType(t) is Type u && IsNumeric(u) => true,
            _ => false
        };

        static bool IsNumeric(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            if ( (typeCode >= TypeCode.SByte && TypeCode.Decimal >= typeCode) || (type == typeof(Int128) || type == typeof(UInt128) || type == typeof(BigInteger)) )
            {
                return true;
            }
            return false;
        }

    }

    #endregion

}


