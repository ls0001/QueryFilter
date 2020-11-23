using System;
using System.Text.Json;

namespace Sag.Data.Common.Query.Internal
{
    internal static class MsgStrings
    {

        internal static string ArgumentsIndexOutOfRange(string source, string methodName) =>source + ":\r\n" + string.Format(_Resources.ArgumentsIndexOutOfRange, methodName);


        internal static string ValueCannotNull(string source) => source + ":\r\n" + string.Format( _Resources.ValueCannotNull,source);

        internal static string TypeCannotNull(string source, Type type) => source + ":\r\n" + string.Format( _Resources.TypeCannotNull,type?.Name??"");

        internal static string MemberNotDefinedForType(string source, string propName,Type type) => source + ":\r\n" + string.Format(_Resources.MemberNotDefinedForType,propName,type?.Name??"");

        //  internal static string TypeParameterIsNotDelegate = _Resources.TypeParameterIsNotDelegate;

        //  internal static string InvalidLvalue(string paramName) =>string.Format( _Resources.Invalidvalue,paramName);

        //  internal static string TypeContainsGenericParameters(Type type) = string.Format(_Resources.TypeContainsGenericParameters,type?Name??"");

        //  internal static string InvalidObjectType(object value,Type type) =string.Format( _Resources.InvalidObjectType,value,type?.Name??"");

        internal static String InvalidParamType(string source, string paramName,Type  destType) => source + ":\r\n" + string.Format(_Resources.InvalidParamType,paramName, destType?.Name ?? "");

      //  internal static string TypeNotIEnumerable(Type type) = string.Format(_Resources.TypeNotIEnumerable,type?.Name??"");

        internal static string InvalidTypeConvert(Type type,Type toType) =>string.Format( _Resources.InvalidTypeConvert,type?.Name??"",toType?.Name??"");

        internal static string ParameterType = _Resources.ParameterType;

        internal static string DestTypeForConvert = _Resources.DestTypeForConvert;

        internal static string NullGetOperatorWithStringDelegate = _Resources.NullGetOperatorWithStringDelegate;

        internal static string IndexOutOfRange = _Resources.IndexOutOfRange;

        internal static string ExpectedMemberListNull = _Resources.ExpectedMemberListNull;

        internal static string JsonTokenTypeNotPropertyName(JsonTokenType token) =>string.Format( _Resources.JsonTokenTypeNotPropertyName,token);

        internal static string PropertyNameCanotNull = _Resources.PropertyNameCanotNull;

        internal static string NotJsonObjectStart = _Resources.NotJsonObjectStart;

        internal static string NotJsonArrayStart = _Resources.NotJsonArrayStart;

        internal static string JsonDeserializeCollectionHasError = _Resources.JsonDeserializeCollectionHasError;

        internal static string JsonCannotConvertToType(object value,Type type) =>string.Format( _Resources.JsonCannotConvertToType,value,type?.Name??"");

    }
}
