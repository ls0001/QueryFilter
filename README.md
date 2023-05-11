# QueryFilter
基于表达式树的通用查询构造器
常见的使用Linq Expression的做法
单个条件的，单层级的，简单组装，
几乎都是只能组装成如：Field_A =1 and Field_B =2 Or Field_C=3
或者更高级点 Field_A =1 and （Field_B =2 Or Field_C=3）

那么是否可以灵活的查询条件组合
&&描述式的构造条件
&&独立的，分离式的，条件描述与分析，执行无关的
&&适应任何DTO的的方案呢。

我们先来考查一个最简单的Sql were 子句：
Id=1
当中有三种节点，分别为字段名：id，比较符:=，查询值:1
那么数据结构可以设计为
{
        "Name" : "id",
        "Op" : "="
        "Value" : 1
}

再来考查具有两个条件的子句：
Id=1 and name="MyName"
比上一条多了逻辑符：and
那么数据结构可以设计为
[
        {
                "lg" : ""
                filter:{
	                "Name" : "id",
                     "Op" : "="
                     "Value" : 1
	           }
        },
        {
                "lg" : "and"
                filter:{
	                "Name" : "name",
	                "Op" : "="
	                "Value" : "MyName"
                }
        }
]
以下两条子句都比较常用，也都属于简单的子句，再来看稍微复杂点儿的子句：
Id >1 and id<10 and  (name="MyName" or name="HisName")
比上一条多了一对分组括号：
数据结构可以设计为:
{
    "lg": "",
    "filters":
    [
        {
            "lg": "",
            "Predicates":
            [
                {
                    "lg": "",
                    "Name": "id",
                    "Op": ">",
                    "Value": 1
                }
            ]
        },
        {
            "lg": "and",
            "Predicates":
            [
                {
                    "lg": "",
                    "Name": "id",
                    "Op": "<",
                    "Value": 10
                }
            ]
        },

        {
            "lg": "and",
            "Predicates":
            [
                {
                    "lg": "",
                    "Name": "name",
                    "Op": "=",
                    "Value": "MyName"
                },
                {
                    "lg": "or",
                    "Name": "name",
                    "Op": "=",
                    "Value": "HisName"
                }
            ]
        }
    ]
}

再来更复杂一点的：
Id=1 and (
        (name="MyName" or Name="HisName")
   or (Phone like"13899%" and Emil Like "%@xx.com")
)

#基于此，本设计实现：

 where
     	 (
    	   (
    	     (
    	           ((A.Field_A + A.Field_B) / A.Field_C=(2+3) % 2 or A.Field_A= A.Field_C)
    	        or (A.Field_C > 2 and Len(A.Field_D) < 2)
    	     ) 
    	     and (A.Field_E Like"%xxooxx" or A.Field_F in (1,2,3))
    	   ) 
    	   and A.Field_G in (select B.Field_N From B where cast(B.Fieild_DatetimeString as Datetime) = A.FieldDatetime)
	)


