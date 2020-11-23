using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    internal static class TypeConverter
    {

        static string[] _boolTrueValueStrings = new string[] { "True", "Yes", "是", "Y", "T" };

        /// <summary>
        /// 将查询值转换至列定义的基本类型,成功则输出转换后的值，失败则输出原值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destType">期望的目标类型,通常是被查询属性的基本数据类型,</param>
        /// <returns>成功：返回True,失败:返回False</returns>
        /// <exception cref="InvalidCastException"/>
        public static bool TryConvertSingleValueType(object value, Type destType, out object outValue)
        {
            outValue = value;

            if (value == null)            //空值不需要处理
                return true;
            if (destType == null)         //目标类型错误
                return false;

            Type orgType = value.GetType();
            if (orgType == destType)
                return true;

            bool orgIsNullAble = orgType.IsNullableType(out var orgUnderlyingType);
            bool destIsNullAble = destType.IsNullableType(out var destUnderlyingType);
            //if (orgIsNullAble)
            //    value = new NullableConverter(orgType).ConvertTo(value, orgUnderlyingType);

            //基础是否类型相同
            if (orgUnderlyingType == destUnderlyingType)//查询值的类型与字段值类型是否相同
            {
                if (destIsNullAble && !orgIsNullAble)
                    outValue = new NullableConverter(destType).ConvertFrom(value);

                return true;
            }

            object tmpVal;
            try
            {
                //字符转bool
                if (destUnderlyingType == Constants.Static_TypeOfBool && orgUnderlyingType == Constants.Static_TypeOfChar)
                {
                    tmpVal = (char)value == '1' ? true : false;
                }
                //字符串转bool
                else if (destUnderlyingType == Constants.Static_TypeOfBool && orgUnderlyingType == Constants.Static_TypeOfString)
                {
                    tmpVal = false;
                    var strValue = ((string)value).Trim();
                    var valueSize = strValue.Length;
                    if (valueSize == 1 && strValue[0] == '1')
                        tmpVal = true;
                    else
                    {
                        for (var i = 0; i < _boolTrueValueStrings.Length; i++)
                        {
                            var str = _boolTrueValueStrings[i];
                            if (valueSize == str.Length
                                && (string.CompareOrdinal(str, strValue) == 0
                                || string.Compare(str, strValue, StringComparison.OrdinalIgnoreCase) == 0)
                               )
                            {
                                tmpVal = true;
                                break;
                            }
                        }
                    }
                }
                //字符串转日期型
                else if (destUnderlyingType == Constants.Static_TypeOfDateTime && (orgUnderlyingType == Constants.Static_TypeOfString))
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime outDate))
                        tmpVal = outDate;
                    else
                        return false;
                }
                //其它
                else
                {
                    if (!(value is IConvertible))            //不支持转换
                        return false;
                    //!=1的值 转bool,使其=false
                    if (destUnderlyingType == Constants.Static_TypeOfBool && int.TryParse(value.ToString(), out int b) && b != 1)
                        tmpVal = false;
                    else
                        tmpVal = Convert.ChangeType(value, destUnderlyingType);
                }
            }
            catch (Exception ex)
            {
                //return false;
                throw new InvalidCastException(MsgStrings.InvalidTypeConvert(orgType,destType) + "\n\r" + ex.Message);
            }

            if (destIsNullAble)
                outValue = new NullableConverter(destType).ConvertFrom(tmpVal);
            else
                outValue = tmpVal;

            return true;
        }

        public static bool TryConvertItemValuesType(ICollection orgList, Type destType, out ICollection outArray)
        {   
            outArray = null; 
            if (orgList == null)
                return false; 
            if (destType == null)
                return false; 
            if (orgList.GetType() == destType)
            { 
                outArray = orgList; return true; 
            }
 
            //校检并重构数组
            var destElemType = destType.GetCollectionElementType();
            var newListType = typeof(List<>).MakeGenericType(destElemType);         //新集合的类型
            var newList = Activator.CreateInstance(newListType,new object[] { orgList.Count});                //新集合实例
            var method = newListType.GetMethod("Add", new[] { destElemType });
            foreach (var v in orgList)
            {
                if (TryConvertSingleValueType(v, destElemType, out dynamic nv))
                    method.Invoke(newList, new[] { nv });
                else//无法转换的值丢掉？
                {
                    return false;
                }
            }
            outArray = ((ICollection)newList);
            return true;
           
        }

        public static bool TryConvertItemValuesType(IEnumerable orgList, Type destType, out IEnumerable outArray)
        {
            outArray = null;
            if (orgList == null || destType == null)
                return false;
            if (destType is not IEnumerable) throw new Exception(MsgStrings.InvalidTypeConvert(orgList.GetType(), destType));
            if (orgList.GetType() == destType)
            { outArray = orgList; return true; }
            //校检并重构数组
            var destElemType = destType.GetCollectionElementType();
            var arr = Array.CreateInstance(destElemType, 10);
            var count = 0;
            var enumerator = orgList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (TryConvertSingleValueType(enumerator.Current, destElemType, out object nv))
                {
                    if (count >= arr.GetLength(0))
                    {
                        var resize = Array.CreateInstance(destElemType, count * 2);
                        Array.Copy(arr, resize, count);
                        arr = resize;
                    }
                    arr.SetValue(nv, count);
                    count++;
                }
                else
                {
                    return false;
                }
            }
            var result = Array.CreateInstance(destElemType, count);
            Array.Copy(arr, result, count);
            outArray = result;
            return true;

        }
    }
}
