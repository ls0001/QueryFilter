using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sag.Data.Common.Query;

namespace UnitTestQueryExpression
{

    [TestClass]
    public class TestQueryPropertyBlock
    {
        [TestMethod]
        public void PropertyBlockAddItem()
        {
            var b = new NodeBlock();
            var p = new ValueItem("field1");
            b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p);
            b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p);
            b.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, p);
            var p2 = new ValueItem("field2");
            p2.TypeAs = typeof(int);
            b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p2);

            Assert.IsTrue(b.AllCount == 4 && b.ItemCount == 4);

            var p3 = new ValueItem("field2");
            b.Add(QueryArithmetic.Add, InsertionBehavior.Overwrite, p3);
            Assert.IsTrue(b.AllCount == 4 && b.ItemCount == 4);

            var p4 = new ValueItem("field2");
            b.Add(QueryArithmetic.Add, InsertionBehavior.IgnoreExists, p4);
            Assert.IsTrue(b.AllCount == 4 && b.ItemCount == 4);


        }


        [TestMethod]
        public void PropertyBlockAddBlock()
        {
            var b1 = new NodeBlock();
            var p = new ValueItem("field1");
            b1.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p);
            b1.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p);
            b1.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, p);

            var b2 = new NodeBlock();
            p = new ValueItem("field1");
            b2.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p);
            b2.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p);
            b2.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, p);


            var b3 = new NodeBlock();
            p = new ValueItem("field1");
            b3.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, p);

            var bb = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.IgnoreExists, b1, b2, b3);
            p = new ValueItem("field2");
            bb.Add(QueryArithmetic.Add, InsertionBehavior.IgnoreExists, p);
            Assert.IsTrue(bb.AllCount == 3 && bb.ItemCount == 1);

            bb.Add(QueryArithmetic.Add, InsertionBehavior.Overwrite, b1,b2,b3);
            Assert.IsTrue(bb.AllCount == 3 && bb.ItemCount == 1);

            bb.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, b1, b2, b3);
            Assert.IsTrue(bb.AllCount == 6 && bb.ItemCount == 1);

            Assert.IsTrue(bb.Blocks[0].Node == b1 && bb.Blocks[1].Node == b3 && bb.Blocks[2].Node == b1
                && bb.Blocks[3].Node == b2 && bb.Blocks[4].Node== b3
                );
            
        }

        [TestMethod]
        public void QueryFilterAddItem()
        {
            var propItem = new ValueItem("intProperty");
            var propBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, propItem);
            var queryItem = new ConditionItem();
            queryItem.Comparison = QueryComparison.Equal;
            //queryItem.DataType = typeof(int);
            queryItem.PropertyBlock = propBlock;
            //queryItem.Value = 21;
            var queryBlock = new ConditionBlock(QueryLogical.And,queryItem);

            var filter = QueryFilter.CreateFilterExpression<TestDataEntity>(queryBlock);
            
        }


    }

}
