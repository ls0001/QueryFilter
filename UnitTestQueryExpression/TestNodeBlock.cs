using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;
using Sag.Data.Common.Query;

namespace UnitTestQueryExpression
{
    [TestClass]
    public class TestNodeBlock
    {
        [TestMethod]
        public void NodeBlockGetHashCode()
        {
            var item0 = new ValueItem("file0");
            var item1 = new ValueItem() {  Value = "Field1" };
            var item2 = new ValueItem("field2") {  };
            var item3 = new ValueItem("field3object") {  };
            var item4 = new ValueItem( "field4");
            var item5 = new ValueItem("Field5object");
            var listPtys1 = new List<ValueItem>() { item0, item1, item2, item3, item4, item5 };

            var bk1 = new NodeBlock();
            var bk2 = new NodeBlock();
            Assert.IsTrue(bk1 == bk2);
            
            var bk11 = new NodeBlock() { AutoReduce = true};
            bk11.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item1, item2);
            var bk12 = new NodeBlock() { AutoReduce = true };
            bk12.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item1, item2);

            Assert.IsTrue(bk11.GetHashCode() == bk12.GetHashCode());

            var bk13 = new NodeBlock() { AutoReduce = false};
            bk13.Add(QueryArithmetic.Mod, InsertionBehavior.Duplicates, item1, item2);
            Assert.IsTrue(bk11.GetHashCode() != bk13.GetHashCode());

            var bk14 = new NodeBlock() { AutoReduce = false };
            bk14.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item2, item3);
            Assert.IsTrue(bk11.GetHashCode() != bk14.GetHashCode());
            UnitsComm.unit.Log("{0},{1},{2},{3}", bk11.GetHashCode(), bk12.GetHashCode(), bk13.GetHashCode(), bk14.GetHashCode());



            var bk31 = new NodeBlock();
            var bk31b = new NodeBlock();
            var bk31c = new NodeBlock();
            Assert.IsTrue(bk31 == bk31b);

            //一个相同项
            bk31.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item3);
            bk31b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item3);
            UnitsComm.unit.Log("Equal=> h1:{0},  h2:{1} \r", bk31.GetHashCode(), bk31b.GetHashCode());
            Assert.IsTrue(bk31 == bk31b);

            //一个相同项，不同运算符
            bk31c.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item3);
            UnitsComm.unit.Log("NotEqual=> h1:{0},  h2:{1} \r", bk31.GetHashCode(), bk31c.GetHashCode());
            Assert.IsTrue(bk31 != bk31c);

            //再加一个相同项
            bk31.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item2);
            bk31b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item2);
            UnitsComm.unit.Log("Equal=> h1:{0},  h2:{1} \r", bk31.GetHashCode(), bk31b.GetHashCode());
            Assert.IsTrue(bk31 == bk31b);

            bk31c = new NodeBlock();
            bk31c.Items = bk31.Items;
            bk31c.Blocks = bk31.Blocks;
            bk31c.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item1, item4);
            var bk31d = new NodeBlock() { Items = bk31b.Items, Blocks = bk31b.Blocks };
            bk31d.Add(QueryArithmetic.Divide, InsertionBehavior.Duplicates, item4, item1);
            Assert.IsTrue(bk31c != bk31d);
             

            bk31c = new NodeBlock();
            bk31c.Items = bk31.Items;
            bk31c.Blocks = bk31.Blocks;
            //一个相同块
            bk31.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk11);
            bk31b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk11);
            UnitsComm.unit.Log("Equal=> h1:{0},  h2:{1} \r", bk31.GetHashCode(), bk31b.GetHashCode());
            Assert.IsTrue(bk31 == bk31b);

            //一个相同块，不同运算符
            bk31c.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, bk11);
            UnitsComm.unit.Log("NotEqual=> h1:{0},  h2:{1} \r", bk31.GetHashCode(), bk31c.GetHashCode());
            Assert.IsTrue(bk31 != bk31c);

            //再加一个相同块
            bk31.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk12);
            bk31b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk12);
            UnitsComm.unit.Log("Equal=> h1:{0},  h2:{1} \r", bk31.GetHashCode(), bk31b.GetHashCode());
            Assert.IsTrue(bk31 == bk31b);


            //来个不同块
            bk31.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk13);
            bk31b.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk12);
            UnitsComm.unit.Log("NotEqual=> h1:{0},  h2:{1} \r", bk31.GetHashCode(), bk31b.GetHashCode());
            Assert.IsTrue(bk31 != bk31b);


            
            bk31c = new NodeBlock();
            bk31c.Items = bk31b.Items;
            bk31c.Blocks = bk31b.Blocks;
            Assert.IsTrue(bk31c == bk31b);

            //*添加空块,不同运算符
            var bk21 = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk1);
            var bk21a = new NodeBlock(QueryArithmetic.Sub, InsertionBehavior.Duplicates, bk1);
            Assert.IsTrue(bk21 == bk21a);
            Assert.IsTrue(bk21 == bk1);

            //加一个项
            bk21a.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item1);
            Assert.IsTrue(bk21 != bk21a);

            //加一个块
            bk21.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk11);
            Assert.IsTrue(bk21 != bk21a);

            //添加一个相同实块
            var bk22 = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk11);
            Assert.IsTrue(bk22 == bk21);

            var bk22a = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk11);
            Assert.IsTrue(bk22 == bk22a);


            //添加一个相实块,不同运算符
            var bk22c = new NodeBlock(QueryArithmetic.Sub, InsertionBehavior.Duplicates, bk11);
            Assert.IsTrue(bk22c != bk22a);

            //相同添加两个实块
            var bk23 = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk13, bk12);
            var bk23a = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk13, bk12);
            Assert.IsTrue(bk23 == bk23a);

            //添加两个相同实块，不同运算符
            var bk23b = new NodeBlock(QueryArithmetic.Sub, InsertionBehavior.Duplicates, bk13, bk12);
            Assert.IsTrue(bk23b != bk23a);

            //添加一个项
            bk23a.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item2);
            Assert.IsTrue(bk23a != bk23);
            //相同项，不同运行符
            bk23.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item2);
            Assert.IsTrue(bk23a != bk23);

            //两个不同块
            var bk23d = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, bk11, bk13);
            Assert.IsTrue(bk23d != bk23);


        }

        [TestMethod]
        public void NoedeItemGetHashCode()
        {
            
            var item0 = new ValueItem("file0");
            var item1 = new ValueItem(){ Value= "Field1" };
            var item2 = new ValueItem("field2") {  };
            var item3 = new ValueItem("field3object") {  };
            var item4 = new ValueItem( "field4");
            var item5 = new ValueItem( "Field5object");
            var listPtys1 = new List<ValueItem>() { item0, item1, item2, item3, item4, item5 };

            var item00 = new ValueItem("file0");
            var item01 = new ValueItem() {  Value = "Field1" };
            var item02 = new ValueItem("field2") {  };
            var item03 = new ValueItem("field3object") { };
            var item04 = new ValueItem("field4");
            var item05 = new ValueItem("Field5object");
            var listPtys01 = new List<ValueItem>() { item00, item01, item02, item03, item04, item05 };

            var item00p = new ValueItem();
            var item01p = new ValueItem() {  Value = "Field1" };
            var item02p = new ValueItem("field2") {  };
            var item03p = new ValueItem("field3object") {  };
            var item04p = new ValueItem( "field4");
            var item05p = new ValueItem( "Field5object");
            var listPtysP = new List<ValueItem>() { item00p, item01p, item02p, item03p, item04p, item05p };


            for(int i=0;i<6;i++)
            {
                var hc1 = listPtys1[i].GetHashCode();
                var hc2 = listPtys01[i].GetHashCode();
                var hc3 = listPtysP[i].GetHashCode();
                UnitsComm.unit.Log("h1:{0} ,\rh2: {1}, \rh3:{2} ,\r", hc1, hc2, hc3);
                Assert.IsTrue(hc1 == hc2);
                Assert.IsTrue(hc1 != hc3);
            }
            UnitsComm.unit.Log("====================================================================");

            var dt = DateTime.Now;
            var item0v = new ValueItem("value1");
            var item1v = new ValueItem(){ Value= new[] { 1,2} };
            var item2v = new ValueItem(new[] { "a","b"}) {  };
            var item3v = new ValueItem(new[] {dt,dt.AddDays(1) }) {  };
            var item4v = new ValueItem(true );
            var item5v = new ValueItem(100);
            var item6v = new ValueItem(new[] { true, false });
            var listValue1 = new List<ValueItem>() { item0v, item1v, item2v, item3v, item4v, item5v, item6v };

            var item00v = new ValueItem("value1");
            var item01v = new ValueItem(){ Value= new[] { 1,2} };
            var item02v = new ValueItem(new[] { "a","b"}) {  };
            var item03v = new ValueItem(new[] {dt,dt.AddDays(1) }) { };
            var item04v = new ValueItem(true );
            var item05v = new ValueItem( 100);
            var item06v = new ValueItem( new[] { true, false });
            var listValue01 = new List<ValueItem>() { item00v, item01v, item02v, item03v, item04v, item05v, item06v };

            var item00vp = new ValueItem();
            var item01vp = new ValueItem(){ Value= new[] { "1","2"} };
            var item02vp = new ValueItem(new[] { "2","3"}) {  };
            var item03vp = new ValueItem(new[] {dt,DateTime.Now.AddDays(2) }) {  };
            var item04vp = new ValueItem(false );
            var item05vp = new ValueItem( 0);
            var item06vp = new ValueItem( new[] { false, true });
            var listValueP = new List<ValueItem>() { item00vp, item01vp, item02vp, item03vp, item04vp, item05vp, item06vp };


            for (int i = 0; i < 6; i++)
            {
                var hc1 = listValue1[i].GetHashCode();
                var hc2 = listValue01[i].GetHashCode();
                var hc3 = listValueP[i].GetHashCode();
                UnitsComm.unit.Log("h1:{0} ,\rh2: {1}, \rh3:{2} ,\n\r", hc1, hc2, hc3);
                Assert.IsTrue(hc1 == hc2);
                Assert.IsTrue(hc1 != hc3);
            }

        }


        [TestMethod]
        public void ConditionItemGetHashCode()
        {
            var item1 = new ValueItem("aa");
            var item2 = new ValueItem("bb");
            var cditem = new ConditionItem();
            cditem.PropertyBlock.AddItem(QueryArithmetic.Add, InsertionBehavior.Duplicates,item1);
            cditem.ValueBlock.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item2);

            var cditem2 = new ConditionItem();
            cditem2.PropertyBlock.AddItem(QueryArithmetic.Add, InsertionBehavior.Duplicates, item1);
            cditem2.ValueBlock.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, item2);



            UnitsComm.unit.Log(cditem.GetHashCode());
            UnitsComm.unit.Log(cditem2.GetHashCode());

            Assert.IsTrue(cditem == cditem2);


        }
    }
}
