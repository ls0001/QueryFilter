using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sag.Data.Common.Query;
using UnitsComm;

namespace UnitTestQueryExpression
{
    enum a
    {
        xx,
    }
    enum b
    {
        yy,
    }

    [TestClass]
    public class TestQueryCompareExpr
    {

        static Type _strType = typeof(string);
        static Type _dateType = typeof(DateTime);
        static Type _intType = typeof(int);
        static Type _doubType = typeof(Double);
        static Type _entityType = typeof(TestDataEntity);

        static PropertyInfo[] Propertys = typeof(TestDataEntity).GetProperties();
        static Expression[] propExpr = new Expression[Propertys.Length];
        //static Expression[] singValueExp = new Expression[props.Length];
        static dynamic[] singQueryValues = new dynamic[Propertys.Length];
        static Array listQueryValues = new Array[4];
        static List<TestDataEntity> entityList = new List<TestDataEntity>();
        static ParameterExpression entityParam = Expression.Parameter(_entityType, "ety");

        void CreateEntityList(int count)
        {
            
            for (int i = 0; i < count; i++)
            {
                TestDataEntity entity = new TestDataEntity()
                {
                    dateTimeProperty = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd")).AddDays(i)
                  ,
                    doubleProperty = 1 + i * 0.1
                  ,
                    intProperty = 1 + i
                  ,
                    strProperty = "stringPropertyValue:ABCDefghijklmnopq_" +(1+ i).ToString()
                };
                entityList.Add(entity);
            }

        }

        void CreateProperyExpressionAndQueryValues()
        {

            for (int i = 0; i < Propertys.Length; i++)
            {

                propExpr[i] = Expression.Property(entityParam, Propertys[i]);

                if (Propertys[i].PropertyType == typeof(string))
                {
                    listQueryValues.SetValue(new[] {"abc","stringPropertyValue:ABCDefghijklmnopq_1" , "efg", "a", "b"   }, i);
                    singQueryValues[i] = "stringPropertyValue:ABCDefghijklmnopq_1";
                }
                if (Propertys[i].PropertyType == typeof(int))
                {
                    listQueryValues.SetValue(new[] { 0, 1, 2, 3, 4, 5, 6 }, i);
                    singQueryValues[i] = 1;
                }
                if (Propertys[i].PropertyType == typeof(double))
                {
                    listQueryValues.SetValue(new[] { 1.0, 1.2, 1.3, 1.4 }, i);
                    singQueryValues[i] = 1;
                }
                if (Propertys[i].PropertyType == typeof(DateTime))
                {
                    var dt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd"));
                    listQueryValues.SetValue(new[] { dt, dt.AddDays(-1), dt.AddDays(1) }, i);
                    singQueryValues[i] = dt;

                }


            }
        }


        public TestQueryCompareExpr()
        {
            CreateEntityList(10);
            CreateProperyExpressionAndQueryValues();
        }


        [TestMethod]
        public void TestPropertyConvert()
        {

            for (var i = 0; i < propExpr.Length; i++)
            {
                var ety = entityList[0];
                var p = Propertys[i];

                var c = QueryFilter.MakeTypeConvertExpr(propExpr[i], _strType);

                unit.Log("  PropertyExpr_tostring:  {0} ", c.ToString());

                var r = runExpr<TestDataEntity, string>(entityParam, c, ety);

                unit.Log("  invoke:  {0}\r", r ?? "result_is_null");
            }
        }

        bool runExpr(Expression testExpr, int etyIndex = 0)
        {
            if (etyIndex >= 0)
            {
                var ety = entityList[etyIndex];
                return runExpr<TestDataEntity, bool>(entityParam, testExpr, ety);
            }
            var r = entityList.Where(Expression.Lambda<Func<TestDataEntity, bool>>(testExpr, entityParam).Compile());
            return r.Count() > 0 ? true : false;
        }


        R runExpr<TEty, R>(ParameterExpression entity, Expression testExpr, TEty etyInstance)
        {
            Expression<Func<TEty, R>> expr = Expression.Lambda<Func<TEty, R>>(testExpr, entity);
            Func<TEty, R> compiled = expr.Compile();

            unit.Log("{0}", expr.ToString());

            Expression<Func<R>> run = () => compiled(etyInstance);
            //unit.Log("{0}", run.ToString());

            Func<R> r = run.Compile();

            var result = r();
            return result;
        }

        private R TestBinary<R>(ParameterExpression entity, Expression expr)
        {
            Expression<Func<R>> f = Expression.Lambda<Func<R>>(expr, entity);

            Func<R> b = f.Compile();
            R rval = b();
            return rval;

        }
       
         
        [TestMethod]
        public void TestNotContains()
        {
            int i = 0;

            for (i = 0; i < propExpr.Length; i++)
            {
                var ety = entityList[0];
                var propExpr = TestQueryCompareExpr.propExpr[i];
                var paramName = Propertys[i].Name;
                var singQueryValue = singQueryValues[i];
                var propertyValue = Propertys[i].GetValue(ety);
                //var list = QueryFilter.TryMakeParameterizeQueryValue(listQueryValues.GetValue(i), propExpr.Type, Propertys[i].Name);

                var orgPropertyType = Propertys[i].PropertyType; 
                //orgType: i switch { 0 => _strType, 1 => _intType, 2 => _doubType, 3 => _dateType, _ => _strType };
                var unSamePropertType = i switch { 0 => _strType, 1 => _strType, 2 => _intType, 3 => _strType, _ => _strType };
                var unSameValueType   = i switch { 0 => _strType, 1 => _strType, 2 => _intType, 3 => _strType, _ => _strType };

                Expression expr;
                Expression orgType_Value;
                Expression unSameType_Value;
                bool b;
                orgType_Value = QueryFilter.MakeQueryValueExpr(singQueryValue, orgPropertyType,paramName);
                unSameType_Value = QueryFilter.MakeQueryValueExpr(singQueryValue,unSameValueType,paramName);

                Expression queryValue;

                //org
                queryValue = orgType_Value;
                expr = QueryFilter.MakeNotContainsExpr(propExpr, queryValue);
                b = runExpr<TestDataEntity, bool>(entityParam, expr, ety);
                Assert.IsFalse(b,string.Format(
                    " propertyName: {0,20}\r propertyType: {1,20}\r propertyValue: {2,20}\r conditionValueType: {3,20}\r conditionValue: {4,20}\r "
                    , paramName,orgPropertyType.Name, propertyValue,$"{queryValue.Type.Name}[{singQueryValue.GetType().Name}]",singQueryValue));

                //unSamePropertyType
                expr = QueryFilter.MakeNotContainsExpr(QueryFilter.MakeTypeConvertExpr(propExpr,unSamePropertType), orgType_Value);
                b = runExpr<TestDataEntity, bool>(entityParam, expr, ety);
                Assert.IsFalse(b);

                //unSameValueType
                expr = QueryFilter.MakeNotContainsExpr(propExpr, unSameType_Value);
                b = runExpr<TestDataEntity, bool>(entityParam, expr, ety);
                Assert.IsFalse(b);
            }
        }
         
        [TestMethod]
        public void TestInList()
        {

        }
         
        [TestMethod]
        public void TestNotInList()
        {
           
        }
         
        [TestMethod]
        public void TestEaual()
        { 

        }
         
        [TestMethod]
        public void TestNotEqual()
        {
            int i = 0;

        }
         
        [TestMethod]
        public void TestLessThen()
        {
            int i = 0;


        }


        [TestMethod]
        public void TestLessEqual()
        {


        }


        [TestMethod]
        public void TestGreaterThen()
        {
            
        }


        [TestMethod]
        public void TestGreaterEqual()
        {
            
        }


        [TestMethod]
        public void TestStartsWith()
        {

        }


        [TestMethod]
        public void TestEndsWith()
        {
        
        }


        [TestMethod]
        public void TestIsNull()
        {

        }


        [TestMethod]
        public void TestNotIsNull()
        {

        }
    }

}
