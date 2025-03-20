using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DynamicQuery.Descriptor;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestDescriptor
{
    public class TestConverter(ITestOutputHelper output)
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
    "$type": "cond",
    "logic": "and",
    "body": [
        {
        "$type":  "not",  //标准 not  节点
        "body": {           //如果使用标准not节点，body属性是必须的
            "$type": "eq",//comparison:eq,lt,gt,lte,gte,nan,like,in,btw(betwwen),pf(prefix:startwith),sf(subfix:endwith),
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
             "$type":"cond",
             "logic":"or",
            "body":[
                    {"$type": "lt",//comparison:lt
                     "left": { "$type": "field", "path": "InvoiceId" },
                    "right": { "$type": "val", "type": "Int32", "value": 1 }
                    }
                ]
           }
        },
        {//此处直接是比较表达式类型
            "$type": "gte",//comparison:gte
            "left": { "$type": "field", "path": "InvoiceId" },
            "right": { "$type": "val", "type": "Int32", "value": 1 }
        },
        {//此为省略类型assert，直接是nan(isnull)表达式
            "$type": "nan",//comparison:nan(isnull)
            "operand": { "$type": "field", "path": "InvoiceId" },
        },
        {//此为直接是btw表达式
            "$type": "btw",    //comparison:btw(between)//operand 可以缩写为：opd
            "operand": { "$type": "field", "path": "InvoiceId" },//可以是field,val,func,add,sub,mul,div,mod等等
            "args":[
                {"$type":"field","path":"field_c"}, //可以是field,val,func,add,sub,mul,div,mod等等
                {"$type":"val","type":"system.int32","value":2}, 
            ]
        },
        {//此为省略类型assert，直接是like表达式
            "$type": "like",    //comparison:like
            "operand": { "$type": "field", "path": "InvoiceId" },//可以是field,val,func,add,sub,mul,div,mod等等//operand 可以缩写为：opd
            "pattern":{
                "$type": "concat",   //可以是field,val,func,add,sub,mul,div,mod等等
                "left": { "$type": "field", "path": "field_c" },
                "right":{"$type":"val","type":"string","value":2}, //可以是field,val,func,add,sub,mul,div,mod等等
            }, "escape":"\\"   //默认值是“\\”可以省略
        },
        {//此为省略类型assert，直接是pf(startwith)表达式
            "$type": "pf",    //comparison:pf
            "operand": { "$type": "field", "path": "InvoiceId" }, //operand 可以缩写为：opd
            "pattern":{"$type":"val","type":"string","value":2}, //可以是field,val,func,add,sub,mul,div,mod等等
            "escape":"\\"   //默认值是“\\”可以省略
        },
        {//此为省略类型assert，直接是sf(endtwith)表达式
            "$type": "sf",    //comparison:sf
            "operand": { "$type": "field", "path": "InvoiceId" }, //operand 可以缩写为：opd
            "pattern":{"$type":"val","type":"string","value":2}, //可以是field,val,func,add,sub,mul,div,mod等等
            "escape":"\\"   //默认值是“\\”可以省略
        },
        {//以下是嵌套的cond
        "$type":"cond",
        "logic":"or",
        "body":[
                {//此处为省略上层类型的写法
                "$type":"in" ,
                "operand":{"$type":"field","path":"table1.Field_a"},   //可以是字段:field、常量:val、嵌套函数:func  、数学算术：add、sub、mul、div、mod      
                "args":[//operand 可以缩写:opd
                        {"$type":"val","type":"int","val":100},//可以是常量
                        {//可以是函数计算
                        "$type":"sfunc", 
                        "name":"round",
                        "args":[
                                {"$type":"field","path":"table1.Field_b"}, //可以是字段
                                {"$type":"val","type":"double","value":1}, //可以是常量
                                {"$type":"func","name":"ceiling", //可以是函数
                                     "operand":
                                        {"$type":"sub",//嵌套的计算 
                                                "left":    {"$type":"field","path":"table2.Field_a"},
                                                 "right":{"$type":"div",//嵌套的计算
                                                            "left":    {"$type":"field","path":"table2.Field_b"},
                                                            "right":{"$type":"val","type":"int","val":1}                                                        
                                                    }
                                        },
                                    "args":[
                                        {"$type":"add",//可以是add ,sub,mul,div,mod等普通数学计算
                                            "left":{"$type":"mod",//嵌套的计算
                                                    "left":  {"$type":"field","path":"table2.Field_a"},
                                                   "right": {"$type":"val","type":"int","val":2}
                                             },
                                            "right":{"$type":"mul",//嵌套的计算                                                
                                                    "left":  {"$type":"field","path":"table2.Field_b"},
                                                    "right":{"$type":"func","name":"float",
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
}
