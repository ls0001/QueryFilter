using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sag.Data.Common.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Sag.Data.Common.Query.Tests
{
    [TestClass()]
    public class QueryFilterTests
    {

        [TestMethod()]
        public void MakeAirthmeticItemsExpr_NumericTest()
        {
            var type1 = typeof(entityTest);
            var item1 = new PropertyItem(type1, nameof(entityTest.IntPty));
            var item2= new PropertyItem(type1, nameof(entityTest.IntPty));
            var item3 = new PropertyItem(type1, nameof(entityTest.IntPty), typeof(decimal));
            var item4 = new ValueItem(10);
            var item5 = new ValueItem(20, typeof(float));
            var block = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, item1, item2,item3,item4,item5);

            var expEntity1 = Expression.Parameter(type1,"et1");
            var filter = QueryFilter.MakeAirthmeticItemsExpr(block, expEntity1);

            var str1=filter.ToString();
           Assert.IsTrue(str1== "(" +
               "(" +
               "(" +
               "(Convert(et1.IntPty, Decimal) + Convert(et1.IntPty, Decimal)) " +
               "+ Convert(et1.IntPty, Decimal)" +
               ") + 10" +
               ") + 20" +
               ")");
 
        }
   
        [TestMethod()]
        public void MakeAirthmeticItemsExpr_StringNumericTest()
        {
            var type1 = typeof(entityTest);
            var item1 = new PropertyItem(type1, nameof(entityTest.IntPty),typeof(float));
            var item2= new PropertyItem(type1, nameof(entityTest.IntPty));
            var item3 = new PropertyItem(type1, nameof(entityTest.IntPty), typeof(decimal));
            var item4 = new ValueItem("10",typeof(int));
            var item5 = new ValueItem(20, typeof(float));
            var block = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, item1, item2,item3,item4,item5);

            var expEntity1 = Expression.Parameter(type1,"et1");
            var filter = QueryFilter.MakeAirthmeticItemsExpr(block, expEntity1);

            var str1=filter.ToString();
           Assert.IsTrue(str1== "(" +
               "(" +
               "(" +
               "(Convert(Convert(et1.IntPty, Single), Decimal) + Convert(et1.IntPty, Decimal)) " +
               "+ Convert(et1.IntPty, Decimal)" +
               ") + 10" +
               ") + 20" +
               ")");
 
        }
        [TestMethod]
        public void MakeAirthmeticItemsExpr_StringDateTimeTest()
        {
            var type1 = typeof(entityTest);
            var item1 = new PropertyItem(type1, nameof(entityTest.DatetimePty), typeof(DateTime));
            var item2= new PropertyItem(type1, nameof(entityTest.DatetimePty));
            var item3 = new ValueItem("2020-01-01",typeof(DateTime));
            var item4 = new ValueItem(DateTime.Parse("2020-02-02"));
            item1.AddFunctions(ItemFunctionsFactory.GetFunction(DateTimeFunction.AddYears, 1))
                .AddFunctions(new DateTimeAddMonths(2))
                .AddFunctions(new DateTimeAddDays(3))
                .AddFunctions(new DateTimeAddHours(4))
                .AddFunctions(new DateTimeAddMinutes(5))
                .AddFunctions(new DateTimeAddSeconds(6))
                .AddFunctions(new DateTimeAddMilliseconds(7));
                

            var block = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates,  item1);

            var expEntity1 = Expression.Parameter(type1,"et1");
            var filter = QueryFilter.MakeAirthmeticItemsExpr(block, expEntity1);

            var str1=filter.ToString();
            //Assert.IsTrue(str1== "(" +
            //    "(" +
            //    "(" +
            //    "Convert(et1.DateTimePty, DateTime) + et1.DateTimePty" +
            //    ") + 2020-01-01" +
            //    ") + 2020-02-02" +
            //    ")");

        }

    }

    public class entityTest
    {
        public int IntPty { get; set; }
        public string StrPty { get; set; }
        public DateTime DatetimePty { get; set; }

    }

}

