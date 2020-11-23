using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sag.Data.Common
{
    public static class LambdaExpressionExtensions

    {

        private static Expression Parser(ParameterExpression parameter, Expression expression)

        {

            if (expression == null) return null;

            switch (expression.NodeType)

            {

                //一元运算符 

                case ExpressionType.Negate:

                case ExpressionType.NegateChecked:

                case ExpressionType.Not:

                case ExpressionType.Convert:

                case ExpressionType.ConvertChecked:

                case ExpressionType.ArrayLength:

                case ExpressionType.Quote:

                case ExpressionType.TypeAs:

                    {

                        var unary = expression as UnaryExpression;

                        var exp = Parser(parameter, unary.Operand);

                        return Expression.MakeUnary(expression.NodeType, exp, unary.Type, unary.Method);

                    }

                //二元运算符 

                case ExpressionType.Add:

                case ExpressionType.AddChecked:

                case ExpressionType.Subtract:

                case ExpressionType.SubtractChecked:

                case ExpressionType.Multiply:

                case ExpressionType.MultiplyChecked:

                case ExpressionType.Divide:

                case ExpressionType.Modulo:

                case ExpressionType.And:

                case ExpressionType.AndAlso:

                case ExpressionType.Or:

                case ExpressionType.OrElse:

                case ExpressionType.LessThan:

                case ExpressionType.LessThanOrEqual:

                case ExpressionType.GreaterThan:

                case ExpressionType.GreaterThanOrEqual:

                case ExpressionType.Equal:

                case ExpressionType.NotEqual:

                case ExpressionType.Coalesce:

                case ExpressionType.ArrayIndex:

                case ExpressionType.RightShift:

                case ExpressionType.LeftShift:

                case ExpressionType.ExclusiveOr:

                    {

                        var binary = expression as BinaryExpression;

                        var left = Parser(parameter, binary.Left);

                        var right = Parser(parameter, binary.Right);

                        var conversion = Parser(parameter, binary.Conversion);

                        if (binary.NodeType == ExpressionType.Coalesce && binary.Conversion != null)

                        {

                            return Expression.Coalesce(left, right, conversion as LambdaExpression);

                        }

                        else

                        {

                            return Expression.MakeBinary(expression.NodeType, left, right, binary.IsLiftedToNull, binary.Method);

                        }

                    }

                //其他 

                case ExpressionType.Call:

                    {

                        var call = expression as MethodCallExpression;

                        List<Expression> arguments = new List<Expression>();

                        foreach (var argument in call.Arguments)

                        {

                            arguments.Add(Parser(parameter, argument));

                        }

                        var instance = Parser(parameter, call.Object);

                        call = Expression.Call(instance, call.Method, arguments);

                        return call;

                    }

                case ExpressionType.Lambda:

                    {

                        var Lambda = expression as LambdaExpression;

                        return Parser(parameter, Lambda.Body);

                    }

                case ExpressionType.MemberAccess:

                    {

                        var memberAccess = expression as MemberExpression;

                        if (memberAccess.Expression == null)

                        {

                            memberAccess = Expression.MakeMemberAccess(null, memberAccess.Member);

                        }

                        else

                        {

                            var exp = Parser(parameter, memberAccess.Expression);

                            var member = exp.Type.GetMember(memberAccess.Member.Name).FirstOrDefault();

                            memberAccess = Expression.MakeMemberAccess(exp, member);

                        }

                        return memberAccess;

                    }

                case ExpressionType.Parameter:

                    return parameter;

                case ExpressionType.Constant:

                    return expression;

                case ExpressionType.TypeIs:

                    {

                        var typeis = expression as TypeBinaryExpression;

                        var exp = Parser(parameter, typeis.Expression);

                        return Expression.TypeIs(exp, typeis.TypeOperand);

                    }

                default:

                    throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));

            }

        }

        public static Expression<Func<TToProperty, bool>> Cast<TInput, TToProperty>(this Expression<Func<TInput, bool>> expression)

        {

            var p = Expression.Parameter(typeof(TToProperty), "p");

            var x = Parser(p, expression);

            return Expression.Lambda<Func<TToProperty, bool>>(x, p);

        }

        //#region# 使用示例

        //class Program1test

        //{

        //    static int[] array0 = new[] { 0, 1 };

        //    static void Main(string[] args)

        //    {

        //        复杂表达式

        //        var array1 = new[] { 0, 1 };

        //        Expression<Func<UserDto, bool>> exp = u =>

        //            u.Id.Equals(1)

        //            && u.Name == "张三"

        //            && u.Id < 10

        //            && array1.Contains(u.Id)

        //            && u.Id + 2 < 10

        //            && (((object)u.Id).ToString() == "1" || u.Name.Contains("三"))

        //            && Math.Abs(u.Id) == 1

        //            && Filter(u.Name)

        //            && true

        //            ;

        //        Expression<Func<User, bool>> exp2 = exp.Cast<UserDto, User>();



        //        测试数据

        //        List<User> list = new List<User>() {

        //    new User{ Id=0,Name="AAA"},

        //    new User{ Id=1,Name="张三"},

        //    new User{ Id=2,Name="李四"}

        //};

        //        var item = list.Where(exp2.Compile()).FirstOrDefault();

        //        Console.WriteLine(item.Name);

        //        Console.ReadKey();

        //    }

        //    public static bool Filter(string name)

        //    {

        //        return name.Contains("三");

        //    }

        //}
        //#endregion
    }
}
