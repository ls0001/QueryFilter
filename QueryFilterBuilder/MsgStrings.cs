using System;

namespace Sag.Data.Common.Query
{
    internal static class MsgStrings
    {

        internal static string DestTypeForConvert = _Resources.DestTypeForConvert;

        internal static string ExpectedMemberListNull = _Resources.ExpectedMemberListNull;

        internal static string IndexOutOfRange = _Resources.IndexOutOfRange;

        internal static string InvalidObjectType(object value,Type type) =>string.Format( _Resources.InvalidObjectType,value,type?.Name??"");

        internal static string InvalidPropertyExpressionConvert(string propName, Type type, Type toType) => string.Format(_Resources.InvalidPropertyExpressionConvert, type?.Name ?? "", toType?.Name ?? "");
        internal static string InvalidTypeConvert(Type type,Type toType) => string.Format(_Resources.InvalidTypeConvert, type?.Name ?? "",toType?.Name??"");

        internal static string InvalidLvalue(string paramName) =>string.Format( _Resources.Invalidvalue,paramName);
        internal static string MemberNotDefinedForType(string propName, Type type) => string.Format(_Resources.MemberNotDefinedForType, propName, type?.Name ?? "");

        internal static string NotSupportedArithmeticException(object leftValue, object rightValue, string opName) => string.Format(_Resources.NotSupportedArithmeticException, leftValue, rightValue, opName);

        internal static string NullGetOperatorWithStringDelegate(string propName) =>string.Format(_Resources.NullGetOperatorWithStringDelegate,propName);

        internal static string ParameterType = _Resources.ParameterType;
        internal static string PropertyDoesNotHaveGetter(string propName) => string.Format(_Resources.PropertyDoesNotHaveGetter, propName);
        internal static string TypeCannotNull(string paramName) => string.Format(_Resources.TypeParamCannotNull, paramName);

        internal static string TypeContainsGenericParameters(Type type) =>string.Format(_Resources.TypeContainsGenericParameters,type?.Name??"");

        internal static string TypeNotIEnumerable(Type type) =>string.Format(_Resources.TypeNotIEnumerable,type?.Name??"");

        internal static string TypeParameterIsNotDelegate = _Resources.TypeParameterIsNotDelegate;

        internal static string ValueCannotNull = _Resources.ValueCannotNull;

        //internal static string ValueCannotNull = _Resources.ValueCannotNull;



        //  internal static string TypeParameterIsNotDelegate = _Resources.TypeParameterIsNotDelegate;

        //  internal static string InvalidLvalue(string paramName) =>string.Format( _Resources.Invalidvalue,paramName);

        //  internal static string TypeContainsGenericParameters(Type type) = string.Format(_Resources.TypeContainsGenericParameters,type?Name??"");

        //  internal static string InvalidObjectType(object value,Type type) =string.Format( _Resources.InvalidObjectType,value,type?.Name??"");

        //  internal static string TypeNotIEnumerable(Type type) = string.Format(_Resources.TypeNotIEnumerable,type?.Name??"");

        // internal static string DestTypeForConvert = _Resources.DestTypeForConvert;

        //   internal static string NullGetOperatorWithStringDelegate = _Resources.NullGetOperatorWithStringDelegate;

        //  internal static string ExpectedMemberListNull = _Resources.ExpectedMemberListNull;

        //internal static string PropertyNameCanotNull = _Resources.PropertyNameCanotNull;




    }
}
