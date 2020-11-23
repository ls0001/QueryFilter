using System;
using System.Collections.Generic;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 查询项的函数工厂
    /// </summary>
    public static class ItemFunctionsFactory
    {
        private static Dictionary<int, IItemFunction> _functionsCached = new Dictionary<int, IItemFunction>();

        #region private methods

        private static int GetCacheKey(Type sourceType, string funcName, object[] args)
        {
            var argsCount = args?.Length ?? 0;
            int key = argsCount;

            lock (args)
            {
                unchecked
                {
                    for (int i = 0; i < argsCount; i++)
                    {
                        key += key << 7 ^ args[i].GetHashCode();
                    }
                    key += key << 7 ^ sourceType.GetHashCode();
                    key += key << 7 ^ funcName.GetHashCode();
                }
            }
            return key;
        }

        private static int GetCacheKey(Type sourceType, string funcName, int[] args)
        {
            var count = args?.Length ?? 0;
            var tmpArr = new object[count];
            Array.Copy(args, tmpArr, count);
            return GetCacheKey(sourceType, funcName, tmpArr);
        }

        private static int GetCacheKey(Type sourceType, string funcName, double[] args)
        {
            var count=args?.Length ?? 0;
            var tmpArr = new object[count];
            Array.Copy(args, tmpArr, count);
            return GetCacheKey(sourceType, funcName, tmpArr);
           
        }     
        
        private static bool TryGetFunctionFromCached(int key, out IItemFunction func)
        {
            return _functionsCached.TryGetValue(key, out func);
        }

        #endregion private methods

        public static IItemFunction GetFunction(DateTimeFunction func, params double[] args)
        {
            var key = GetCacheKey(Constants.Static_TypeOfDateTime, func.ToString(), args);
            if (TryGetFunctionFromCached(key, out var mt))
                return mt;

            mt = func switch
            {
                DateTimeFunction.AddYears => new DateTimeAddYears(ConvertArgsToIntArray()),
                DateTimeFunction.AddMonths => new DateTimeAddMonths(ConvertArgsToIntArray()),
                DateTimeFunction.AddDays => new DateTimeAddDays(args),
                DateTimeFunction.AddHours => new DateTimeAddHours(args),
                DateTimeFunction.AddMinutes => new DateTimeAddMinutes(args),
                DateTimeFunction.AddSeconds => new DateTimeAddSeconds(args),
                DateTimeFunction.AddMilliseconds => new DateTimeAddMilliseconds(args),
                _ => throw new Exception(),
            };

            return mt;

            int[] ConvertArgsToIntArray()
            {
                var tmpArr = new int[args?.Length ?? 0];
                if (args == null) return tmpArr;
                for (var i = 0; i < args.Length; i++)
                {
                    tmpArr[i] = Convert.ToInt32(args[i]);
                }
                return tmpArr;
            }
        }
         
        public static IItemFunction GetFunction(DateTimeFunction func, params int[] args)
        {
            var key = GetCacheKey(Constants.Static_TypeOfDateTime, func.ToString(), args);
            if (TryGetFunctionFromCached(key, out var mt))
                return mt;

            mt = func switch
            {
                DateTimeFunction.AddYears => new DateTimeAddYears(args),
                DateTimeFunction.AddMonths => new DateTimeAddMonths(args),
                DateTimeFunction.AddDays => new DateTimeAddDays(ConvertArgsToDoubleArray()),
                DateTimeFunction.AddHours => new DateTimeAddHours(ConvertArgsToDoubleArray()),
                DateTimeFunction.AddMinutes => new DateTimeAddMinutes(ConvertArgsToDoubleArray()),
                DateTimeFunction.AddSeconds => new DateTimeAddSeconds(ConvertArgsToDoubleArray()),
                DateTimeFunction.AddMilliseconds => new DateTimeAddMilliseconds(ConvertArgsToDoubleArray()),
                _ => throw new Exception(),
            };

            return mt;

            double[] ConvertArgsToDoubleArray()
            {
                var tmpArr = new double[args?.Length ?? 0];
                if (args == null) return tmpArr;
                for (var i = 0; i < args.Length; i++)
                {
                    tmpArr[i] =args[i];
                }
                return tmpArr;
            }

        }
 }


}
