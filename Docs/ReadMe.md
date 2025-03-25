## 基于JSON 的动态查询描述器

它能够支持任意逻辑条件的组合或嵌套，它与QueryFilterBuilder组合使用可以动态生成Expression<Func<TEntity,bool>>表达式树，供给EFCore 等ORM框架使用，查询条件只需要构造一个Json,无需编写任何代码即可以实现任意的查询。

### 包含节点：

* ##### 字段节点：

```jsonc
//直接字段名
{"$type": "field", "path": "InvoiceId"}
//指定表名，用于多表联合查询
{"$type": "field", "path": "Table.InvoiceId"}
//再指定字段的属性
{"$type": "field", "path": "Table.FirstName.Length"}
```

* ##### 常量节点

```jsonc
{ "$type": "val", "type": "Int32", "value": 1 }
```

* #### 比较表达式节点：

  包括：eq,lt,gt,lte,gte,nan,like,in,btw(Between),pf(Prefix:StartWith),sf(Subfix:EndWith),
* ##### 二元表达式:包括比较、算术表达式：eq、lt、gt、lte、gte，算术表达式：add,sub,mul,div,mod。

```jsonc
{
    "$type": "eq",//可以是eq,lt,gt,lte,gte，add,sub,div,mul,mod
    //left 和 right 两个参数可以是任何返回单值的节点
    "left": { "$type": "field", "path": "InvoiceId" }, 
    "right": { "$type": "val", "type": "Int32", "value": 1 }
}

```

* ##### 一元比较表达式节点:

  包括 nan(IsNull)、in，以及其它返回单值的函数，包括T-SQL函数、EF函数、EF支持的.Net实例和静态函数

```jsonc
{
    "$type": "like",//pf,sf,like
    //operand 和 pattern 两个参数可以是任何返回单值的节点
    "operand": { "$type": "field", "path": "InvoiceId" }, 
    "pattern": { "$type": "val", "type": "Int32", "value": 1 }
}

{
    "$type": "nan",
    //operand参数可以是任何返回单值的节点
    "operand": { "$type": "field", "path": "InvoiceId" }, 
}

{
    "$type": "in",//"func"
    //operand参数可以是任何返回单值的节点
    "operand": { "$type": "field", "path": "InvoiceId" }, 
    "args":[
        //同样的，参数也可以是任何返回单值的节点
        { "$type": "field", "path": "InvoiceId" },
        { "$type": "val", "type": "Int32", "value": 1 }
    ]
}
```

* ##### 函数节点

```jsonc
{
  "$type": "sfunc", //通用函数
  //operand参数可以是任何返回单值的节点
  "operand":{ "$type": "field", "path": "InvoiceId" }, 
  "args":[
      //第一个参数是操作数，任意单值节点
      { "$type": "field", "path": "InvoiceId" },
      //同样的，参数也可以是任何返回单值的节点
      { "$type": "val", "type": "Int32", "value": 1 }
  ]
}
```

* ##### 逻辑条件组节点

```jsonc
  {
    "$type": "cond",
    "logic": "and",
    "body": [  
        //body 可以是任何返回布尔的表达式节点
        //也可以是嵌套的cond条件组节点
    ]
  }
```

* ##### 逻辑非节点

```jsonc
{
    "$type":  "not",  //标准 not  节点
    "body": {        //如果使用标准not节点，body属性是必须的
        "$type": "eq",//可以是任何布尔值节点
        "left": { "$type": "field", "path": "InvoiceId" },
        "right": { "$type": "val", "type": "Int32", "value": 1 }
    }
}

{
    "not": {  //此为省略$type和body写法，直接not 一个比较表达式
    "$type": "gt",
    "left": { "$type": "field", "path": "InvoiceId" },
    "right": { "$type": "val", "type": "Int32", "value": 1 }
    }
}
```

### 以下示例描述能力的展示

```jsonc
{
  "$type": "cond",
  "logic": "and",
  "body": [
      {
      "$type":  "not",  //标准 not  节点
      "body": {        //如果使用标准not节点，body属性是必须的
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
          "body":[//可以是任何逻辑或条件条件组合
                  {"$type": "lt",
                   "left": { "$type": "field", "path": "InvoiceId" },
                  "right": { "$type": "val", "type": "Int32", "value": 1 }
                  }
              ]
         }
      },
      {//此处直接是比较表达式类型
          "$type": "gte",
          "left": { "$type": "field", "path": "InvoiceId" },
          "right": { "$type": "val", "type": "Int32", "value": 1 }
      },
      {//此为nan(isnull)表达式
          "$type": "nan",
          "operand": { "$type": "field", "path": "InvoiceId" }
      },
      {//此为直接是btw表达式
          "$type": "btw", 
          "operand": { "$type": "field", "path": "InvoiceId" },//可以是任何返回单个值的表达式
          "args":[
              {"$type":"field","path":"field_c"}, //可以是任何返回单个值的表达式
              {"$type":"val","type":"system.int32","value":2}//可以是任何返回单个值的表达式
          ]
      },
      {//此为like表达式
          "$type": "like", 
          "operand": { "$type": "field", "path": "InvoiceId" },//可以是任何返回单个值的表达式
          "pattern":{
              "$type": "concat",   //可以是任何返回单个值的表达式
              "left": { "$type": "field", "path": "field_c" },
              "right":{"$type":"val","type":"string","value":2}
          }, "escape":"\\"   //默认值是“\\”可以省略
      },
      {//此为直接是pf(startwith)表达式
          "$type": "pf",
          "operand": { "$type": "field", "path": "InvoiceId" },
          "pattern":{"$type":"val","type":"string","value":2}, //可以是任何返回单个值的表达式
          "escape":"\\"   //默认值是“\\”可以省略
      },
      {//此为直接是sf(endtwith)表达式
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
              "operand":{"$type":"field","path":"table1.Field_a"}, //可以是任何返回单个值的表达式
              "args":[
                      {"$type":"val","type":"int","val":100},//可以是任何返回单个值的表达式
                      {//可以是函数计算
                      "$type":"sfunc", 
                      "name":"round",
                      "args":[
                              {"$type":"field","path":"table1.Field_b"}, //可以是字段
                              {"$type":"val","type":"double","value":1}, //可以是常量
                              {"$type":"func","name":"ceiling", //嵌套函数
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
```
