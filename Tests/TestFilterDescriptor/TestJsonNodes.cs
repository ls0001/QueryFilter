using System.Text.Json.Serialization;
using System.Text.Json;
using DynamicQuery.Descriptor;

namespace TestDescriptor;


public class QueryNodeTests
{
    private static JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions
        {
            Converters = { 
                new QueryNodeJsonConverter(),
                new ConstNodeJsonConverter(){  valueOptions=new JsonValueConvertOptions()} ,
                new IsNotNodeJsonConverter(),
                new JsonStringEnumConverter()
            },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    #region Common Test Patterns
    public static TheoryData<QueryNode> AllNodeTypes => new()
    {
        // Binary nodes
        new EqualNode { Left = new FieldNode { Path = "age" }, Right = new ConstantNode { TypeName = "int", Value = 25 } },
        new LessThanNode { Left = new FieldNode { Path = "price" }, Right = new ConstantNode { TypeName = "decimal", Value = 99.99m } },
        new AddNode { Left = new FieldNode { Path = "a" }, Right = new FieldNode { Path = "b" } },
        
        // Unary nodes
        new IsNullNode { Operand = new FieldNode { Path = "name" } },
        new NotNode { Body = new EqualNode { Left = new FieldNode { Path = "active" }, Right = new ConstantNode { TypeName = "bool", Value = true } } },
        
        // Complex nodes
        new FuncNode
        {
            Name = "MockFunc",
         Operand=new FieldNode{ Path="sales"},
            Arguments = new QueryNode[]
            {
                new FieldNode { Path = "sales" },
                new ConstantNode { TypeName = "int", Value = 100 }
            }
        },
        CreateNestedConditionNode()
    };

    private static QueryNode CreateNestedConditionNode() => new ConditionNode
    {
        Logic = "and",
        Conditions = new List<QueryNode>
        {
            new ConditionNode
            {
                Logic = "or",
                Conditions = new List<QueryNode>
                {
                    new EqualNode { Left = new FieldNode { Path = "status" }, Right = new ConstantNode { TypeName = "str", Value = "active" } },
                    new GreaterThanNode { Left = new FieldNode { Path = "score" }, Right = new ConstantNode { TypeName = "int", Value = 80 } }
                }
            },
            new LessThanOrEqualNode { Left = new FieldNode { Path = "age" }, Right = new ConstantNode { TypeName = "int", Value = 65 } }
        }
    };
    #endregion

    #region Core Test Logic
    [Theory]
    [MemberData(nameof(AllNodeTypes))]
    public void Should_RoundTrip_AllNodeTypes(QueryNode node)
    {
        var options = CreateOptions();
        var json = JsonSerializer.Serialize(node, options);
        var deserialized = JsonSerializer.Deserialize<QueryNode>(json, options);

        Assert.NotNull(deserialized);
        Assert.Equal(node.GetType(), deserialized.GetType());
    }

    [Fact]
    public void Should_Serialize_ConditionNodeWithNesting()
    {
        // Arrange
        var node = CreateNestedConditionNode();
        var options = CreateOptions();

        // Act
        var json = JsonSerializer.Serialize(node, options);
        var deserialized = JsonSerializer.Deserialize<ConditionNode>(json, options);

        // Assert
        Assert.NotNull(deserialized);
        Assert.IsType<ConditionNode>(deserialized);
        Assert.True(2==deserialized.Conditions.Count);
        Assert.IsType<ConditionNode>(deserialized.Conditions[0]);
    }
    #endregion

    #region Specific Node Type Tests
    [Fact]
    public void Should_Serialize_EqualNode()
    {
        var node = new EqualNode
        {
            Left = new FieldNode { Path = "name" },
            Right = new ConstantNode { TypeName = "string", Value = "John" }
        };

        var json = JsonSerializer.Serialize(node, CreateOptions());
        Assert.Contains("\"$type\":\"eq\"", json);
        Assert.Contains("\"path\":\"name\"", json);
        Assert.Contains("\"value\":\"John\"", json);
    }

    [Fact]
    public void Should_Throw_ForInvalidBetweenNode()
    {
        Assert.Throws<ArgumentException>(() => new BetweenNode()
        {
            Operand = new FieldNode() { Path = "age" },
            Arguments = [new ConstantNode { TypeName = "int", Value = 18 }]
        });
    }

    [Fact]
    public void Should_Handle_ComplexConstantTypes()
    {
        var node = new ConstantNode
        {
            TypeName = "DateTime",
            Value = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var json = JsonSerializer.Serialize(node, CreateOptions());
        var deserialized = JsonSerializer.Deserialize<ConstantNode>(json, CreateOptions());

        Assert.Equal(node.Value, deserialized.Value);
    }
    #endregion

    #region Edge Case Tests
    [Fact]
    public void Should_Handle_NullValues()
    {
        var node = new ConstantNode { TypeName = "string", Value = null };
        var json = JsonSerializer.Serialize(node, CreateOptions());
        Assert.Contains("\"value\":null", json);
    }

    [Fact]
    public void Should_Validate_NotNodeBody()
    {
        Assert.Throws<ArgumentNullException>(() => new NotNode { Body = null });
    }


    [Fact]
    public void Should_Use_CustomConverterForNotNode()
    {
        var node = new NotNode
        {
            Body =
            new ConditionNode
            {
                Logic = "and",
                Conditions = new List<QueryNode>
                {
                        new EqualNode { Left = new FieldNode { Path = "status" }, Right = new ConstantNode { TypeName = "str", Value = "active" } },
                        new GreaterThanNode { Left = new FieldNode { Path = "score" }, Right = new ConstantNode { TypeName = "int", Value = 80 } },
                        new InNode{ Operand= new FieldNode{ Path="id"},
                            Arguments= [
                                new ConstantNode{ TypeName="int", Value=1}, new ConstantNode{ TypeName="int", Value=2} ,
                                new FuncNode{ Name="add",
                                    Operand=new FieldNode{ Path="sales"},
                                    Arguments= [ new ConstantNode{ TypeName="int", Value=100}]}
                                ]
                        } 
                }
            }

        };
        var opt = CreateOptions();
        var json = JsonSerializer.Serialize(node, opt);
        var node2 = JsonSerializer.Deserialize<NotNode>(json, opt);
        var json2 = JsonSerializer.Serialize(node2, opt);
        Assert.Equal(json, json2);
        Assert.Equivalent(node, node2);
    }


    #endregion
}
