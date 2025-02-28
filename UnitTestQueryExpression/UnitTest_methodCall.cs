using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System;
using Sag.Data.Common;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sag.Data.Common.Query;

namespace UnitTestQueryExpression
{
    [TestClass]
    public class UnitTest_methodCall
    {
        [TestMethod]
        public void TestCreateData()
        {
            var dSet = DataGeter.GetTestEntitySet();
            Logger.LogMessage(string.Format("RecordCount:{0}", dSet.Count));
        }

        [TestMethod]
        public void TestGetMethod()
        {
            var lstTypes = new List<Type>()
            {typeof(List<string>)
            ,typeof(List<int>)
            ,typeof(List<double>)
            ,typeof(List<DateTime>)

            };



            foreach (var m in lstTypes)
            {
                var itemType = m.GetGenericArguments()[0];
                var r = QueryFilter.GetMethod(m, CallMethodId.ListAddItem, itemType);
                Logger.LogMessage(
                    String.Format(" Type: {0} , Method: {1} , ElementType: {2} , Result: {3} ",
                    m.Name, "ListAdd", itemType?.Name, r?.Name));

                r = QueryFilter.GetMethod(m, CallMethodId.ListContains, itemType);
                Logger.LogMessage(
                    String.Format(" Type: {0} , Method: {1} , ElementType: {2} , Result: {3} ",
                    m.Name, "ListContains", itemType?.Name, r?.Name));
            }


        }
 
        [TestMethod]
        public void TestGetMethod_String()
        {

            var testTypes = new List<Type>() { typeof(string) };
            var methNameId = new List<CallMethodId>() {
                CallMethodId.StringCompare,
                CallMethodId.StringContains,
                CallMethodId.StringEndsWith,
                CallMethodId.StringIsNullOrEmpty,
                CallMethodId.StringStartsWith
            };
            var methParamTypes = new List<Type[]>()
            {
                new []{typeof(string),typeof(string),typeof(bool)}
                ,new []{typeof(string)}
                ,new []{typeof(string)}
                ,new []{typeof(string)}
                ,new []{typeof(string)}
            };
            foreach (var m in testTypes)
            {

                foreach (var id in methNameId)
                {
                    var itemType = methParamTypes[methNameId.IndexOf(id)];
                    var r = QueryFilter.GetMethod(m,id, itemType);
                    Logger.LogMessage(
                        String.Format(" Type:{0} , Method:{1} , ElementType:{2} , Result: {3} ",
                        m.Name, Enum.GetName(typeof(CallMethodId), id), itemType?.ToString(), r?.Name));
                }
            }

        }

        [TestMethod]
        public void TestMethodCall_String()
        {
            var testTypes = new List<Type>() { typeof(string) };
            var methNameId = new List<CallMethodId>() {
                CallMethodId.StringCompare,
                CallMethodId.StringContains,
                CallMethodId.StringEndsWith,
                CallMethodId.StringIsNullOrEmpty,
                CallMethodId.StringStartsWith
            };
            var methParamsType = new List<Type[]>()
            {
                new []{typeof(string),typeof(string)}
                ,new []{typeof(string)}
                ,new []{typeof(string)}
                ,new []{typeof(string)}
                ,new []{typeof(string)}
            };
            var methParamValue = new List<string[]>()
            {
                new string[]{"abcdefg","efg"}
                ,new string[]{"cde"}
                ,new string[]{"efg"}
                ,new string[]{"isnulloremptyInstanct"}
                ,new string[]{ "abcd" }
            };

            foreach (var m in testTypes)
            {
                foreach (var id in methNameId)
                {
                    var instanctObj = "abcdefg";
                    var index = methNameId.IndexOf(id);
                    var paramValues = methParamValue[index];
                    var paramExprs = new Expression[paramValues.Length];
                    for (var i = 0; i < paramValues.Length; i++)
                    {
                        paramExprs[i] = Expression.Constant(paramValues[i]);
                    }
                    var argTypes = methParamsType[index];
                    var call = QueryFilter.MakeMethodCallExpr(  id, argTypes,Expression.Constant( instanctObj), paramExprs);
                    Logger.LogMessage(
                        String.Format(" Type:{0} , Method:{1} , ElementType:{2} , Result: {3} ",
                        m.Name, Enum.GetName(typeof( CallMethodId), id), argTypes?.ToString(), call?.ToString()));

                    dynamic exprCode;
                    if (index == 0)
                        exprCode = Expression.Lambda<Func<string, int>>(call, Expression.Parameter(instanctObj.GetType())).Compile();
                    else
                        exprCode = Expression.Lambda<Func<string, bool>>(call, Expression.Parameter(instanctObj.GetType())).Compile();
                    var exprRun = exprCode(instanctObj);
                    
                    Logger.LogMessage("expr_run:  {0} ", exprRun);
                }
            }
        }

        [TestMethod]
        public void TestMethodCall_List()
        {
            var testTypes = new List<Type>() { typeof(List<string>) };
            var methNameId = new List< CallMethodId>() {
                    CallMethodId.ListAddItem,
                    CallMethodId.ListContains
            };
            var methParamsType = new List<Type[]>()
            {
                new []{typeof(string)}
                ,new []{typeof(string)}
            };
            var methParamValue = new List<string[]>()
            {
                new string[]{"abcd"}
                ,new string[]{"efg"}
            };
            foreach (var m in testTypes)
            {
                Logger.LogMessage("Type: {0} :{1}", m?.Name
                    ,"================================");

                var instanctObj = new List<string>();
                foreach (var id in methNameId)
                {

                    var index = methNameId.IndexOf(id);
                    var paramValues = methParamValue[index];
                    var paramExprs = new Expression[paramValues.Length];
                    for (var i = 0; i < paramValues.Length; i++)
                    {
                        paramExprs[i] = Expression.Constant(paramValues[i]);
                    }
                    var argTypes = methParamsType[index];
                    var r = QueryFilter.MakeMethodCallExpr(  id, argTypes, Expression.Constant(instanctObj), paramExprs);

                    Logger.LogMessage(" MethodName:  {0}:{1}",
                        Enum.GetName(typeof( CallMethodId), id)
                        ,"-----------------------------------");
                    Logger.LogMessage(
                        String.Format("     ElementType:    {0} ,   Result: {1} ", 
                        argTypes?.ToString()??"null_arg_type", 
                        r?.ToString()??"expr_result_isnull"));

                    var exprCode = Expression.Lambda(r, Expression.Parameter(instanctObj.GetType())).Compile();
                    var x = exprCode.DynamicInvoke(instanctObj);
                    Logger.LogMessage("     expr_listCount:     {0}     expr_run:  {1} ", 
                        instanctObj.Count,
                        x??"null_returnValue");
                    
                    

                }
            }
        }


        [TestMethod]
        public void TestSampleEqual()
        {
            var dSet = DataGeter.GetTestEntitySet();

        }


        [TestMethod]
        public void TestGetMethod_ValueType()
        {
            var valType = new List<Type>()
            {
                typeof(int),typeof(double),typeof(DateTime)
            };


        }


        [TestMethod]
        public void TestCreateList()
        {

            var count = 10;
            var elementType = typeof(int);
            var value = new int[count];
            var parameters = new ParameterExpression[count];
            for (int i = 0; i < value.Length; i++)
            {
                parameters[i] = Expression.Parameter(elementType, "lst_" + i.ToString());
                value[i] = i; // Expression.Constant(i);
            }
            var listExpr= QueryFilter.MakeListExpr(value, elementType);
            var lmbda = Expression.Lambda<Func<List<int>>>(listExpr);
            var list = lmbda.Compile()();
          //  for(int i=0;i<value.Length;i++)
          //      Assert.IsTrue(list[i] == value[i]);

        }

    }
}
