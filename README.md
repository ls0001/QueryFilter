## 关于QueryFilterBuilder

[更多详情请戳这里,查看连载](https://www.cnblogs.com/ls0001/p/17395510.html)

基于表达式树的通用查询构造器 常见的使用**Linq Expression**的做法 单个条件的，单层级的，简单组装， 几乎都是只能组装成如：

```sql
Field_A =1 and Field_B =2 Or Field_C=3 
--或者 
Field_A =1 and （Field_B =2 Or Field_C=3）
```

是否可以`<u>`灵活的查询条件组合`</u>` &独立的分离式的描述式条件，描述与执行分离的 &适应任何DTO的通用方案呢。

我们先来考查一个最简单的 `Sql where` 子句： `Id=1` 当中有三种节点，分别为字段名：**id**，比较符:**=**，查询值:**1** 通过观察，我们用 `Name`来标识字段名，`Op`来标识比较符，`Value`来标识查询值，那么数据结构可以设计为 ：

```json
{ "Name" : "id", "Op" : "=", "Value" : 1 }
```

再来考查具有两个条件的子句：

```sql
Id=1 and name="MyName" 
```

比上一条多了逻辑符：**“and”**，
那么我们添加两个节点，用 `Predicates`来描述查询条件，用 `Lg`来标识逻辑串接，数据结构可以设计为

```json
 [ 
     {
      	"lg" : "",
      	"Predicates" : {"Name" : "id", "Op" : "=", "Value" : 1 }
     },
     {
        "lg":"and",
      	"Predicates": { "Name" : "Name", "Op" : "=", "Value" : "MyName"}
     } 
 ] 
```

以下两条子句都比较常用，也都属于简单的子句，再来看稍微复杂点儿的子句：

```sql
Id>1 and Id<10 and (Name="MyName" or Name="HisName")
```

 比上一条多了一对*分组括号*： 添加节点 `filters`来容纳逻辑组合，数据结构可以设计为:

```json
 { 
    "lg": "", 
    "filters":[
        {  	
            "lg": "", 
            "Predicates":[ { "lg": "", "Name": "id", "Op": ">", "Value": 1 } ]
        }, 
        { 
        "lg": "and", 
            "Predicates":[ { "lg": "", "Name": "id", "Op": "<", "Value": 10 } ] 
        },
        {
            "lg": "and",
            "Predicates":
            [
                {"lg": "","Name": "name",  "Op": "=", "Value": "MyName"},       
                {"lg": "or","Name": "name","Op": "=","Value": "HisName"}
            ]
        }
    ]
}
```

```sql
--更复杂一点的等等：
Id=1 
 and (
     (Name="MyName" or Name="HisName") or (Phone like"13899%" and Emil Like "%@xx.com")
 )
```

**基于此设计，能够以简洁的方式：**

```c#
Query.Where(QueryFilterBuilder.CreateFilterExpression<Entity>(conditionBlock));
```

**实现如下复杂的查询组合：**

```sql
where 
    ( 
        (          
            (A.Field_A + A.Field_B) / A.Field_C=(2+3) % 2 or A.Field_A= A.Field_C
			or (A.Field_C > 2 and Len(A.Field_D) < 2)
        ) 
        and (A.Field_E Like"%xxooxx" or A.Field_F in (1,2,3))
    ) 
    and A.Field_G in (select B.Field_N From B where cast(B.Fieild_DatetimeString as Datetime) = A.FieldDatetime)
)
```

## 使用示例：

* [ ] **相等条件**

```c#
var equal=new EqualNode(new FieldNode(nameof(Invoice.InvoiceId)),new ConstantNode<int>(1));
var where=Builder.BuildFilter<TestEntity>(equal);
TestEntitys.Where(where).Dump("equal");
/*lambda:
DbSet<Invoice>()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    })  
    .Where(TestEntity => TestEntity.InvoiceId == (long)ConstantNode<int>.Value)

*sql:
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE "i"."InvoiceId" = @__p_0
*/
```

* [ ] **嵌套函数调用**

```C#
var funcCall=new EqualNode( new ValueFuncNode("Substring", 
new FieldNode(nameof(TestEntity.BillingCity)),[new ConstantNode<int>(1),new ConstantNode<int>(4)]),
new ConstantNode<string>("rank"));
where=Builder.BuildFilter<TestEntity>(funcCall );
TestEntitys.Where(where).Dump("substring");
/*
lambda:
DbSet<Invoice>()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    }
    )
    .Where(TestEntity => TestEntity.BillingCity.Substring(
        startIndex: ConstantNode<int>.Value,
        length: ConstantNode<int>.Value) == ConstantNode<string>.Value)
	
*sql:
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE substr("i"."BillingCity", @__Value_0 + 1, @__Value_1) = @__Value_2
*/
```

* [ ] **Like查询**
```C#
var like=new LikeNode(new FieldNode(nameof(TestEntity.BillingCity)),new ConstantNode`<string>`("gart"));
where = Builder.BuildFilter`<TestEntity>`(like);
TestEntitys.Where(where).Dump("like");
/*lambda:
DbSet`<Invoice>`()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    }
    )
    .Where(TestEntity => TestEntity.BillingCity.Contains(ConstantNode`<string>`.Value))

*sql:
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE instr("i"."BillingCity", @__Value_0) > 0
*/
```

* [ ] **StartWidth查询**
```C#
var startWith=new StartWithNode(new FieldNode(nameof(TestEntity.BillingCity)),new ConstantNode`<string>`("Stutt"));
where = Builder.BuildFilter`<TestEntity>`(startWith);
TestEntitys.Where(where).Dump("startwith");
/*lambda:
DbSet`<Invoice>`()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    }
    )
    .Where(TestEntity => TestEntity.BillingCity.StartsWith(ConstantNode`<string>`.Value))

*sql:
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE "i"."BillingCity" LIKE @__Value_0_startswith ESCAPE '\'
*/
```

* [ ] **Between范围查询**
```C#
var betweenIn=new BetweenNode(new FieldNode(nameof(TestEntity.InvoiceDate)),
[new ConstantNode`<DateTime>`(DateTime.Parse("2009/1/3")),
new ConstantNode`<DateTime>`(DateTime.Parse("2009/1/30"))]);
where=Builder.BuildFilter`<TestEntity>`(betweenIn);
TestEntitys.Where(where).Dump("between");
/*lambda:
DbSet`<Invoice>`()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    }
    )
    .Where(TestEntity => TestEntity.InvoiceDate >= ConstantNode`<DateTime>`.Value && TestEntity.InvoiceDate <= ConstantNode`<DateTime>`.Value)

*sql:
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE "i"."InvoiceDate" >= @__Value_0 AND "i"."InvoiceDate" <= @__Value_1
*/
```

* [ ] **In [...]查询**
```C#
var exists = new InNode(new FieldNode(nameof(TestEntity.InvoiceId)), [new ConstantNode`<int>`(2)]);
where=Builder.BuildFilter<TestEntity>(exists);
TestEntitys.Where(where).Dump("in [...]");
/*
lambda:
DbSet<Invoice>()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    }
    )
    .Where(TestEntity => new ParameterizeValue<long[]>{ Value = new long[]{ (long)ConstantNode<int>.Value } }
    .Value
        .Contains(TestEntity.InvoiceId))
*sql:
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE "i"."InvoiceId" IN (
    SELECT "v"."value"
    FROM json_each(@__Value_0) AS "v"
)
*/
```

* [ ] **And 逻辑组合查询**
	***节点子条件可以嵌套任何表示boolean的子节点，包括比较节点、boolean函数节点、and和or逻辑节点、condition复杂条件节点***
```C#
var and = new AndNode() {
 Body= {
 	equal,
	new OrNode(){Body={like,exists}},
 }
};
where=Builder.BuildFilter`<TestEntity>`(and);
TestEntitys.Where(where).Dump("id in[2]");
/*
lambda:
DbSet`<Invoice>`()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    }
    )
    .Where(TestEntity => TestEntity.InvoiceId == (long)ConstantNode`<int>`.Value && TestEntity.BillingCity.Contains(ConstantNode`<string>`.Value) || new ParameterizeValue<long[]>{ Value = new long[]{ (long)ConstantNode`<int>`.Value } }
    .Value
        .Contains(TestEntity.InvoiceId))

sql:	
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE "i"."InvoiceId" = @__p_0 AND (instr("i"."BillingCity", @__Value_1) > 0 OR "i"."InvoiceId" IN (
    SELECT "v"."value"
    FROM json_each(@__Value_2) AS "v"
))
*/
```

* [ ] **复杂组件条件查询**
***嵌套使用示例***
```C#
var cond=new CondNode(){
	new AndNode(){Body=[equal,like,startWith]},
	new OrNode(){Body=[exists,betweenIn,and]}
};
where=Builder.BuildFilter`<TestEntity>`(cond);
TestEntitys.Where(where).Dump();
/*lambda:
DbSet`<Invoice>`()
    .Select(x => new TestEntity{
        InvoiceId = x.InvoiceId,
        BillingCity = x.BillingCity,
        InvoiceDate = x.InvoiceDate
    }
    )
    .Where(TestEntity => TestEntity.InvoiceId == (long)ConstantNode`<int>`.Value && TestEntity.BillingCity.Contains(ConstantNode`<string>`.Value) && TestEntity.BillingCity.StartsWith(ConstantNode`<string>`.Value) || new ParameterizeValue<long[]>{ Value = new long[]{ (long)ConstantNode`<int>`.Value } }
    .Value
        .Contains(TestEntity.InvoiceId) || TestEntity.InvoiceDate >= ConstantNode`<DateTime>`.Value && TestEntity.InvoiceDate <= ConstantNode`<DateTime>`.Value || TestEntity.InvoiceId == (long)ConstantNode`<int>`.Value && TestEntity.BillingCity.Contains(ConstantNode`<string>`.Value) || new ParameterizeValue<long[]>{ Value = new long[]{ (long)ConstantNode`<int>`.Value } }
    .Value
        .Contains(TestEntity.InvoiceId))

*sql:
SELECT "i"."InvoiceId", "i"."BillingCity", "i"."InvoiceDate"
FROM "Invoice" AS "i"
WHERE ("i"."InvoiceId" = @__p_0 AND instr("i"."BillingCity", @__Value_1) > 0 AND "i"."BillingCity" LIKE @__Value_2_startswith ESCAPE '\') OR "i"."InvoiceId" IN (
    SELECT "v"."value"
    FROM json_each(@__Value_3) AS "v"
) OR ("i"."InvoiceDate" >= @__Value_4 AND "i"."InvoiceDate" <= @__Value_5) OR ("i"."InvoiceId" = @__p_0 AND (instr("i"."BillingCity", @__Value_1) > 0 OR "i"."InvoiceId" IN (
    SELECT "v0"."value"
    FROM json_each(@__Value_3) AS "v0"
)))
*/
```