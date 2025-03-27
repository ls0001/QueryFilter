using System.Diagnostics;
using System.Text.Json;

using DynamicQuery.Descriptor.Json;

using Xunit.Abstractions;


namespace DynamicQuery.Tests;

public class JsonValueTypeParserTests
{
    private readonly JsonSerializerOptions _options = new();
    private readonly JsonValueConvertOptions _convertOptions = new();

    private ITestOutputHelper _Output;
    public JsonValueTypeParserTests(ITestOutputHelper output)
    {
        _Output =output;
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void ConvertJsonElement_Bool(string json, bool expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
        var result = JsonValueTypeParser.ConvertJsonElement(element, typeof(bool), _convertOptions);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("123.45", 123.45)]
    public void ConvertJsonElement_Number(string json, object expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
        var result = JsonValueTypeParser.ConvertJsonElement(element, expected.GetType(), _convertOptions);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"2023-01-01T00:00:00Z\"", "2023-01-01T00:00:00Z")]
    public void ConvertJsonElement_DateTime(string json, string expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
     var sw=Stopwatch.StartNew();
        var result = JsonValueTypeParser.ConvertJsonElement(element, typeof(DateTime), _convertOptions);
        sw.Stop();
        _Output.WriteLine($"Convert: from string to datetime {sw.ElapsedMilliseconds}ms");
        Assert.Equal(DateTime.Parse(expected,_convertOptions.CultureInfo,_convertOptions.DateTimeStyles), result);
    }

    [Theory]
    [InlineData("\"2023-01-01T00:00:00Z\"", "2023-01-01T00:00:00Z")]
    public void ConvertJsonElement_DateTimeOffset(string json, string expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
        var sw= Stopwatch.StartNew();
        var result = JsonValueTypeParser.ConvertJsonElement(element, typeof(DateTimeOffset), _convertOptions);
        sw.Stop();
        _Output.WriteLine($"Convert string to DateTimeOffset: {sw.ElapsedMilliseconds}ms");
        Assert.Equal(DateTimeOffset.Parse(expected,_convertOptions.CultureInfo,_convertOptions.DateTimeStyles),  result);
    }

    //[Theory]
    //[InlineData("\"PT1H\"", "01:00:00")]
    //public void ConvertJsonElement_TimeSpan(string json, string expected)
    //{
    //    var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
    //    var result = JsonValueTypeParser.ConvertJsonElement(element, typeof(TimeSpan), _convertOptions);
    //    Assert.Equal(TimeSpan.Parse(expected), result);
    //}

    [Theory]
    [InlineData("\"d3b07384-d9a0-4f1b-8b0d-6b1a1a1a1a1a\"", "d3b07384-d9a0-4f1b-8b0d-6b1a1a1a1a1a")]
    public void ConvertJsonElement_Guid(string json, string expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
        var result = JsonValueTypeParser.ConvertJsonElement(element, typeof(Guid), _convertOptions);
        Assert.Equal(Guid.Parse(expected), result);
    }

    [Theory]
    [InlineData("\"One\"", TestEnum.One)]
    [InlineData("1", TestEnum.One)]
    public void ConvertJsonElement_Enum(string json, TestEnum expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
        var result = JsonValueTypeParser.ConvertJsonElement(element, typeof(TestEnum), _convertOptions);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"123\"", 123)]
    [InlineData("\"123.45\"", 123.45)]
    public void ConvertJsonElement_NumericString(string json, object expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
        var result = JsonValueTypeParser.ConvertJsonElement(element, expected.GetType(), _convertOptions);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"Hello, World!\"", "Hello, World!")]
    public void ConvertJsonElement_String(string json, string expected)
    {
        var element = JsonDocument.Parse($"{{\"value\": {json}}}").RootElement.GetProperty("value");
        var result = JsonValueTypeParser.ConvertJsonElement(element, typeof(string), _convertOptions);
        Assert.Equal(expected, result);
    }
}

public enum TestEnum
{
    None = 0,
    One = 1,
    Two = 2
}
