using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using DynamicQuery.Descriptor;
using DynamicQuery.Descriptor.Json;

using Xunit.Abstractions;


namespace DynamicQuery.Tests;

    public class TestJsonToDeserialize(ITestOutputHelper output)
    {
        private static JsonSerializerOptions CreateOptions()
        {
            return new JsonSerializerOptions
            {
                Converters = {
                new QueryNodeJsonConverter(),
                new ConstNodeJsonConverter(){  ValueConvertOptions=new JsonValueConvertOptions()} ,
                new IsNotNodeJsonConverter(),
                new JsonStringEnumConverter(),
                new BoolResultNodeJsonConverter(),
            },
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault,
           //,                AllowOutOfOrderMetadataProperties = true
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
        }

        const string JsonAll =
                /*lang=json*/
                """
{
    "$type": "and",
    //"logic": "and",
    "body": [
        {
        "$type":  "not",  //标准 not  节点
        "body": {           //如果使用标准not节点，body属性是必须的
            "$type": "eq", 
            "left": { "$type": "field", "path": "InvoiceId" },
            "right": { "$type": "val", "type": "Int32", "value": 1 }
            }
        },
        {//此为省略$type和body写法，直接not 一个比较表达式
        "not": {        
            "$type": "gt",//comparison:gt
            "left": { "$type": "field", "path": "InvoiceId" },
            "right": { "$type": "val", "type": "Int32", "value": 1 }
            }
        },
        {//此为省略“$type”和"body"属性，直接not 一个条件块
        "not":  {
             "$type":"or",
             //"logic":"or",
            "body":[
                    {"$type": "lt",//comparison:lt
                     "left": { "$type": "field", "path": "InvoiceId" },
                    "right": { "$type": "val", "type": "Int32", "value": 1 }
                    }
                ]
           }
        },
        {//此处直接是比较表达式类型
            "$type": "neq",//comparison:gte
            "left": { "$type": "field", "path": "InvoiceId" },
            "right": { "$type": "val", "type": "Int32", "value": 1 }
        },
        {// eqn/equnll(isnull)表达式
            "$type": "eqn", 
            "operand": { "$type": "field", "path": "InvoiceId" },
        },
        {// btw表达式
            "$type": "btw",  
            "operand": { "$type": "field", "path": "InvoiceId" }, 
            "args":[
                {"$type":"field","path":"field_c"}, 
                {"$type":"val","type":"system.int32","value":2}, 
            ]
        },
        { //like表达式
            "$type": "like",  
            "operand": { "$type": "field", "path": "InvoiceId" }, 
            "pattern":{
                "$type": "concat",   
                "left": { "$type": "field", "path": "field_c" },
                "right":{"$type":"val","type":"string","value":2},  
            }, "escape":"\\"   //默认值是“\\”可以省略
        },
        {// pf(startwith)表达式
            "$type": "pf",    
            "operand": { "$type": "field", "path": "InvoiceId" },  
            "pattern":{"$type":"val","type":"string","value":2}, 
            "escape":"\\"   //默认值是“\\”可以省略
        },
        {//sf(endtwith)表达式
            "$type": "sf",    
            "operand": { "$type": "field", "path": "InvoiceId" }, 
            "pattern":{"$type":"val","type":"string","value":2}, 
            "escape":"\\"   //默认值是“\\”可以省略
        },
        {//以下是嵌套的cond
        "$type":"or",
        //"logic":"or",
        "body":[
                {//此处为省略上层类型的写法
                "$type":"in" ,
                "operand":{"$type":"field","path":"table1.Field_a"},   
                "args":[//operand 可以缩写:opd
                        {"$type":"val","type":"int","val":100},//可以是常量
                        {//可以是函数计算
                        "$type":"svfunc", 
                        "name":"round",
                        "args":[
                                {"$type":"field","path":"table1.Field_b"}, //可以是字段
                                {"$type":"val","type":"double","value":1}, //可以是常量
                                {"$type":"vfunc","name":"ceiling", //可以是值函数
                                     "operand":
                                        {"$type":"sub",//嵌套的计算 
                                                "left":    {"$type":"field","path":"table2.Field_a"},
                                                 "right":{"$type":"div",//嵌套的计算
                                                            "left":    {"$type":"field","path":"table2.Field_b"},
                                                            "right":{"$type":"val","type":"int","val":1}                                                        
                                                    }
                                        },
                                    "args":[
                                        {"$type":"add", 
                                            "left":{"$type":"mod",//嵌套的计算
                                                    "left":  {"$type":"field","path":"table2.Field_a"},
                                                   "right": {"$type":"val","type":"int","val":2}
                                             },
                                            "right":{"$type":"mul",//嵌套的计算                                                
                                                    "left":  {"$type":"field","path":"table2.Field_b"},
                                                    "right":{"$type":"vfunc","name":"float",
                                                        "operand":{"$type":"field","path":"tablec.Field_a"},
                                                        "args":[] //args可以为空，如果没有参数可以省略
                                                    }
                                            }
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        } 
    ]
}
""";

        private readonly ITestOutputHelper _output = output;

        [Fact]
        public void Should_Use_CustomJsonToDeserialize()
        {
            var sw = Stopwatch.StartNew();
            var cond = JsonSerializer.Deserialize<QueryNode>(JsonAll, CreateOptions());
            sw.Stop();
            _output.WriteLine($"Deserialize from custoJson: {sw.ElapsedMilliseconds}ms");
            sw.Restart();
              cond = JsonSerializer.Deserialize<QueryNode>(JsonAll, CreateOptions());
            sw.Stop();
            _output.WriteLine($"Deserialize from custoJson again : {sw.ElapsedMilliseconds}ms");
            sw.Restart();
            var json= JsonSerializer.Serialize(cond, CreateOptions());
            sw.Stop();
            _output.WriteLine($"Serialize : {sw.ElapsedMilliseconds}ms");
            sw.Restart();
             json = JsonSerializer.Serialize(cond, CreateOptions());
            sw.Stop();
            _output.WriteLine($"Serialize again : {sw.ElapsedMilliseconds}ms");
            sw.Restart();
            var cond2 = JsonSerializer.Deserialize<QueryNode>(json, CreateOptions());
            sw.Stop();
            _output.WriteLine($"Deserialize : {sw.ElapsedMilliseconds}ms");
            sw.Restart();
            var json2= JsonSerializer.Serialize(cond2, CreateOptions());
            sw.Stop();
            _output.WriteLine($"Serialize : {sw.ElapsedMilliseconds}ms");
            Assert.Equal(json, json2);
            Assert.Equivalent(cond, cond2);
            _output.WriteLine(json);
        }
    }

