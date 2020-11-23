using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Sag.Data.Common.Query
{
    public enum CallMethodId
    {

        StringCompare = 1001,
        StringStartsWith ,
        StringEndsWith ,
        StringIsNullOrEmpty ,
        StringContains ,
        ListContains ,
        ValueTypeCompareTo ,
        DateTimeAddYears = DateTimeFunction.AddYears,
        DateTimeAddMonths = DateTimeFunction.AddMonths,
        DateTimeAddDays = DateTimeFunction.AddDays,
        DateTimeAddHours = DateTimeFunction.AddHours,
        DateTimeAddMinutes = DateTimeFunction.AddMinutes, 
        DateTimeAddSeconds = DateTimeFunction.AddSeconds,
        DateTimeAddMilliseconds = DateTimeFunction.AddMilliseconds,
        DateTimeAddTicks = DateTimeFunction.AddTicks,
        DBDateDiffYear=StringCompare+7,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffMonth,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffDay,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffHour,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffHourTimeSpan,//DbFunctions,TimeSpan<?>,TimeSpan<?>
        DBDateDiffMinute,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffMinuteTimeSpan,//DbFunctions,TimeSpan<?>,TimeSpan<?>
        DBDateDiffSecond,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffSecondTimeSpan,//DbFunctions,TimeSpan<?>,TimeSpan<?>
        DBDateDiffMillisecond,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffMillisecondTimeSpan,//DbFunctions,TimeSpan<?>,TimeSpan<?>
        DBDateDiffMicrosecond,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffMicrosecondTimeSpan,//DbFunctions,TimeSpan<?>,TimeSpan<?>
        DBDateDiffNanosecond,//DbFunctions,DateTime(offset)<?>,DateTime(offset)<?>
        DBDateDiffNanosecondTimeSpan,//DbFunctions,TimeSpan<?>,TimeSpan<?>
        DBLike, //DbFunctions,String,String  <,Sting>
        DBContains,//DbFunctions,String,String <,Int32>
        DBFreeText, //DbFunctions,String,String <,Int32>
        DBIsDate,//DbFunctions,String
        ListAddItem,

    }
    public partial class QueryFilter
    {

        private static readonly Type _typeOfBool = typeof(bool);

        private static readonly Type _typeOfString = typeof(string);

        private static readonly Type _typeOfDateTime = typeof(DateTime);

        private static readonly Type _typeOfICollection = typeof(ICollection);

        private static readonly Type _typeOfObject = typeof(object);

        private static readonly Type _typeOfNullAbleDeclare = typeof(Nullable<>);

        /// <summary>
        ///字符串比较类型枚举
        /// </summary>
        private static readonly Type _stringComaprsionTppe = typeof(StringComparison);

        /// <summary>
        /// 字符串比较器类型
        /// </summary>
        private static readonly Type _strCompareType = typeof(StringComparer);

        /// <summary>
        /// 字符串比较枚举值表达式
        /// </summary>
        private static readonly Expression _stringComparisonExpr = Expression.Constant(StringComparison);

        /// <summary>
        /// 字符串比较器表达式
        /// </summary>
        private static readonly Expression _stringCompareExpr = StringComparison switch
        {
            StringComparison.OrdinalIgnoreCase => Expression.Constant(StringComparison.OrdinalIgnoreCase),
            StringComparison.CurrentCulture => Expression.Constant(StringComparison.CurrentCulture),
            StringComparison.CurrentCultureIgnoreCase => Expression.Constant(StringComparison.CurrentCultureIgnoreCase),
            StringComparison.InvariantCulture => Expression.Constant(StringComparison.InvariantCulture),
            StringComparison.InvariantCultureIgnoreCase => Expression.Constant(StringComparison.InvariantCultureIgnoreCase),
            StringComparison.Ordinal => Expression.Constant(StringComparison.Ordinal),
            _ => Expression.Constant(StringComparer.OrdinalIgnoreCase),

        };

        private static readonly Expression _falseConstantExpr = MakeParameterizeConstant(false, _typeOfBool);

        private static readonly Expression _trueConstantExpr = MakeParameterizeConstant(true, _typeOfBool);

        private static readonly Dictionary<CallMethodId, string> _callMathNamesDict = new Dictionary<CallMethodId, string>
        {
            {CallMethodId.ListAddItem,"Add" },
            {CallMethodId.ListContains,"Contains" },
            {CallMethodId.StringCompare,"Compare" },
            {CallMethodId.StringContains,"Contains" },
            {CallMethodId.StringStartsWith,"StartsWith" },
            {CallMethodId.StringEndsWith,"EndsWith" },
            {CallMethodId.StringIsNullOrEmpty,"IsNullOrEmpty"} ,
            {CallMethodId.ValueTypeCompareTo,"CompareTo"}

        };

        /// <summary>
        /// 是否忽略查询值 的大小写
        /// </summary>
        public static bool IsValueIgnoreCase = false;

        /// <summary>
        /// /是否忽略属性或字段名的大小写.
        /// </summary>
        public static bool IsPropertyIgnoreCase = false;

        /// <summary>
        /// 字符串比较枚举值 
        /// </summary>
        public static StringComparison StringComparison = StringComparison.CurrentCultureIgnoreCase;



    }
}
