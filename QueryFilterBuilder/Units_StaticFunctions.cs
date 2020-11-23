using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sag.Data.Common.Query
{
    internal static class Units
    {

        public const string const_funcName_EndsWith = "EndsWith";
        public const string const_funcName_StartsWith = "StartsWith";
        public const string const_funcName_IsNullOrEmpty = "IsNullOrEmpty";
        public const string const_funcName_Compare = "Compare";
        public const string const_funcName_CompareTo = "CompareTo";
        public const string const_funcName_AddYears = "AddYears";
        public const string const_funcName_AddMonths = "AddMonths";
        public const string const_funcName_AddDays = "AddDays";
        public const string const_funcName_AddHours = "AddHours";
        public const string const_funcName_AddMinutes = "AddMinutes";
        public const string const_funcName_AddSeconds = "AddSeconds";
        public const string const_funcName_AddMilliseconds = "AddMilliseconds";
        public const string const_funcName_AddTicks = "AddTicks";
        public const string const_funcName_DBDateDiffDay = "DBDateDiffDay";
        public const string const_funcName_DBDateDiffYear = "DBDateDiffYear";
        public const string const_funcName_DBDateDiffMonth = "DBDateDiffMonth";
        public const string const_funcName_DBDateDiffHour = "DBDateDiffHour";
        public const string const_funcName_DBDateDiffMinute = "DBDateDiffMinute";
        public const string const_funcName_DBDateDiffSecond = "DBDateDiffSecond";
        public const string const_funcName_DBDateDiffMillisecond = "DBDateDiffMillisecond";
        public const string const_funcName_DBDateDiffMicrosecond = "DBDateDiffMicrosecond";
        public const string const_funcName_DBDateDiffNanosecond = "DBDateDiffNanosecond";
        public const string const_funcName_DBLike = "DBLike";
        public const string const_funcName_DBIsDate = "DBIsDate";
        public const string const_funcName_DBFreeText = "DBFreeText";
        public const string const_funcName_ListAddItem = "ListAddItem";
        public const string const_funcName_StringContains = "StringContains";
        public const string const_funcName_ListContains = "ListContains";
        public const string const_funcName_DBContains = "DbContains";


        public static CallMethodId GetCallMethodId(Type fromType, string methodName)
        {
            const string nameofContains = "Contains";
            string tmpName = methodName;
            if (methodName == nameofContains)
            {
                if (fromType == Constants.static_typeofString)
                    tmpName = Units.const_funcName_StringContains;
                else if (fromType is ICollection)
                    tmpName = Units.const_funcName_ListContains;
            }
            return tmpName switch
            {
                const_funcName_ListContains => CallMethodId.ListContains,
                const_funcName_StringContains => CallMethodId.StringContains,
                const_funcName_EndsWith => CallMethodId.StringEndsWith,
                const_funcName_StartsWith => CallMethodId.StringStartsWith,
                const_funcName_IsNullOrEmpty => CallMethodId.StringIsNullOrEmpty,
                const_funcName_Compare => CallMethodId.StringCompare,
                const_funcName_CompareTo => CallMethodId.ValueTypeCompareTo,
                const_funcName_AddYears => CallMethodId.DateTimeAddYears,
                const_funcName_AddMonths => CallMethodId.DateTimeAddMonths,
                const_funcName_AddDays => CallMethodId.DateTimeAddDays,
                const_funcName_AddHours => CallMethodId.DateTimeAddHours,
                const_funcName_AddMinutes => CallMethodId.DateTimeAddMinutes,
                const_funcName_AddSeconds => CallMethodId.DateTimeAddSeconds,
                const_funcName_AddMilliseconds => CallMethodId.DateTimeAddMilliseconds,
                const_funcName_AddTicks => CallMethodId.DateTimeAddTicks,
                const_funcName_DBDateDiffDay => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffDay            ,  
                const_funcName_DBDateDiffYear => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffYear           ,  
                const_funcName_DBDateDiffMonth => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffMonth          ,  
                const_funcName_DBDateDiffHour => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffHour           ,  
                const_funcName_DBDateDiffMinute => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffMinute         ,  
                const_funcName_DBDateDiffSecond => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffSecond         ,  
                const_funcName_DBDateDiffMillisecond => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffMillisecond    ,  
                const_funcName_DBDateDiffMicrosecond => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffMicrosecond    ,  
                const_funcName_DBDateDiffNanosecond => throw new NotImplementedException(),    //    CallMethodId.DBDateDiffNanosecond     ,  
                const_funcName_DBLike => throw new NotImplementedException(),    //    CallMethodId.DBLike                   ,  
                const_funcName_DBContains => throw new NotImplementedException(),    //    CallMethodId.DBContains               ,  
                const_funcName_DBIsDate => throw new NotImplementedException(),    //    CallMethodId.DBIsDate,                   
                const_funcName_DBFreeText => throw new NotImplementedException(),    //    CallMethodId.DBFreeText,                 
                const_funcName_ListAddItem => CallMethodId.ListAddItem,
                _ => throw new MemberAccessException(),
            };
        }


        public static string GetCallMethodName(CallMethodId methodId)
        {
            return methodId switch
            {
                CallMethodId.ListContains => const_funcName_ListContains,
                CallMethodId.StringContains => const_funcName_StringContains,
                CallMethodId.StringEndsWith => const_funcName_EndsWith,
                CallMethodId.StringStartsWith => const_funcName_StartsWith,
                CallMethodId.StringIsNullOrEmpty => const_funcName_IsNullOrEmpty,
                CallMethodId.StringCompare => const_funcName_Compare,
                CallMethodId.ValueTypeCompareTo => const_funcName_CompareTo,
                CallMethodId.DateTimeAddYears => const_funcName_AddYears,
                CallMethodId.DateTimeAddMonths => const_funcName_AddMonths,
                CallMethodId.DateTimeAddDays => const_funcName_AddDays,
                CallMethodId.DateTimeAddHours => const_funcName_AddHours,
                CallMethodId.DateTimeAddMinutes => const_funcName_AddMinutes,
                CallMethodId.DateTimeAddSeconds => const_funcName_AddSeconds,
                CallMethodId.DateTimeAddMilliseconds => const_funcName_AddMilliseconds,
                CallMethodId.DateTimeAddTicks => const_funcName_AddTicks,
                CallMethodId.DBDateDiffDay => throw new NotImplementedException(),      //   const_funcName_DBDateDiffDay               ,   
                CallMethodId.DBDateDiffYear => throw new NotImplementedException(),      //   const_funcName_DBDateDiffYear              ,   
                CallMethodId.DBDateDiffMonth => throw new NotImplementedException(),      //   const_funcName_DBDateDiffMonth             ,   
                CallMethodId.DBDateDiffHour => throw new NotImplementedException(),      //   const_funcName_DBDateDiffHour              ,   
                CallMethodId.DBDateDiffMinute => throw new NotImplementedException(),      //   const_funcName_DBDateDiffMinute            ,   
                CallMethodId.DBDateDiffSecond => throw new NotImplementedException(),      //   const_funcName_DBDateDiffSecond            ,   
                CallMethodId.DBDateDiffMillisecond => throw new NotImplementedException(),      //   const_funcName_DBDateDiffMillisecond       ,   
                CallMethodId.DBDateDiffMicrosecond => throw new NotImplementedException(),      //   const_funcName_DBDateDiffMicrosecond       ,   
                CallMethodId.DBDateDiffNanosecond => throw new NotImplementedException(),  //   const_funcName_DBDateDiffNanosecond        ,   
                CallMethodId.DBLike => throw new NotImplementedException(),      //   const_funcName_DBLike                      ,   
                CallMethodId.DBContains => throw new NotImplementedException(),     //   const_funcName_DBContains                  ,   
                CallMethodId.DBIsDate => throw new NotImplementedException(),   //   const_funcName_DBIsDate                    ,   
                CallMethodId.DBFreeText => throw new NotImplementedException(),   //   const_funcName_DBFreeText                  ,   
                CallMethodId.ListAddItem => const_funcName_ListAddItem,
                _ => throw new NotImplementedException(),
            };
        }

    }
}
