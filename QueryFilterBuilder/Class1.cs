#define if
#if false

#region 方法


Ascept(expressionVisitor visitor)

    对本节点进行访问的访问者,调度到本节点类型的特定visit方法

#region new    创建一个 NewExpression。(new sampleType())
    New(ConstructorInfo)
    创建一个表示调用不带参数的指定构造函数的 NewExpression。
    New(Type)
    创建一个表示调用指定类型的无参数构造函数的 NewExpression。
    New(ConstructorInfo, IEnumerable<Expression>)
    创建一个表示调用带指定参数的指定构造函数的 NewExpression。    
    New(ConstructorInfo, Expression[])
    创建一个表示调用带指定参数的指定构造函数的 NewExpression。
    New(ConstructorInfo, IEnumerable<Expression>, IEnumerable<MemberInfo>)
    创建一个表示调用带指定参数的指定构造函数的 NewExpression。 其中指定了访问构造函数初始化的字段的成员。
    New(ConstructorInfo, IEnumerable<Expression>, MemberInfo[])
    创建一个表示调用带指定参数的指定构造函数的 NewExpression。 将访问构造函数初始化字段的成员指定为数组。
    参数
    constructor     ConstructorInfo 
    要将 ConstructorInfo 属性设置为与其相等的 Constructor。
    arguments   IEnumerable<Expression> 
    一个 IEnumerable<T>，包含用来填充 Expression 集合的 Arguments 对象。
    type    Type 
    一个具有不带自变量的构造函数的 Type。
    members IEnumerable<MemberInfo> 
    一个 IEnumerable<T>，包含用来填充 MemberInfo 集合的 Members 对象。 

    示例
    下面的示例演示如何使用New(Type)方法创建一个NewExpression , 它表示通过调用不带参数的构造函数来构造字典对象的新实例。     // Create a NewExpression that represents constructing
    // a new instance of Dictionary<int, string>.
    System.Linq.Expressions.NewExpression newDictionaryExpression =
        System.Linq.Expressions.Expression.New(typeof(Dictionary<int, string>));

    Console.WriteLine(newDictionaryExpression.ToString());

    // This code produces the following output:
    //
    // new Dictionary`2()

    返回
    NewExpression 
    一个 NewExpression，其 NodeType 属性等于 New，


#endregion

#region NewArrayBounds
    NewArrayBounds(Type, Expression[]) 
    创建一个表示创建具有指定秩的数组的 NewArrayExpression。
    NewArrayBounds(Type, IEnumerable<Expression>)
    创建一个表示创建具有指定秩的数组的 NewArrayExpression。
    参数
    type     Type 
    一个表示数组的元素类型的 Type。
    bounds    Expression[] 
    用于填充 Expression 集合的 Expressions 对象的数组。
    bounds    IEnumerable<Expression> 
    一个 IEnumerable<T>，包含用来填充 Expression 集合的 Expressions 对象。

    示例
    下面的示例演示如何使用NewArrayBounds方法创建表达式树, 该表达式树表示创建排名为2的字符串数组。 
    // Create an expression tree that represents creating a 
    // two-dimensional array of type string with bounds [3,2].
    System.Linq.Expressions.NewArrayExpression newArrayExpression =
        System.Linq.Expressions.Expression.NewArrayBounds(
                typeof(string),
                System.Linq.Expressions.Expression.Constant(3),
                System.Linq.Expressions.Expression.Constant(2));

    // Output the string representation of the Expression.
    Console.WriteLine(newArrayExpression.ToString());

    // This code produces the following output:
    //
    // new System.String[,](3, 2)

    注解
    bounds type所得Type 的属性表示一个数组类型,其秩等于的长度,其元素类型为。NewArrayExpression 
    的每个bounds元素的属性必须表示整数类型

#endregion NewArrayBounds

#region NewArrayInit (new sampleType[]{a,b,c})
    NewArrayInit(Type, IEnumerable<Expression>)
    NewArrayInit(Type, Expression[])
    创建一个表示创建一维数组并使用元素列表初始化该数组的 NewArrayExpression。
    参数
    type    Type 
    一个表示数组的元素类型的 Type。
    initializers    IEnumerable<Expression> 
    一个 IEnumerable<T>，包含用来填充 Expression 集合的 Expressions 对象。
    示例
    下面的示例演示如何使用NewArrayInit方法创建一个表示创建一维字符串数组的表达式树, 该数组是使用字符串表达式列表进行初始化的。 
    List<System.Linq.Expressions.Expression> trees =
        new List<System.Linq.Expressions.Expression>()
            { System.Linq.Expressions.Expression.Constant("oak"),
              System.Linq.Expressions.Expression.Constant("fir"),
              System.Linq.Expressions.Expression.Constant("spruce"),
              System.Linq.Expressions.Expression.Constant("alder") };

    // Create an expression tree that represents creating and  
    // initializing a one-dimensional array of type string.
    System.Linq.Expressions.NewArrayExpression newArrayExpression =
        System.Linq.Expressions.Expression.NewArrayInit(typeof(string), trees);

    // Output the string representation of the Expression.
    Console.WriteLine(newArrayExpression.ToString());

    // This code produces the following output:
    //
    // new [] {"oak", "fir", "spruce", "alder"}
    注解
    的每个initializers元素的type 属性必须表示可分配给由表示的类型的类型, 可能在被引用后Type 。 
     备注
    仅当type为时Expression, 才会对元素进行引用。 引号表示元素包装在Quote节点中。 生成的节点是一个UnaryExpression , Operand其initializers属性为的元素。

#endregion NewArrayInit 

#region call  ( obj.sampleMethod() )

Call

    Call(Type, String, Type[], Expression[])

    通过调用合适的工厂方法，创建一个 MethodCallExpression，它表示对 static（在 Visual Basic 中为 Shared）方法的调用。
	Call(Expression, MethodInfo)

    创建一个 MethodCallExpression，它表示调用不带自变量的方法。
	Call(Expression, MethodInfo, IEnumerable<Expression>)

    创建一个表示调用带参数的方法的 MethodCallExpression。	
	Call(MethodInfo, IEnumerable<Expression>)

    创建一个 MethodCallExpression，它表示调用有参数的 static（在 Visual Basic 中为 Shared）方法。
	Call(Expression, String, Type[], Expression[])

    通过调用合适的工厂方法，创建一个 MethodCallExpression，它表示方法调用。

	参数:
		instance Expression

        要将 Expression 属性设置为与其相等的 Object，它为 null（在 Visual Basic 中则为static）方法传递 Shared。
		method MethodInfo

        要将 MethodInfo 属性设置为与其相等的 Method。
		arguments IEnumerable<Expression>

        一个 IEnumerable<T>，包含用来填充 Expression 集合的 Arguments 对象。

        type Type

        包含指定的 static（在 Visual Basic 中为 Shared）方法的类型。
		methodName String

        方法的名称。
		typeArguments Type[]

        指定泛型方法的类型参数的 Type 对象的数组。 当 methodName 指定非泛型方法时，此参数应为 null。
		例:创建一个表达式, 该表达式调用不带参数的方法。
		Expression callExpr = Expression.Call(
            Expression.Constant("sample string"), typeof(String).GetMethod("ToUpper", new Type[] { }));
例:创建一个表达式, 该表达式调用具有两个参数的实例方法
Expression callExpr = Expression.Call(
    Expression.New(typeof(SampleClass)),
     		typeof(SampleClass).GetMethod("AddIntegers", new Type[] { typeof(int), typeof(int) }),
		Expression.Constant(1),
		Expression.Constant(2)
        		);
		 return arg1 + arg2;

#endregion call
	
#region constant
Constant(object value)/(object value, type type)
    创建一个 ConstantExpression，其 NodeType 属性等于 Constant，并且其 Value 和 Type 属性设置为指定值。
	它把 Value 属性设置为指定值
    参数:	value:Object 要将 Object 属性设置为与其相等的 Value。
	参数:	type:Type 要将 Type 属性设置为与其相等的 Type
    例:创建一个表达式, 该表达式表示可以为 null 的类型的常量并将其null值设置为。
	Expression constantExpr = Expression.Constant(
                            null,
                            typeof(double?)
                        );

#endregion

#region Parameter
Parameter(Type, String)
Parameter(Type)
创建一个 ParameterExpression 节点，该节点可用于标识表达式树中的参数或变量。
参数
    type    Type 
    参数或变量的类型。
    name    String 
    仅用于调试或打印目的的参数或变量的名称。

    示例
    下面的示例演示如何创建MethodCallExpression输出ParameterExpression对象的值的对象。 
    // Add the following directive to the file:
    // using System.Linq.Expressions;  

    // Creating a parameter for the expression tree.
    ParameterExpression param = Expression.Parameter(typeof(int));

    // Creating an expression for the method call and specifying its parameter.
    MethodCallExpression methodCall = Expression.Call(
        typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }),
        param
    );

    // The following statement first creates an expression tree,
    // then compiles it, and then runs it.
    Expression.Lambda<Action<int>>(
        methodCall,
        new ParameterExpression[] { param }
    ).Compile()(10);

    // This code example produces the following output:
    //
    // 10

#endregion Parameter

#region Quote UnaryExpression
(System.Linq.Expressions.Expression expression);
创建一个表示具有类型 UnaryExpression 的常量值的表达式的 Expression。
    参数
    expression    Expression 
    要将 Expression 属性设置为与其相等的 Operand。

    返回
        一个 UnaryExpression，其 NodeType 属性等于 Quote，并且其 Operand 属性设置为指定值。
    注解
    Expression<TDelegate> expression生成Type 的的属性表示构造类型,其中类型参数是表示的类型。
    UnaryExpression类别. Method 属性为 null。 IsLifted和都IsLiftedToNull是。false

#endregion Quote

#region Reduce
    将此节点简化为更简单的表达式。 
    如果 CanReduce 返回 true，则它应返回有效的表达式。 此方法可以返回本身必须简化的另一个节点。

    返回
    已简化的表达式。

#endregion Reduce

赋值
#region Assign  (a = b)

Assign(Expression left, Expression right)

    创建一个表示赋值运算的 BinaryExpression。
	参数:
		left Expression

        要将 Expression 属性设置为与其相等的 Left。
		right Expression

        要将 Expression 属性设置为与其相等的 Right。
	示例:创建一个表示赋值运算的表达式。
	ParameterExpression variableExpr = Expression.Variable(typeof(String), "sampleVar");
Expression assignExpr = Expression.Assign(
variableExpr,
    Expression.Constant("Hello World!")
    );
Expression blockExpr = Expression.Block(
    new ParameterExpression[] { variableExpr },
   assignExpr
    );
Console.WriteLine(assignExpr.ToString());//==>(sampleVar = "Hello World!")
	Console.WriteLine(Expression.Lambda<Func<String>>(blockExpr).Compile()());//==> Hello World!
	
#endregion
	
#region Bind
Bind
    Bind(MemberInfo, Expression)

    创建一个 MemberAssignment，它表示字段或属性的初始化。
	Bind(MethodInfo, Expression)

    使用属性访问器方法，创建一个表示成员初始化的 MemberAssignment。
	参数
        member MemberInfo
        要将 MemberInfo 属性设置为与其相等的 Member。
		expression Expression

        要将 Expression 属性设置为与其相等的 Expression。
		propertyAccessor MethodInfo
        一个表示属性访问器方法的 MethodInfo。

#endregion

#region convert  (int)sampleObject
Convert(expression exp, type type)/(expression exp, type type, MethodInfo method)
     创建一个表示类型转换运算的 UnaryExpression。其 NodeType 属性等于 Convert，并且其 Operand 和 Type 属性设置为指定值。
	参数:
		expression Expression 要将 Expression 属性设置为与其相等的 Operand。
		type Type 要将 Type 属性设置为与其相等的 Type。
		Method 生成UnaryExpression的的属性设置为实现方法。 IsLiftedToNull 属性为 false。 如果提升节点, IsLifted则为。 true 否则，它是 false。
	例:创建表示类型转换运算的表达式。
	Expression convertExpr = Expression.Convert(
                            Expression.Constant(5.5),
                            typeof(Int16)
                        );
Console.WriteLine(convertExpr.ToString());//==>Convert(5.5)
	Console.WriteLine(Expression.Lambda<Func<Int16>>(convertExpr).Compile()());//==>5


condition(expr test, expr ifTrue, expr ifFalse)

    创建一个表示条件语句的 ConditionalExpression。
	例:
	Expression conditionExpr = Expression.Condition(
                           Expression.Constant(num > 10),
                           Expression.Constant("num is greater than 10"),
                           Expression.Constant("num is smaller than 10")
                         );

ConvertChecked
    ConvertChecked(Expression, Type)
    ConvertChecked(Expression, Type, MethodInfo)
    创建一个 UnaryExpression，它表示在目标类型发生溢出时引发异常的转换运算。
    参数:
		expression Expression 要将 Expression 属性设置为与其相等的 Operand。
		type Type 要将 Type 属性设置为与其相等的 Type。
		Method 生成UnaryExpression的的属性设置为实现方法。 IsLiftedToNull 属性为 false。 如果提升节点, IsLifted则为。 true 否则，它是 false。

#endregion convert

#region Default



    Default(Type type)
    创建一个 DefaultExpression，Type 属性设置为指定类型。
    参数
    type Type 
    要将 Type 属性设置为与其相等的 Type。
    示例:创建一个表达式, 该表达式表示给定类型的默认值。
    Expression defaultExpr = Expression.Default(
                            typeof(byte)
                        );//==default(Byte)
    Console.WriteLine(defaultExpr.ToString());
    Console.WriteLine(
    Expression.Lambda<Func<byte>>(defaultExpr).Compile()());//==0

#endregion Default

#region Field
    Field
    Field(Expression, FieldInfo)
    Field(Expression, String)
    Field(Expression, Type, String)
    创建一个表示访问字段的 MemberExpression。
    参数:    
    expression    Expression 
    要将 Expression 属性设置为与其相等的 Expression。 对于 static（在 Visual Basic 中为 Shared），expression 必须是 null。
    field    FieldInfo 
    要将 FieldInfo 属性设置为与其相等的 Member。
    fieldName    String 
    要访问的字段的名称。
    type    Type 
    包含字段的 Type。
    示例:创建一个表示访问字段的表达式。
    class TestFieldClass
    {
        int sample = 40;
    }
    TestFieldClass obj = new TestFieldClass();
     Expression fieldExpr = Expression.Field(
        Expression.Constant(obj),
        "sample"
    Console.WriteLine(Expression.Lambda<Func<int>>(fieldExpr).Compile()());//==40
    https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.expressions.expression.field?view=netframework-4.8


#endregion

#region Property
    Property(Expression, String, Expression[]) 
    创建一个 IndexExpression，它表示对索引属性的访问。
    Property(Expression, PropertyInfo, Expression[]) 
    创建一个 IndexExpression，它表示对索引属性的访问。
    Property(Expression, PropertyInfo, IEnumerable<Expression>) 
    创建一个 IndexExpression，它表示对索引属性的访问。
    Property(Expression, Type, String) 
    创建一个访问属性的 MemberExpression。
    Property(Expression, PropertyInfo) 
    创建一个表示访问属性的 MemberExpression。
    Property(Expression, MethodInfo) 
    使用属性访问器方法创建一个表示访问属性的 MemberExpression。
    Property(Expression, String) 
    创建一个表示访问属性的 MemberExpression。
    参数
    instance    Expression 
    属性所属的对象。 如果该属性为 static/shared，则此对象必须为 null。
    propertyName    String 
    索引器的名称。
    arguments    Expression[] 
    用于为属性编制索引的 Expression 对象的数组。
    property    PropertyInfo 
    要将 PropertyInfo 属性设置为与其相等的 Member。
    示例
    下面的示例演示如何创建一个表示访问属性的表达式。 
    // Add the following directive to your file:
    // using System.Linq.Expressions;  

     class TestPropertyClass
     {
         public int sample {get; set;}
     }

     static void TestProperty()
     {
         TestPropertyClass obj = new TestPropertyClass();
         obj.sample = 40;

         // This expression represents accessing a property.
         // For static fields, the first parameter must be null.
         Expression propertyExpr = Expression.Property(
             Expression.Constant(obj),
             "sample"
         );

         // The following statement first creates an expression tree,
         // then compiles it, and then runs it.
         Console.WriteLine(Expression.Lambda<Func<int>>(propertyExpr).Compile()());            
     }

     // This code example produces the following output:
     //
     // 40

#endregion Property

#region PropertyOrField
(System.Linq.Expressions.Expression expression, string propertyOrFieldName);
    参数
    expression    Expression 
    一个 Expression，其 Type 包含一个名为 propertyOrFieldName 的属性或字段。 对于静态成员，这可以为 null。
    propertyOrFieldName    String 
    要访问的属性或字段的名称。
    示例
    下面的示例演示如何创建一个表达式, 该表达式表示访问属性或字段。 
    // Add the following directive to your file:
    // using System.Linq.Expressions;  

    class TestClass
    {
        public int sample { get; set; }
    }

    static void TestPropertyOrField()
    {
        TestClass obj = new TestClass();
        obj.sample = 40;

        // This expression represents accessing a property or field.
        // For static properties or fields, the first parameter must be null.
        Expression memberExpr = Expression.PropertyOrField(
            Expression.Constant(obj),
            "sample"
        );

        // The following statement first creates an expression tree,
        // then compiles it, and then runs it.
        Console.WriteLine(Expression.Lambda<Func<int>>(memberExpr).Compile()());
    }

    // This code example produces the following output:
    //
    // 40
    返回

    MemberExpression 
    一个 MemberExpression，其 NodeType 属性等于 MemberAccess，Expression 属性设置为 expression，
    并且 Member 属性设置为表示 PropertyInfo 所表示的属性或字段的 FieldInfo 或 propertyOrFieldName。
    注解

    Type生成的的PropertyType属性分别等于FieldType 或FieldInfo的属性,
    表示由propertyOrFieldName表示的属性或字段。 PropertyInfo MemberExpression 
    此方法搜索expression。类型及其基类型作为名称propertyOrFieldName的属性或字段。 
    公共属性和字段优先于非公共属性和字段。 另外, 属性优先于字段。 
    如果找到匹配的属性或字段, 
    则此方法会expression将PropertyInfo和分别FieldInfo表示该属性或字段Property Field的或传递给或。


#endregion PropertyOrField

#region MemberInit  (new point{x=1,y=2})
    MemberInit(NewExpression, IEnumerable<MemberBinding>)
    MemberInit(NewExpression, MemberBinding[])
    表示一个表达式，该表达式创建新对象并初始化该对象的一个属性。
    参数
    newExpression    NewExpression 
    要将 NewExpression 属性设置为与其相等的 NewExpression。
    bindings    IEnumerable<MemberBinding> 
    一个 IEnumerable<T>，包含用来填充 MemberBinding 集合的 Bindings 对象。
    示例
    下面的示例演示了一个表达式, 该表达式创建一个新的对象并初始化该对象的一个属性。 
    class TestMemberInitClass
    {
        public int sample { get; set; }
    }

    static void MemberInit()
    {   
        // This expression creates a new TestMemberInitClass object
        // and assigns 10 to its sample property.
        Expression testExpr = Expression.MemberInit(
            Expression.New(typeof(TestMemberInitClass)),
            new List<MemberBinding>() {
                Expression.Bind(typeof(TestMemberInitClass).GetMember("sample")[0], Expression.Constant(10))
            }
        );

        // The following statement first creates an expression tree,
        // then compiles it, and then runs it.
        var test = Expression.Lambda<Func<TestMemberInitClass>>(testExpr).Compile()();
        Console.WriteLine(test.sample);
    }

    // This code example produces the following output:
    //
    // 10
    
        下面的示例演示如何使用MemberInit(NewExpression, MemberBinding[])方法创建一个MemberInitExpression , 该对象表示新对象的两个成员的初始化。 
    class Animal
    {
        public string Species {get; set;}
        public int Age {get; set;}
    }

    public static void CreateMemberInitExpression()
    {
        System.Linq.Expressions.NewExpression newAnimal =
            System.Linq.Expressions.Expression.New(typeof(Animal));

        System.Reflection.MemberInfo speciesMember =
            typeof(Animal).GetMember("Species")[0];
        System.Reflection.MemberInfo ageMember =
            typeof(Animal).GetMember("Age")[0];

        // Create a MemberBinding object for each member
        // that you want to initialize.
        System.Linq.Expressions.MemberBinding speciesMemberBinding =
            System.Linq.Expressions.Expression.Bind(
                speciesMember,
                System.Linq.Expressions.Expression.Constant("horse"));
        System.Linq.Expressions.MemberBinding ageMemberBinding =
            System.Linq.Expressions.Expression.Bind(
                ageMember,
                System.Linq.Expressions.Expression.Constant(12));

        // Create a MemberInitExpression that represents initializing
        // two members of the 'Animal' class.
        System.Linq.Expressions.MemberInitExpression memberInitExpression =
            System.Linq.Expressions.Expression.MemberInit(
                newAnimal,
                speciesMemberBinding,
                ageMemberBinding);

        Console.WriteLine(memberInitExpression.ToString());

        // This code produces the following output:
        //
        // new Animal() {Species = "horse", Age = 12}
    }


    注解
    生成Type Type 的的newExpression属性等于的属性。 MemberInitExpression



#endregion MembeInit

#endregion 方法

#region 比较符

#region Equal BinaryExpression(a==b) 
    Equal(Expression, Expression)
    Equal(Expression, Expression, Boolean, MethodInfo)
    参数
    left
    Expression 
    要将 Expression 属性设置为与其相等的 Left。
    right
    Expression 
    要将 Expression 属性设置为与其相等的 Right。
    method    MethodInfo 
    要将 MethodInfo 属性设置为与其相等的 Method。
    示例
    下面的代码示例演示如何创建一个表达式, 用于检查其两个参数的值是否相等。 
    // Add the following directive to your file:
    // using System.Linq.Expressions;  

    // This expression compares the values of its two arguments.
    // Both arguments need to be of the same type.
    Expression equalExpr = Expression.Equal(
        Expression.Constant(42),
        Expression.Constant(45)
    );

    // Print out the expression.
    Console.WriteLine(equalExpr.ToString());

    // The following statement first creates an expression tree,
    // then compiles it, and then executes it.
    Console.WriteLine(
        Expression.Lambda<Func<bool>>(equalExpr).Compile()());

    // This code example produces the following output:
    //
    // (42 == 45)
    // False

#endregion Equal

#region NotEqual (a!=b)
    NotEqual(Expression, Expression)
    NotEqual(Expression, Expression, Boolean, MethodInfo)
    参数
    left    Expression 
    要将 Expression 属性设置为与其相等的 Left。
    right    Expression 
    要将 Expression 属性设置为与其相等的 Right。
    method    MethodInfo 
    要将 MethodInfo 属性设置为与其相等的 Method。

    返回
    一个 BinaryExpression，其 NodeType 属性等于 NotEqual，并且其 Left 和 Right 属性设置为指定值。
    注解
    生成BinaryExpression的Method属性已设置为实现方法。 Type属性设置为节点的类型。 
    如果节点已提升, IsLifted则属性为。 true 否则，它是 false。
    IsLiftedToNull 属性始终为 false。 Conversion 属性为 null。

        isFalse isTrue 返回UnaryExpression 的一个实例。
            IsFalse(Expression)
            IsFalse(Expression, MethodInfo)
            IsTrue(Expression)
            IsTrue(Expression, MethodInfo)
            参数
            expression     Expression 
            要计算的 Expression。
            method    MethodInfo 
            表示实现方法的 MethodInfo。
#endregion NotEqual

#region Great GreatThen BinaryExpression (a>b,a>=b,a<b,a<=b)
    GreatThan (a>b)
        创建一个表示“大于”数值比较的 BinaryExpression。
    GreatThenOrEequal (a>=b)
        创建一个表示“大于或等于”数值比较的 BinaryExpression。
    参数:
    left    Expression 
    要将 Expression 属性设置为与其相等的 Left。
    right    Expression 
    要将 Expression 属性设置为与其相等的 Right。
    liftToNull    Boolean 
    若要将 true 设置为 IsLiftedToNull，则为 true；若要将 false 设置为 IsLiftedToNull，则为 false。
    method    MethodInfo 
    要将 MethodInfo 属性设置为与其相等的 Method。
    https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.expressions.expression.greaterthanorequal?view=netframework-4.8
    
    LessThan (a<b)
        创建一个表示“小于”数值比较的 BinaryExpression。
    LessThanOrEqual (a<=b)
        创建一个表示“小于或等于”数值比较的 BinaryExpression。
    LessThanOrEqual(Expression, Expression)
    LessThanOrEqual(Expression, Expression, Boolean, MethodInfo)
    参数
    left    Expression 
    要将 Expression 属性设置为与其相等的 Left。
    right    Expression 
    要将 Expression 属性设置为与其相等的 Right。
    liftToNull    Boolean 
    若要将 true 设置为 IsLiftedToNull，则为 true；若要将 false 设置为 IsLiftedToNull，则为 false。
    method    MethodInfo 
    要将 MethodInfo 属性设置为与其相等的 Method。

#endregion great greatThen less lessThen


#endregion 比较符


#region 逻辑

#region and (a & b)
and  (a & b)
    创建一个表示按位 BinaryExpression 运算的 AND。
	And(Expression, Expression)

    And(Expression, Expression, MethodInfo)

    参数
        left Expression
        要将 Expression 属性设置为与其相等的 Left。
		right Expression

        要将 Expression 属性设置为与其相等的 Right。
		method MethodInfo

        要将 MethodInfo 属性设置为与其相等的 Method。
	示例:创建一个表达式, 该表达式表示对两个布尔值的逻辑 AND 运算。
	Expression andExpr = Expression.And(
        Expression.Constant(true),
        Expression.Constant(false)
    );
Console.WriteLine(andExpr.ToString());//==(True And False)
	Console.WriteLine(Expression.Lambda<Func<bool>>(andExpr).Compile()());==False

#endregion and

#region andAlso ( a && b )
andAlso ( a && b)

    AndAlso(Expression, Expression)

    AndAlso(Expression, Expression, MethodInfo)

    创建一个 BinaryExpression，它表示仅在第一个操作数的计算结果为 AND 时才计算第二个操作数的条件 true 运算。
	备注
    在或AND Visual Basic 中C#无法重载条件运算符。 
	但是, 使用按AND位AND运算符来计算条件运算符。 
	因此, 按位AND运算符的用户定义的重载可以是此节点类型的实现方法。

#endregion andAlso

#region AndAssign ( a &=b )
AndAssign (a &=b)
    创建一个表示按位 AND 赋值运算的 BinaryExpression。
	AndAssign(Expression, Expression)

    AndAssign(Expression, Expression, MethodInfo)

    AndAssign(Expression, Expression, MethodInfo, LambdaExpression)

    参数
        left Expression
        要将 Expression 属性设置为与其相等的 Left。
		right Expression

        要将 Expression 属性设置为与其相等的 Right。
		method MethodInfo

        要将 MethodInfo 属性设置为与其相等的 Method。
		conversion
        LambdaExpression

        要将 LambdaExpression 属性设置为与其相等的 Conversion。

#endregion AndAssign

#region NOT UnaryExpression  (!a,~a)
    Not(Expression)
    Not(Expression, MethodInfo)
    创建一个表示按位求补运算的 UnaryExpression。
    参数
    expression    Expression 
    要将 Expression 属性设置为与其相等的 Operand。
    method  MethodInfo 
    要将 MethodInfo 属性设置为与其相等的 Method。
    示例
    下面的示例演示如何创建表示逻辑 "非" 运算的表达式。 
    // Add the following directive to your file:
    // using System.Linq.Expressions; 

    // This expression represents a NOT operation.
    Expression notExpr = Expression.Not(Expression.Constant(true));

    Console.WriteLine(notExpr);

    // The following statement first creates an expression tree,
    // then compiles it, and then runs it.
    Console.WriteLine(Expression.Lambda<Func<bool>>(notExpr).Compile()());

    // This code example produces the following output:
    //
    // Not(True)
    // False

    返回
    UnaryExpression 
    一个 UnaryExpression，其 NodeType 属性等于 Not，并且其 Operand 属性设置为指定值。

#endregion Not

#region NotEqual BinaryExpression  (a!=b)
    NotEqual(Expression, Expression)
    NotEqual(Expression, Expression, Boolean, MethodInfo)
    创建一个表示不相等比较的 BinaryExpression。
    参数
    left    Expression 
    要将 Expression 属性设置为与其相等的 Left。
    right    Expression 
    要将 Expression 属性设置为与其相等的 Right。
    method    MethodInfo     
    要将 MethodInfo 属性设置为与其相等的 Method。
    返回
    BinaryExpression 
    一个 BinaryExpression，其 NodeType 属性等于 NotEqual，并且其 Left 和 Right 属性设置为指定值。
    注解
    生成BinaryExpression的Method属性已设置为实现方法。 Type属性设置为节点的类型。 如果节点已提升, IsLifted则属性为。 true 否则，它是 false。 IsLiftedToNull 属性始终为 false。 Conversion 属性为 null。 
    以下信息描述了实现方法、节点类型以及节点是否已提升。



#endregion Not

#region Or BinaryExpression (a | b ,a ||b)
    Or(Expression, Expression)
    Or(Expression, Expression, MethodInfo)
    OrElse(Expression, Expression)
    OrElse(Expression, Expression, MethodInfo)
    创建一个表示按位 BinaryExpression 运算的 OR。
    参数
    left
    Expression 
    要将 Expression 属性设置为与其相等的 Left。
    right
    Expression 
    要将 Expression 属性设置为与其相等的 Right。
    method
    MethodInfo 
    要将 MethodInfo 属性设置为与其相等的 Method。

#endregion or

#region OrAssign BinaryExpression (a |=b)
    OrAssign(Expression, Expression)
    OrAssign(Expression, Expression, MethodInfo)
    OrAssign(Expression, Expression, MethodInfo, LambdaExpression)
    创建一个表示按位 OR 赋值运算的 BinaryExpression。
    参数
    left    Expression 
    要将 Expression 属性设置为与其相等的 Left。
    right    Expression 
    要将 Expression 属性设置为与其相等的 Right。
    method    MethodInfo 
    要将 MethodInfo 属性设置为与其相等的 Method。
    conversion    LambdaExpression 
    要将 LambdaExpression 属性设置为与其相等的 Conversion。

    返回
    BinaryExpression 
    一个 BinaryExpression，其 NodeType 属性等于 OrAssign，并且其 Left、Right、Method 和 Conversion 属性设置为指定值。


#endregion OrAssign

#endregion

#region 数组

#region ArrayAccess
ArrayAccess
#endregion ArrayAccess

#region ArrayIndex ( array(index)
ArrayIndex
#endregion ArrayIndex

#region ArrayLength (array.Length)
ArrayLength
#endregion ArrayLength

#region

#endregion

#region

#endregion

#endregion 数组


#region 工厂方法

#region Dynamic
    Dynamic(CallSiteBinder, Type, Expression, Expression, Expression, Expression)
    Dynamic(CallSiteBinder, Type, Expression, Expression)
    Dynamic(CallSiteBinder, Type, Expression, Expression, Expression)
    Dynamic(CallSiteBinder, Type, Expression)
    Dynamic(CallSiteBinder, Type, IEnumerable<Expression>)
    Dynamic(CallSiteBinder, Type, Expression[])
    创建一个表示动态操作的 DynamicExpression。它表示由提供的 CallSiteBinder 绑定的动态操作。
    参数:
    binder     CallSiteBinder 
    动态操作的运行时联编程序。
    returnType    Type 
    动态表达式的结果类型。
    arg0    Expression 
    动态操作的第一个自变量。
    arg1    Expression 
    动态操作的第二个自变量。
    arg2    Expression 
    动态操作的第三个参数。
    arg3    Expression 
    动态操作的第四个自变量。
    arguments    IEnumerable<Expression> 
    动态操作的参数。
    注解
    结果DelegateType的属性将从参数的类型和指定的返回类型推断而来。

#endregion MakeDynamic

#region MakeBinary
    
    MakeBinary(ExpressionType, Expression, Expression)
    MakeBinary(ExpressionType, Expression, Expression, Boolean, MethodInfo)
    MakeBinary(ExpressionType, Expression, Expression, Boolean, MethodInfo, LambdaExpression)
    通过调用适当的工厂方法来创建一个 BinaryExpression。
    参数
    binaryType    ExpressionType 
    指定二元运算类型的 ExpressionType。
    left    Expression 
    一个表示左操作数的 Expression。
    right    Expression 
    一个表示右操作数的 Expression。
    liftToNull    Boolean 
    若要将 true 设置为 IsLiftedToNull，则为 true；若要将 false 设置为 IsLiftedToNull，则为 false。
    method    MethodInfo 
    一个指定实现方法的 MethodInfo。
    conversion    LambdaExpression 
    一个表示类型转换函数的 LambdaExpression。 只有在 binaryType 为 Coalesce 或复合赋值时，才使用此参数。


    示例
    使用MakeBinary(ExpressionType, Expression, Expression)方法创建一个BinaryExpression , 该对象表示从一个数字中减去另一个数。 
    // Create a BinaryExpression that represents subtracting 14 from 53.
    System.Linq.Expressions.BinaryExpression binaryExpression =
        System.Linq.Expressions.Expression.MakeBinary(
            System.Linq.Expressions.ExpressionType.Subtract,
            System.Linq.Expressions.Expression.Constant(53),
            System.Linq.Expressions.Expression.Constant(14));

        Console.WriteLine(binaryExpression.ToString());==(53 - 14)

        注解
        参数确定此方法BinaryExpression调用的工厂方法。 binaryType 例如, 如果binaryType为, Subtract则此方法将Subtract调用。
        如果liftToNull适当method的工厂方法没有相应的参数, 则将忽略和参数。

        https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.expressions.expression.makebinary?view=netframework-4.8



#endregion MakeBinary

#region MakeDyanmic
    MakeDynamic(Type, CallSiteBinder, Expression, Expression, Expression, Expression)
    MakeDynamic(Type, CallSiteBinder, Expression, Expression)
    MakeDynamic(Type, CallSiteBinder, Expression, Expression, Expression)
    MakeDynamic(Type, CallSiteBinder, Expression)
    MakeDynamic(Type, CallSiteBinder, IEnumerable<Expression>)
    MakeDynamic(Type, CallSiteBinder, Expression[])
    创建一个表示动态操作的 DynamicExpression。它表示由提供的 CallSiteBinder 绑定的动态操作。
    参数
    delegateType
    Type     CallSite 使用的委托的类型。
    binder    CallSiteBinder 
    动态操作的运行时联编程序。
    arg0    Expression 
    动态操作的第一个自变量。
    arg1    Expression 
    动态操作的第二个自变量。
    arg2    Expression 
    动态操作的第三个参数。
    arg3    Expression 
    动态操作的第四个自变量。
    arguments IEnumerable<Expression> 
    动态操作的参数。
    arguments Expression[] 
    动态操作的参数。


#endregion MakeDynamic

#region MakeIndex
    MakeIndex(Expression, PropertyInfo, IEnumerable<Expression>) Method 
    创建一个 IndexExpression，它表示访问对象中的索引属性。
    参数
    instance    Expression 
    属性所属的对象。 如果属性为 static（在 Visual Basic 中为 shared），则它应为 null。
    indexer    PropertyInfo 
    一个 Expression，它表示要编制索引的属性。
    arguments    IEnumerable<Expression> 
    一个 IEnumerable<Expression>（在 Visual Basic 中为 IEnumerable (Of Expression)）
    ，其中包含将用于为属性编制索引的自变量。

#endregion MakeINdex

#region MakeMemberAccess  (obj.sampleProperty)
    MakeMemberAccess(Expression, MemberInfo) Method 
    创建一个表示访问字段或属性的 MemberExpression。
    参数
    expression    Expression 
    一个表示成员所属对象的 Expression。 对于静态成员，这可以为 null。
    member    MemberInfo 
    描述要访问的字段或属性的 MemberInfo。
    返回
    MemberExpression 
    通过调用适当的工厂方法生成的 MemberExpression。
    注解
    此方法可用于创建一个MemberExpression表示访问字段或属性的, 具体取决于的member类型。
    如果member的类型FieldInfo为, 则此MemberExpression方法Field调用以创建。
    如果member的类型PropertyInfo为, 则此MemberExpression方法Property调用以创建。

#endregion MakeMemberAccess

#region MakeUnary
    MakeUnary(ExpressionType, Expression, Type)
    MakeUnary(ExpressionType, Expression, Type, MethodInfo)
    通过调用适当的工厂方法来创建一个 UnaryExpression。
    参数
    unaryType    ExpressionType 
    指定一元运算类型的 ExpressionType。
    operand    Expression 
    一个表示操作数的 Expression。
    type    Type 
    指定转换的目标类型的 Type（如果不适用，则传递 null）。
    返回
    通过调用适当的工厂方法生成的 UnaryExpression。
    注解
    参数确定此方法UnaryExpression调用的工厂方法。 
    unaryType 例如, 如果unaryType Convert等于, 则此方法将调用Convert。 
    如果type参数不适用于调用的工厂方法, 则将其忽略。

#endregion MakeUnary

#endregion 工厂方法


#endif


