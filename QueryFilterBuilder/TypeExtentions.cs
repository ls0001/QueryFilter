using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    internal static class TypeExtentions
    {

        private static readonly Type _ICollectionType = typeof(ICollection);
        private static readonly Type _ObjectType = typeof(object);
        private static readonly Type _ValueTypeType = typeof(ValueTuple);
        private static readonly Type _EnumType = typeof(Enum);

        /// <summary>
        /// 从自身开始向上查找所有父级(包括接口)的泛型定义,是否包含指定类型并返回,否则返回null.
        /// </summary>
        /// <param name="type">要检查的类型</param> 
        /// <param name="definition">指定的被比较的类型定义</param>

        /// <returns></returns>
        internal static Type FindGenericType(this Type type, Type definition)
        {
            return internalFindGenericType(type, definition);
        }

        private static Type internalFindGenericType(Type type, Type definition)
        {
            bool? definitionIsInterface = null;
            while (type != null && type != _ObjectType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
                    return type;
                if (!definitionIsInterface.HasValue)
                    definitionIsInterface = definition.IsInterface;
                if (definitionIsInterface.GetValueOrDefault())
                {
                    foreach (Type itype in type.GetInterfaces())
                    {
                        Type found = internalFindGenericType(itype, definition);
                        if (found != null)
                            return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }


        /// <summary>
        /// 用于检查本类型是否实现或继承了指定类型（接口或类，但不包含object类型）
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <param name="definition">指定的类型定义</param>
        /// <returns>从自身开始向上查找所有父级(包括接口),如果包含指定类型定义，返回找到的类型,否则返回null.</returns>
        internal static Type FindType(this Type type, Type definition)
        {
            return internalFindType(type, definition);
        }

        private static Type internalFindType(Type type, Type definition)
        {
            bool? definitionIsInterface = null;
            while (type != null && type != _ObjectType)
            {
                if (type == definition)
                    return definition;
                if (!definitionIsInterface.HasValue)
                    definitionIsInterface = definition.IsInterface;
                if (definitionIsInterface.GetValueOrDefault())
                {
                    foreach (Type itype in type.GetInterfaces())
                    {
                        Type found = internalFindType(itype, definition);
                        if (found != null)
                            return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }


        /// <summary>
        /// 获取本类型运行时的静态方法集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static MethodInfo[] GetStaticMethods(this Type type)
        {
            return type.GetRuntimeMethods().Where(m => m.IsStatic).ToArray();
        }


        internal static bool IsNullableOrReferenceType(this Type type)
             => !type.IsValueType || IsNullableType(type);


        /// <summary>
        /// 检查<see cref="Type">类型</see>是否为<see cref="Nullable{T}"/>的封闭式泛型类型泛型，
        /// 如果是则从参数(<paramref name="underlyingType"/>)输出可空泛型的本底类型,否则输出本身类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <param name="underlyingType">如果检查结果为<see cref="bool">True</see>，则输出实际类型。
        /// 否则为 <paramref name="type"/>本身。</param>
        /// <returns>如果 <paramref name="type"/> 是可空封闭式泛型类型(<see cref="Nullable{T}"/>)，则为True；否则为False。</returns>
        internal static bool IsNullableType(this Type type, out Type underlyingType)
        {
            if (IsNullableType(type))
            {
                underlyingType = type.UnderlyingSystemType;
                return true;
            }
            else
            {
                underlyingType = type;
                return false;
            }
        }

        /// <summary>
        /// 检查是否非值类型，或是<see cref="Nullable{T}"/>封闭式泛型类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal static bool IsNullableType(this Type type)
            => !type.IsValueType || type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);


        /// <summary>
        /// 如果类型是Nullable封闭式泛型类型,返回其本底类型,否则返回本类型,
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal static Type UnwrapNullableType(this Type type)
        {
            //return Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }


        /// <summary>
        /// 根据指定类型，生成一个可空的泛型类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns>如果类型是值类型,返回实际类型为本类型的Nullable封闭式泛型,否则返回本类型.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal static Type MakeNullableType(this Type type)
        {
            Debug.Assert(type != null, MsgStrings.ParameterType + MsgStrings.ValueCannotNull);

            return type.IsNullableType() ? type : typeof(Nullable<>).MakeGenericType(type);
        }


        /// <summary>
        /// 判断类型(检查其本底类型)是否为数字类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>如果<paramref name="type"/>为null,返回<see cref="TypeCode.Empty"/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal static bool IsNumeric(this Type type)
        {
            type = UnwrapNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 检查指定类型(检查可空的基础类型)是否为整数类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>整数：返回 <see cref="true"/> ,否则 <see cref="false"/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static bool IsInteger(this Type type)
        {
            type = type.UnwrapNullableType();
            //  if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        ///// <summary>
        ///// 测试类型(检查可空的基础类型)是否可以进行数学运算
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns>如果可以返回true,否则返回false</returns>
        ///// <exception cref="InvalidOperationException"></exception>
        ///// <exception cref="NotSupportedException"></exception>
        //public static bool IsArithmetic(this Type type)
        //{
        //    type = UnwrapNullableType(type);
        //    if (!type.IsEnum)
        //    {
        //        switch (Type.GetTypeCode(type))
        //        {
        //            case TypeCode.Int16:
        //            case TypeCode.Int32:
        //            case TypeCode.Int64:
        //            case TypeCode.Double:
        //            case TypeCode.Single:
        //            case TypeCode.UInt16:
        //            case TypeCode.UInt32:
        //            case TypeCode.UInt64:
        //            case TypeCode.Decimal:
        //                return true;
        //        }
        //    }

        //    return false;
        //}
        /// <summary>
        /// 测试指定类型是否匿名类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool IsAnonymousType(this Type type)
        {
            return type.Name.StartsWith("<>")
                && type.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length > 0
                && type.Name.Contains("AnonymousType");
        }
        /// <summary>
        /// 测试指定类型否可以转换到目标类型
        /// </summary>
        /// <param name="type">要测试的源类型</param>
        /// <param name="destType">要测试的转换到的目标类型</param>
        /// <returns></returns>
        internal static bool IsConvertableTo(this Type type, Type destType) => IsImplicitlyConvertibleTo(type, destType);

        /// <summary>
        /// 获取集合的元素类型,(原本定义的类型，没有脱空处理）
        /// </summary>
        /// <param name="type"></param>
        /// <returns>当类型是实现了ICollection的泛型集合或类型是数组时,返回集合元素类型(原本定义的类型,这里没有进行脱空),否则返回本类型.</returns>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal static Type GetCollectionElementType(this Type type)
        {
            Type eType = type;
            if (type.IsArray)
                eType = type.GetElementType();
            else
            if (type.GetInterfaces().Contains(_ICollectionType)
                //&& type.IsGenericType 
                )
                eType = type.GetGenericArguments()[0];
            return eType;
        }

        /// <summary>
        /// 当类型(或者其父级)是实现了ICollection的泛型集合或类型是数组时,返回其集合元素类型,否则返回本类型.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isLookupParent">指定是否检查其父级实现ICollection接口</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        internal static Type GetCollectionElementType(this Type type, bool isLookupParent)
        {
            Type eType = type;
            if (type.IsArray)
                eType = type.GetElementType();
            else
            if (isLookupParent ? type.FindType(_ICollectionType) != null : type.GetInterfaces().Contains(_ICollectionType)
                //&& type.IsGenericType && type.GetGenericArguments().Length==1
                )
                eType = type.GetGenericArguments()[0];

            return eType;
        }

        /// <summary>
        /// 方法确定<paramref name="dest"/>的实例是否可以分配给当前<see cref="Type">类型</see>的实例
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static bool AreReferenceAssignableTo(this Type src, Type dest)
        {
            // This actually implements "Is this identity assignable and/or reference assignable?"
            if (AreEquivalent(dest, src))
            {
                return true;
            }

            return !dest.IsValueType && !src.IsValueType && dest.IsAssignableFrom(src);
        }




        #region private
        private static bool IsImplicitNumericConversion(Type source, Type destination)
        {
            TypeCode tcSource = Type.GetTypeCode(source);
            TypeCode tcDest = Type.GetTypeCode(destination);

            switch (tcSource)
            {
                case TypeCode.SByte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Byte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int32:
                    switch (tcDest)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt32:
                    switch (tcDest)
                    {
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    switch (tcDest)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Char:
                    switch (tcDest)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Single:
                    return tcDest == TypeCode.Double;
            }

            return false;
        }

        private static bool IsImplicitReferenceConversion(Type source, Type destination) =>
            destination.IsAssignableFrom(source);

        private static bool IsImplicitBoxingConversion(Type source, Type destination) =>
            source.IsValueType && (destination == _ObjectType || destination == _ValueTypeType) || source.IsEnum && destination == _EnumType;

        private static bool IsImplicitNullableConversion(Type source, Type destination) =>
            IsNullableType(destination) && IsImplicitlyConvertibleTo(UnwrapNullableType(source), UnwrapNullableType(destination));

        private static bool IsImplicitlyConvertibleTo(this Type source, Type destination) =>
            AreEquivalent(source, destination) // identity conversion
            || IsImplicitNumericConversion(source, destination)
            || IsImplicitReferenceConversion(source, destination)
            || IsImplicitBoxingConversion(source, destination)
            || IsImplicitNullableConversion(source, destination);

        public static bool AreEquivalent(Type t1, Type t2) => t1 != null && t1.IsEquivalentTo(t2);

        #endregion //private


    }
}
