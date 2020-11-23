using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.VisualBasic;

namespace Sag.Data.Common.Query
{
    public partial class QueryFilter
    {

        //internal static class itemvalueconvert //<tsource,tdest>
        //{
        //internal static IEnumerable<TDestElement> ConvertEnumerablealue<TDestElement>(object itemValue) //where TDestElement:class,IComparable
        //{
        //    if (itemValue == null)
        //        throw new ArgumentNullException(string.Format(MsgStrings.ValueCannotNull, nameof(itemValue)));
        //    if (itemValue is IEnumerable<TDestElement> col)
        //    {
        //        var orgElemType = itemValue.GetType().GetCollectionElementType();

        //        if (orgElemType != typeof(TDestElement))
        //        {
        //            foreach (var cur in col)
        //            {
        //                if (TryConvertSingleValueType<TDestElement>(cur, out var ov))
        //                    yield return ov;
        //                else
        //                    throw new InvalidCastException(string.Format(MsgStrings.InvalidTypeConvert, $"{orgElemType}，{MsgStrings.DestTypeForConvert}:{typeof(TDestElement)}"));
        //            }
        //        }
        //        else
        //        {
        //            foreach (var cur in col)
        //                yield return cur;
        //        }
        //        yield break;
        //    }
        //    else
        //    {
        //        throw new InvalidCastException(string.Format(MsgStrings.InvalidTypeConvert, $"{itemValue.GetType()}，{MsgStrings.DestTypeForConvert}:{typeof(TDestElement)}"));
        //    }
        //}

        //internal static bool TryConvertEnumerableValue<TDestElement>(object itemValue, out IEnumerable<TDestElement> ov)
        //{
        //    try
        //    {
        //        ov = ConvertEnumerablealue<TDestElement>(itemValue);
        //        return true;
        //    }
        //    catch
        //    {
        //        ov = null;
        //        return false;
        //    }
        //}






    }
    //}
}
