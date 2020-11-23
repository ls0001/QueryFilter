using System;
using System.Collections.Generic;

namespace Sag.Data.Common.Query
{
    public sealed class TypeMatcher
    {
        static Dictionary<string, Type> _nullAbleNumberTypeCache = new Dictionary<string, Type>();

        /// <summary>
        /// 匹配两个类型(进行基础类型级的比较)是否相同,或数值精度更高。
        /// </summary>
        /// <param name="tp1"></param>
        /// <param name="tp2"></param>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// 返回<paramref name="tp1"/>（如果=<paramref name="tp2"/>）,或精度更高的那一个类型。
        /// </item><item>
        /// 从输出参数输出类型本身
        /// （当其为非集合类型）
        /// 或其所声明的集合元素类型（当其为集合类型）。
        /// </item> <item>
        /// 如果匹配失败，或任何一个为空,则返回和输出都为<paramref name="null"/>。
        /// </item>
        /// </list>
        /// </returns>
        public static Type MatchDataTtype(Type tp1, Type tp2, out Type elementType)
        {
            elementType = null;
            if (tp1 == null || tp2 == null)
                return null;
            if (tp1 == tp2)
                return tp1;

            var te1 = tp1.GetCollectionElementType();
            var te2 = tp2.GetCollectionElementType();
            //一个是集合,另一个不是,无法适配
            if ((te1 == tp1 && te2 != tp2) || (te1 != tp1 && te2 == tp2))
                return null;

            var tu1 = te1.UnwrapNullableType();
            var tu2 = te2.UnwrapNullableType();

            //脱空后tp1,tp2本底类型相同（ltu==rtu）,但rte（tp2)不是可空类型,则不管tp1是可空或不可空 返回tp1(可空类型优先,tp1优先)
            if (tu1 == tu2)  //本底类型相同
            {
                if (tu2 == te2)  //非可空 tp2
                {
                    elementType = te1;
                    return tp1;
                }
                else
                if (tu1 == te1)  //非可空 tp1
                {
                    elementType = te2;
                    return tp2;
                }
                else             //两个都可空，本底相同，同是集合，集合容器的类型不同（不会是非集合：那即是相等类型）
                {
                    elementType = te1;   //这种情况只出现在in查询，因此是两种容器也是等效的
                    return tp1;
                }
            }
            //如果本底类型不同，看是不是数值，比谁的精度高
            var type = MatchNumericalAccuracy(tu1, tu2);
            if (type == null)
                return null;

            if (type == tu1)
            {
                elementType = te1;
                return tp1;
            }
            elementType = te2;
            return tp2;

        }

        /// <summary>
        /// 比较两个数值类型的精度.
        /// </summary>
        /// <param name="numer1"></param>
        /// <param name="numer2"></param>
        /// <returns>返回兼容性更高（可空兼容，精度更高）的类型,若任何一个不是数值，则无法比较，返回空值.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static Type MatchNumericalAccuracy(Type numer1, Type numer2)
        {
            var nt1 = numer1.UnwrapNullableType();
            var nt2 = numer2.UnwrapNullableType();
            if (!nt1.IsNumeric() || !nt2.IsNumeric())
                return null;
            //比较精度
            Type matched = Type.GetTypeCode(nt1)>=Type.GetTypeCode(nt2) ? numer1:numer2;         
  
            if (nt1 == numer1 && nt2 == numer2) //两个都不可空
                return matched;
 
            if (matched == numer1)  //numer1精度高，
            {
                return matchNullable(matched, nt1, numer2, nt2);
            }
            else  //number2精度高
            {
                return matchNullable(matched, nt2, numer1, nt1);
            }

            Type matchNullable(Type matchOrg, Type matchUnwrap, Type otherOrg, Type otherUnwrap)
            {
                if (matchUnwrap != matchOrg) //高精度的可空
                    return matchOrg;
                if (otherUnwrap != otherOrg)  //另一个可空
                {
                    var typeKey = $"NullableOf{matchUnwrap}";
                    if (_nullAbleNumberTypeCache.TryGetValue(typeKey, out matchOrg))
                        return matchOrg;

                    matchOrg = (Type)Activator.CreateInstance(typeof(Nullable<>).GetGenericTypeDefinition(), new Type[] { matchUnwrap });
                    _nullAbleNumberTypeCache.Add(typeKey, matchOrg);
                    return matchOrg;
                }
                return matched;//两个都不可空
            }

        }
    }



}

