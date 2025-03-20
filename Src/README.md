## 关于QueryFilterBuilder


[更多详情请戳这里,查看连载](https://www.cnblogs.com/ls0001/p/17395510.html) 


基于表达式树的通用查询构造器 常见的使用**Linq Expression**的做法 单个条件的，单层级的，简单组装， 几乎都是只能组装成如：
```sql
Field_A =1 and Field_B =2 Or Field_C=3 
--或者 
Field_A =1 and （Field_B =2 Or Field_C=3）
```

是否可以<u>灵活的查询条件组合</u> &独立的分离式的描述式条件，描述与执行分离的 &适应任何DTO的通用方案呢。

我们先来考查一个最简单的`Sql where` 子句： `Id=1` 当中有三种节点，分别为字段名：**id**，比较符:**=**，查询值:**1** 通过观察，我们用`Name`来标识字段名，`Op`来标识比较符，`Value`来标识查询值，那么数据结构可以设计为 ：
```json
{ "Name" : "id", "Op" : "=", "Value" : 1 }
```

再来考查具有两个条件的子句： 
```sql
Id=1 and name="MyName" 
```

比上一条多了逻辑符：**“and”**，

那么我们添加两个节点，用`Predicates`来描述查询条件，用`Lg`来标识逻辑串接，数据结构可以设计为
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
 比上一条多了一对*分组括号*： 添加节点`filters`来容纳逻辑组合，数据结构可以设计为: 
```json
 { 
     "lg": "", 
     	"filters":
	[ 
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

