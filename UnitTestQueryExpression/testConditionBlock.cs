using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json;
using Sag.Data.Common.Query;
using Sag.Data.Common.Query.Internal;
using UnitsComm;

namespace UnitTestQueryExpression
{
    [TestClass]
    public class TestConditionBlock
    {
        JsonSerializerOptions opts = new JsonSerializerOptions();

        public TestConditionBlock()
        {
            opts.IgnoreReadOnlyProperties = true;
        }
        [TestMethod]
        public void TestExpressionAdd()
        {
            Expression left = Expression.Constant(1.2f, typeof(float));
            Expression right = Expression.Constant(1, typeof(int));
            var add=Expression.Add(left, right);
            Expression<Func<int>> funcExpr = Expression.Lambda<Func<int>>(add);
            Func<int> func = funcExpr.Compile();
            var r=func();

        }

        [TestMethod]
        public void TestConditionToJson()
        {
            var cb = new ConditionBlock();
            var ci1 = new ConditionItem("item1", QueryComparison.Equal, 100, null);
            // var ci2 = new ConditionItem("item2", QueryComparison.InList, new List<string>() { "a", "b", "c" }, typeof(string));

            var jsoni = JsonSerializer.Serialize<ConditionItem>(ci1);

            var jsonItem = JsonSerializer.Deserialize<ConditionItem>(jsoni);

            cb.AutoReduce = false;
            cb.Add(QueryLogical.And, InsertionBehavior.Duplicates, ci1);
            //   cb.AddItem(QueryLogical.And, InsertionBehavior.Duplicates, ci2);

            var json = JsonSerializer.Serialize(cb);

            // var json = Newtonsoft.Json.JsonConvert.SerializeObject(cb);
            var opt = new JsonSerializerOptions();
            // opt.Converters.Add(new NodeOperatorJsonConverter());

            var cbr = JsonSerializer.Deserialize<ConditionBlock>(json);
            // var cbrt = (ConditionBlock)cbr;\

            var itr = cbr.Items[0].Node;

            Assert.IsTrue(cb == cbr);
        }


        [TestMethod]
        public void TestNodeBlockJson()
        {
            var item1 = new ValueItem(new[] { "1", "2" });

            var item2 = new ValueItem( new[] { "a", "b" });
            var pty1 = new PropertyItem(typeof(entTest), nameof(entTest.MyStringProperty),typeof(string));
            var item4 = new ValueItem(null);
           
            var nb2 = new NodeBlock() { TypeAs = typeof(List<string>) };
            nb2.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item2);
            var nb = new NodeBlock();
            nb.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, nb2);
            nb.Add(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item1);
            nb.Add(QueryArithmetic.Divide, InsertionBehavior.Duplicates, pty1);
            nb.Add(QueryArithmetic.Divide, InsertionBehavior.Duplicates, item4);

            var newtonJson = Newtonsoft.Json.JsonConvert.SerializeObject(nb);
            var json = System.Text.Json.JsonSerializer.Serialize<NodeBlock>(nb);
            //json = Newtonsoft.Json.JsonConvert.SerializeObject(nb);
            //json = nb.ToJson();
            NodeBlock jsonNb = null;
            for (int i = 0; i < 10000; i++)
            {
                 jsonNb = System.Text.Json.JsonSerializer.Deserialize<NodeBlock>(json);
            }
            Assert.IsTrue(nb == jsonNb);
        }


        [TestMethod]
        public void TestNodeBlockNewtonJson()
        {
            var item1 = new ValueItem(  new[] { 1, 2 });
            var nb = new NodeBlock(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item1) { TypeAs = typeof(int) };
            var item2 = new ValueItem(  new[] { "a", "b" });
            var nb2 = new NodeBlock(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item2) { TypeAs = typeof(string) };
            nb.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, nb2);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(nb);
            for (int i = 0; i < 10000; i++)
            {
                var jsonNb = Newtonsoft.Json.JsonConvert.DeserializeObject<NodeBlock>(json);
            }

        }

        [TestMethod]
        public void TestConditionResetFromMember2()
        {

            //var its = nb.Items;
            //var bks = nb.Blocks;
            //var nbk = new NodeBlock();


            for (int i = 0; i < 100000; i++)
            {
                //var   m_items = new NodeCollection<QueryArithmetic, NodeItem>();
                //var   m_blocks = new NodeCollection<QueryArithmetic, NodeItem>();

                var item1 = new ValueItem( new[] { 1, 2 });
                var nb = new NodeBlock(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item1) { TypeAs = typeof(int) };
                var item2 = new ValueItem( new[] { "a", "b" });
                var nb2 = new NodeBlock(QueryArithmetic.Sub, InsertionBehavior.Duplicates, item2) { TypeAs = typeof(string) };
                nb.Add(QueryArithmetic.Add, InsertionBehavior.Duplicates, nb2);
                //var autoReduceName = nameof(nb.AutoReduce);
                //var typeAsName = nameof(nb.TypeAs);
                //var itemsName = nameof(nb.Items);
                //var blocksName = nameof(nb.Blocks);
                //var autoReduceName = nameof(QueryBlock<NodeItem, NodeBlock, QueryArithmetic>.AutoReduce);
                //var typeAsName = nameof(QueryBlock<NodeItem, NodeBlock, QueryArithmetic>.TypeAs);
                //var itemsName = nameof(QueryBlock<NodeItem, NodeBlock, QueryArithmetic>.Items);
                //var blocksName = nameof(QueryBlock<NodeItem, NodeBlock, QueryArithmetic>.Blocks);
            }

        }
        [TestMethod]
        public void TestNodeCollectionResetFromMember()
        {
            var c1 = new NodeCollection<QueryArithmetic, ValueItem>();
            var i1 = new NodeOperatorPair<QueryArithmetic, QueryItem>() { Operator = QueryArithmetic.Add, Node = new ValueItem("s1") };
            var i2 = new NodeOperatorPair<QueryArithmetic, QueryItem>() { Operator = QueryArithmetic.Sub, Node = new ValueItem("s2") };
            var cm1 = new NodeOperatorPair<QueryArithmetic, QueryItem>[] { i1, i2 };
           // c1.Items = (NodeCollection<QueryArithmetic, NodeItem>)c1;// cm1;

            var c1e = new NodeCollection<QueryArithmetic, ValueItem>();
            foreach (var n in cm1)
            {
                c1e.Add(n.Operator, (ValueItem)n.Node, InsertionBehavior.Duplicates);
            }
            Assert.IsTrue(c1.Items.Length == cm1.Length);
            for (var i = 0; i < c1e.Items.Length; i++)
            {
                var item = c1e.Items[i];
                Assert.IsTrue(item.Operator == c1.Items[i].Operator);
                Assert.IsTrue(item.Node == c1.Items[i].Node);
            }

            var c2 = new NodeCollection<QueryLogical, NodeBlock>();
            var b1 = new NodeOperatorPair<QueryLogical, NodeBlock>() { Operator = QueryLogical.Not, Node = new NodeBlock() };
            var b2 = new NodeOperatorPair<QueryLogical, NodeBlock>() { Operator = QueryLogical.Not, Node = new NodeBlock(QueryArithmetic.Mod, InsertionBehavior.Duplicates, new ValueItem("N1")) };
            var b3 = new NodeOperatorPair<QueryLogical, NodeBlock>() { Operator = QueryLogical.Or, Node = new NodeBlock() { Items =  cm1 } };
            var bm1 = new NodeOperatorPair<QueryLogical, NodeBlock>[] { b1, b2, b3 };
            c2.Items = bm1;

            var b1e = new NodeCollection<QueryLogical, NodeBlock>();
            foreach (var n in bm1)
            {
                b1e.Add(n.Operator, n.Node, InsertionBehavior.Duplicates);
            }
            Assert.IsTrue(c2.Items.Length == bm1.Length);
            for (var i = 0; i < b1e.Items.Length; i++)
            {
                var item = b1e.Items[i];
                Assert.IsTrue(item.Operator == c2.Items[i].Operator);
                Assert.IsTrue(item.Node == c2.Items[i].Node);
            }



        }

        [TestMethod]
        public void TestNodeBlockResetFromMember()
        {
            var i1 = new NodeOperatorPair<QueryArithmetic, ValueItem>() { Operator = QueryArithmetic.Add, Node = new ValueItem("s1") };
            var i2 = new NodeOperatorPair<QueryArithmetic, ValueItem>() { Operator = QueryArithmetic.Add, Node = new ValueItem("s2") };
            var cm1 = new NodeOperatorPair<QueryArithmetic, ValueItem>[] { i1, i2 };

            var c2 = new NodeCollection<QueryArithmetic, NodeBlock>();
            var b1 = new NodeOperatorPair<QueryArithmetic, NodeBlock>() { Operator = QueryArithmetic.Sub, Node = new NodeBlock() };
            var b2 = new NodeOperatorPair<QueryArithmetic, NodeBlock>();//{ Operator = QueryArithmetic.Sub, Node = new NodeBlock() { Items = cm1 } };
            var bm1 = new NodeOperatorPair<QueryArithmetic, NodeBlock>[] { b1, b2 };

            var b3 = new NodeBlock();
            //b3.Items = cm1;
            b3.Blocks = bm1;
            Debug.Assert(b3.ItemCount == 2 && b3.BlockCount == 2);
            //Debug.Assert(b3.Items[0] == i1);
            Debug.Assert(b3.Blocks[1] == b2);
        }

        [TestMethod]
        public void TestNodeCollectionJson()
        {
            var nodes = new NodeCollection<QueryArithmetic, ValueItem>();
            var item1 = new ValueItem(  new List<string>() { "a", "b" });
            var jsonItem = System.Text.Json.JsonSerializer.Serialize<ValueItem>(item1);
            var jItem = System.Text.Json.JsonSerializer.Deserialize<ValueItem>(jsonItem);
            Assert.IsTrue(item1 == jItem);

            nodes.Add(QueryArithmetic.Add, item1);
            nodes.Add(QueryArithmetic.Add, item1, InsertionBehavior.Duplicates);
            nodes.Add(QueryArithmetic.Add, item1);

            var json = System.Text.Json.JsonSerializer.Serialize(nodes);
            var nodesJ = (NodeCollection<QueryArithmetic, ValueItem>)System.Text.Json.JsonSerializer.Deserialize(json, nodes.GetType());
            json = Newtonsoft.Json.JsonConvert.SerializeObject(nodes);
            var nodesFromNewtonJson = (NodeCollection<QueryArithmetic, ValueItem>)System.Text.Json.JsonSerializer.Deserialize(json, nodes.GetType());

            Assert.IsTrue(nodesJ.Items[0].Node.GetHashCode() == nodesFromNewtonJson.Items[0].Node.GetHashCode());

            // json = System.Text.Json.JsonSerializer.Serialize(nodes);
            // json = Newtonsoft.Json.JsonConvert.SerializeObject(nodes);
            //var  nodesNewton = Newtonsoft.Json.JsonConvert.DeserializeObject<NodeCollection<QueryArithmetic, NodeItem>>(json);

            
            var count = 20000;

            for (int j = 0; j < 3; j++)
            {
                var sw = Stopwatch.StartNew();
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    json = System.Text.Json.JsonSerializer.Serialize(nodes);
                }
                sw.Stop();
                UnitsComm.unit.Log("优化to Json{0}:", sw.ElapsedMilliseconds);

                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(nodes);
                }
                sw.Stop();
                UnitsComm.unit.Log("NewtonSoft to Json{0}:", sw.ElapsedMilliseconds);


                json = System.Text.Json.JsonSerializer.Serialize(nodes);
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    nodesJ = (NodeCollection<QueryArithmetic, ValueItem>)System.Text.Json.JsonSerializer.Deserialize(json, nodes.GetType());
                }
                sw.Stop();
                UnitsComm.unit.Log("优化 from 优化Json{0}:", sw.ElapsedMilliseconds);

                json = Newtonsoft.Json.JsonConvert.SerializeObject(nodes);
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    nodesJ = (NodeCollection<QueryArithmetic, ValueItem>)System.Text.Json.JsonSerializer.Deserialize(json, nodes.GetType());
                }
                sw.Stop();
                UnitsComm.unit.Log("优化 from NewtonSoft Json{0}:", sw.ElapsedMilliseconds);


                //json = Newtonsoft.Json.JsonConvert.SerializeObject(nodes);
                //sw.Restart();
                //for (int i = 0; i < count; i++)
                //{
                //    nodesFromNewtonJson = (NodeCollection<QueryArithmetic, NodeItem>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, nodes.GetType());
                //}
                //sw.Stop();
                //UnitsComm.unit.Log("NewtonSoft from Json{0}:", sw.ElapsedMilliseconds);

                //sw.Restart();
                //for (int i = 0; i < count; i++)
                //{
                //    json = System.Text.Json.JsonSerializer.Serialize(nodes);
                //    nodesJ = (NodeCollection<QueryArithmetic, NodeItem>)System.Text.Json.JsonSerializer.Deserialize(json, nodes.GetType());
                //}
                //sw.Stop();
                //UnitsComm.unit.Log("全程优化  Json{0}:", sw.ElapsedMilliseconds);

                //sw.Restart();
                //for (int i = 0; i < count; i++)
                //{
                //    json = Newtonsoft.Json.JsonConvert.SerializeObject(nodes);
                //    nodesFromNewtonJson = (NodeCollection<QueryArithmetic, NodeItem>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, nodes.GetType());
                //}
                //sw.Stop();
                //UnitsComm.unit.Log("全程NewtonSoft from Json{0}:", sw.ElapsedMilliseconds);


            }


        }

        [TestMethod]
        public void TestNodeOperatorPair()
        {
            var opt = new JsonSerializerOptions();
            //opt.Converters.Add(new ItemOperatorJsonConverter());
            opt.Converters.Add(new NodeOperatorJsonConverter());
            var item = new ValueItem( new int[] { 1, 2, 3 });
            var pair = new NodeOperatorPair<QueryArithmetic, ValueItem>(QueryArithmetic.Mod, item);
            for (int i = 0; i < 5000; i++)
            {
                var json = JsonSerializer.Serialize(pair, opt);
                var jsonNode = JsonSerializer.Deserialize(json, pair.GetType(), opt);

            }

            var block = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, new ValueItem(new int[] { 1, 2, 3 }));
            var pairblock = new NodeOperatorPair<QueryArithmetic, NodeBlock>(QueryArithmetic.Divide, block);

            var jsonNewton = Newtonsoft.Json.JsonConvert.SerializeObject(pair);
            var jsonNewtonPair = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonNewton, pair.GetType());


            //var jsonbk = JsonSerializer.Serialize(block);
            //var jsonbkk = JsonSerializer.Deserialize(jsonbk,block.GetType(),opt);

            var jsonb = JsonSerializer.Serialize(pairblock, opt);

            var jsonBlock = JsonSerializer.Deserialize(jsonb, pairblock.GetType(), opt);
        }

        [TestMethod]
        public void TestListSpeed2()
        {

            var count = 100;
            var loop = 1000000;

            var list = new List<string>(count);
            for (int i = 0; i < count; i++)
                list.Add(i.ToString());

            var arr2 = new string[count];
            for (int i = 0; i < count; i++)
                arr2[i] = i.ToString();

            string tmp;
            var sw = Stopwatch.StartNew();

            sw.Restart();
            for (int j = 0; j < loop; j++)
                foreach (var c in arr2)
                {
                    tmp = c;
                }
            var t2 = sw.ElapsedMilliseconds;

            sw.Restart();
            for (int j = 0; j < loop; j++)
                foreach (var c in list)
                {
                    tmp = c;
                }
            var t1 = sw.ElapsedMilliseconds;


            sw.Restart();
            for (int j = 0; j < loop; j++)
                for (int i = 0; i < count; i++)
                {
                    tmp = list[i];
                }
            var t3 = sw.ElapsedMilliseconds;

            sw.Restart();
            for (int j = 0; j < loop; j++)
                for (int i = 0; i < count; i++)
                {
                    tmp = arr2[i];
                }
            var t4 = sw.ElapsedMilliseconds;
            sw.Stop();

            unit.Log(t1);
            unit.Log(t2);
            unit.Log(t3);
            unit.Log(t4);

        }
        [TestMethod]
        public void TestListSpeed()
        {

            var countj = 10;
            var count = 1;

            var tbuildList = Stopwatch.StartNew();
            var arr = new List<string>(count);
            for (int i = 0; i < count; i++)
                arr.Add(i.ToString());
            tbuildList.Stop();

            var tbulidArray = Stopwatch.StartNew();
            var arr2 = new string[count];
            for (int i = 0; i < count; i++)
                arr2[i] = i.ToString();
            tbulidArray.Stop();

            var totalsw = Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();
            sw.Restart();
            for (var j = 0; j < countj; j++)
                if (arr is IList lst)
                    for (int i = 0; i < count; i++)
                    { var m = lst[i]; }
            var t1 = sw.ElapsedMilliseconds;

            sw.Restart();
            for (var j = 0; j < countj; j++)
                if (arr is ICollection c)
                    foreach (var m in c)
                    { var x = m; }
            var t2 = sw.ElapsedMilliseconds;


            sw.Restart();
            for (var j = 0; j < countj; j++)
                if (arr2 is IList ar)
                    for (int i = 0; i < count; i++)
                    { var m = ar[i]; }
            var t3 = sw.ElapsedMilliseconds;

            sw.Restart();
            for (var j = 0; j < countj; j++)
                if (arr2 is ICollection ar2)
                    foreach (var m in ar2)
                    { var x = m; }
            var t4 = sw.ElapsedMilliseconds;



            sw.Restart();
            for (var j = 0; j < countj; j++)
                for (int i = 0; i < count; i++)
                { var m = arr[i]; }
            var t5 = sw.ElapsedMilliseconds;

            sw.Restart();
            for (var j = 0; j < countj; j++)
                for (int i = 0; i < count; i++)
                { var m = arr2[i]; }
            var t6 = sw.ElapsedMilliseconds;
            sw.Stop();
            totalsw.Stop();

            var totalTime = totalsw.ElapsedMilliseconds;

            unit.Log(
                "List  no as            {0} ,\n\r" +
                "arry  no as            {1} ,\n\r" +
                "List: as IList:        {2} ,\n\r" +
                "List: as IColl:        {3} ,\n\r" +
                "Array as IList:        {4} ,\n\r" +
                "Array as IColl:        {5} ,\n\r\n\r" +
                "List Vs array with no as: t5-t6= {6},   {7}% \n\r" +
                "List Vs array with IList: t1-t3= {8},   {9}% \n\r" +
                "List Vs array with IColl: t2-t4= {10},  {11}% \n\r" +
                "List as IList Vs IColl:   t1-t2= {12},  {13}% \n\r" +
                "List as IColl Vs no as:   t2-t5= {14},  {15}% \n\r" +
                "Array as IList Vs IColl:  t3-t4= {16},  {17}% \n\r" +
                "Array as IColl Vs no as:  t4-t6= {18},  {19}% \n\r",
                t5, t6, t1, t2, t3, t4,
                t5 - t6, ((t5 - t6) / (float)t6 * 100f).ToString("F6"),
                t1 - t3, ((t1 - t3) / (float)t3 * 100f).ToString("F6"),
                t2 - t4, ((t2 - t4) / (float)t4 * 100f).ToString("F6"),
                t1 - t2, ((t1 - t2) / (float)t2 * 100f).ToString("F6"),
                t2 - t5, ((t2 - t5) / (float)t5 * 100f).ToString("F6"),
                t3 - t4, ((t3 - t4) / (float)t4 * 100f).ToString("F6"),
                t4 - t6, ((t4 - t6) / (float)t6 * 100f).ToString("F6")

               );

            var tblist = tbuildList.Elapsed.TotalMilliseconds;
            var tbArray = tbulidArray.Elapsed.TotalMilliseconds;
            unit.Log("\n\rTotal Times:  {0}:", (totalTime / 1000f).ToString("F3"));
            unit.Log("\n\rBilidlist:    {0}\n\r" +
                     "BuildArray:       {1}\n\r" +
                     "BuildList Vs BuildArray:  {2},    {3}%\n\r",
                     tblist,
                     tbArray,
                     (tblist - tbArray).ToString("F3"), ((tblist - tbArray) / (float)tbArray).ToString("F3")

                ); ;
        }

        [TestMethod]
        public void testEnum()
        {
            var lst =new List<int>{ 1,2};
            if(lst is IEnumerable kse)
            {
 
                if(kse is IEnumerable<int> ienum)
                {
                    var ints = ienum.ToArray();
                    var iarr = new int[ints.Length + 1];
                    iarr[0] = 10;
                    Array.Copy(ints, 0, iarr, 1, ints.Length);

                }
            }

            if (lst is IList<int> ilst)
            { var ii = ilst; }
            var ks = decimal.GetBits(10000001.22333m);
            ks = decimal.GetBits(0.22333m);
            ks = decimal.GetBits(0m);
            ks = decimal.GetBits(1m);
            ks = decimal.GetBits(int.MaxValue);
            ks = decimal.GetBits(int.MinValue);
            ks = decimal.GetBits(decimal.MaxValue);
            ks = decimal.GetBits(long.MaxValue);

            var dc=10000222.233312m.GetHashCode();
            unit.Log(dc);
            var dt = DateTime.MaxValue.GetHashCode();
            unit.Log(dt);
            unit.Log(false.GetHashCode());
            sbyte sbt = 33;
            unit.Log(sbt.GetHashCode());
            byte bt = 33;
            unit.Log(bt.GetHashCode());
            uint uit = 33;
            unit.Log(uit.GetHashCode());
            ulong ulg = 33;
            unit.Log(ulg.GetHashCode());

            int[] intarr = new int[] { 1, 2 };
            unit.Log(intarr.GetHashCode());
            unit.Log(QueryArithmetic.Divide.GetHashCode());

            unit.Log("asbc".GetHashCode());

            var f = float.MaxValue;

            var count = 10000;
            var sw = Stopwatch.StartNew();
            var key = typeof(List<string>).GetHashCode().ToString();
            for (int i = 0; i < count; i++)
            {                
                //var hs = ComputeHashCode(key);
                //unit.Log(hs);
            }
            sw.Stop();
            var t1 = sw.ElapsedMilliseconds;
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                var hs = typeof(List<string>).GetHashCode();
                //var hs = key.GetHashCode();
            }
            sw.Stop();
            var t2 = sw.ElapsedMilliseconds;

                unit.Log("t1:   {0},        t2:  {1}",t1,t2);

            unit.Log(typeof(List<string>).GetHashCode());
            unit.Log(typeof(List<DateTime>).GetHashCode());
        }

        internal protected int ComputeHashCode(string key)
        {
            if (key == null)
                return 0;

            var ks = key.ToCharArray();
            var length = ks.Length;
            unchecked
            {
                int hashCode = length;
                for (int i = 0; i < length; i++)
                {
                    hashCode += (hashCode << 7) ^ ks[i];
                }

                // mix it a bit more
                hashCode -= hashCode >> 17;
                hashCode -= hashCode >> 11;
                hashCode -= hashCode >> 5;

                return hashCode;
            }
        }

        [TestMethod]
        public void test()
        {
            
            //lst1的元素是乱序的数字,元素个数也是不固定的，可能0长度，也可能有重复的。
            var lst1 = new List<int>() { 12, 1, 2, 3, 4,  6, 8,  10};
            //lst2内含的元素是数组，这些数组都只内含有从小到大排序的两个数字(表示一个区间)，
            //lst2的元素与元素之间是无序的，个数也是不固定的,由其数组内两个数表示的区间可能有交叉，
            //也有可能某个区间包含了另一个区间，但不会完全刚好重叠（即不会完全相同的）。
            var lst2 = new List<int[]>() { new[] { 1, 2 }, new[] { 4, 5 }, new[] { 6, 11 },new[] {6,10 } };
            //1.要求：找出lst2里的数组，这个数组要满足其所界定的区间最少要包含lis1中的两个数.
            var f1 = lst2.Where(p => lst1.Count(x => x >= p[0] && x <= p[1]) >= 2).ToArray();
            //2.要求：获取lst1中的数，这些数要满足：不落在由f1中所表示的任何区间内，
            //  但必须能够落在lst2里表示的其它任意一个或多个区间内。
            //  PS:lst1的元素并不一定满足能够满足要求1的匹配，要求1的结果存在全部匹配失败的可能，而使得下面f2的长度为0，
            //  lst1的数字，有可能落在lst2里所表示的所有区间之外，也有可能刚好全部被匹配，也有可能只落在某个或者某几个区间之内。
            //3.要求：在要求1中所匹配到的两个或多个lst1里的数所能表示的区间（设为区间X）,找出能够与X 有更接近100%重叠
            //  的那个lst2元素,以及对应的lst1的两个元素。典型如下：lst1里的6，8，10落在lst2所表示的区间们[6,11]和[6,10]之上，
            //    X:   6______8_____10
            //         6________________11
            //        6____________10 
            // 例如上面三个数：6，8，10，可以被lst2里的[6,11]匹配， 也可以被[6,10]匹配。那么拿lst1的6和10来匹配lst2的[6,10]重叠可以达到100%，
            // 而拿{6，8}或{8，10}来匹配[6,10]或[6，11],或者拿{6，10}来匹配[6,11]，,重叠区域都要小之。
            // 因此：lst1的{6,10}，lst2的[6,10]是所需的结果。
            var f2 = lst1.Where(s => (!f1.Any(f => s >= f[0] & s <= f[1]))
                                && lst2.Any(y => s >= y[0] && s <= y[1])).ToArray();
            

            //满足要求1的结果:{1,2,},{6,9}
            Assert.IsTrue(f1.Length == 3 && f1[0][0]==1 && f1[0][1]==2 && f1[1][0]==6 && f1[1][1]==11 && f1[2][0]==6 && f1[2][1]==10);
            //在f1之外，在lst2之中的结果： 4
            Assert.IsTrue(f2.Length == 1 && f2[0] == 4) ;
            //要求3？

            var a = 10;
            var b = 1.1;

            //   var varleft = Expression.Variable(typeof(int),"a");
            //   var assignLeftExpr = Expression.Assign(varleft, Expression.Constant(a, a.GetType()));
            //   var leftExpr = Expression.Block(new ParameterExpression[] { varleft }, assignLeftExpr);

            //     var varRight = Expression.Variable(b.GetType(),"b");
            //   var assignRightExpr = Expression.Assign(varRight, Expression.Constant(b, b.GetType()));
            //   var RightExpr = Expression.Block(new ParameterExpression[] { varRight }, assignRightExpr);
            //   var resultExpr = Expression.GreaterThan(leftExpr, RightExpr);

            //   var lambda = Expression.Lambda<Func<bool>>(resultExpr);
            //var r=   lambda.Compile()();


            //var lst = new list<int>() { 1, 2 };
            //expression<func<int, bool>> lamb=(p) => p > 1.2;
            //lambdaexpression.lambda(lamb).Compile();

            var count = 1000000;
            var type = typeof(decimal);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                UnwrapNullableType(type);
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Decimal:
                        break;
                }
            }
            sw.Stop();
            var t1 = sw.ElapsedMilliseconds;
            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                type = UnwrapNullableType(type);
                var bxx = type == typeof(int) || type == typeof(long) || type == typeof(short)
                || type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort)
                || type == typeof(sbyte) || type == typeof(char);

                type = UnwrapNullableType(type);
                var xxc = type == typeof(double) || type == typeof(float) || type == typeof(decimal);
            }
            sw.Stop();
            var t2 = sw.ElapsedMilliseconds;
            UnitsComm.unit.Log("次数：{0},   时间1：{1}  ,时间2： {2}", count, t1, t2);
        }
        public static Type UnwrapNullableType(Type type) => Nullable.GetUnderlyingType(type) ?? type;

    }
    enum ttxx
    {
        a,
        b,
        c,
    }



}





class entTest
{
    public int MyIntProperty { get; set; }
    public string MyStringProperty { get; set; }
}


public class ExpressionConverter<TToB>
{
    public static Expression<Func<TToB, TR>> Convert<TFrom, TR>(Expression<Func<TFrom, TR>> expr)
    {
        Dictionary<Expression, Expression> substitutues = new Dictionary<Expression, Expression>();
        var oldParam = expr.Parameters[0];
        var newParam = Expression.Parameter(typeof(TToB), oldParam.Name);
        substitutues.Add(oldParam, newParam);
        Expression body = ConvertNode(expr.Body, substitutues);
        return Expression.Lambda<Func<TToB, TR>>(body, newParam);
    }

    static Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subst)
    {
        if (node == null) return null; if (subst.ContainsKey(node)) return subst[node]; switch (node.NodeType)
        {
            case ExpressionType.Equal:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.LessThan:
            case ExpressionType.NotEqual:
            case ExpressionType.GreaterThan:
            //case ExpressionType.:   
            case ExpressionType.Or:
                {
                    var be = (BinaryExpression)node;
                    return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subst), ConvertNode(be.Right, subst), be.IsLiftedToNull, be.Method);
                }
            case ExpressionType.Call:
                {
                    var be = (MethodCallExpression)node;

                    var expression = GetMethodExpression((MemberExpression)be.Object, ((ConstantExpression)be.Arguments[0]).Value.ToString(), be.Method.Name, subst);
                    return expression.Body;
                }

            default: throw new NotSupportedException(node.NodeType.ToString());
        }
    }



    static Expression<Func<TToB, bool>> GetMethodExpression(
        MemberExpression propertyExp,
        string propertyValue,
        string MethodName,
        IDictionary<Expression, Expression> subst)
    {
        //var parameterExp = Expression.Parameter(typeof(T), ((ParameterExpression)propertyExp.Expression).Name);
        var newParameterExp = (ParameterExpression)ConvertNode((ParameterExpression)propertyExp.Expression, subst);


        var newPropertyExp = ConvertNode(propertyExp, subst);
        MethodInfo method = typeof(string).GetMethod(MethodName, new[] { typeof(string) });
        var someValue = Expression.Constant(propertyValue, typeof(string));
        var containsMethodExp = Expression.Call(newPropertyExp, method, someValue);

        return Expression.Lambda<Func<TToB, bool>>(containsMethodExp, newParameterExp);
    }


    public static Expression<Func<TEntity, bool>> ContainsPredicate<TEntity, T>(T[] arr, string fieldname) where TEntity : class
    {
        ParameterExpression entity = Expression.Parameter(typeof(TEntity), "entity");
        MemberExpression member = Expression.Property(entity, fieldname);

        var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(m => m.Name == "Contains");
        MethodInfo method = null;
        foreach (var m in containsMethods)
        {
            if (m.GetParameters().Count() == 2)
            {
                method = m;
                break;
            }
        }
        method = method.MakeGenericMethod(member.Type);
        var exprContains = Expression.Call(method, new Expression[] { Expression.Constant(arr), member });
        return Expression.Lambda<Func<TEntity, bool>>(exprContains, entity);
    }



}