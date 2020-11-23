using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sag.Data.Common.Query;
using Sag.Data.Common.Query.Tests;

namespace QueryFilterTests
{
    [TestClass]
   public class DinstinctSetTests
    {
        private long GetMemorySize()
        {
            GC.Collect(0);
            return  GC.GetTotalMemory(true);
        }

        [TestMethod]
        public void ktest()
        {
            var k=1;
            var p = k;
            for (var count = 1; count < 10000000; count = k)
            {
                 k = (int)(count*0.9m * (2m + 100m / (count + 100m)));
                Units.Log("count: {0} newCount: {1} , p：{2} ， count/newCount:{3}", count, k,(p*=2), (decimal)k / count);
            }
        }


        [TestMethod]
        public void SortSetTest()
        {
 

            var count = 1000000;
            
            long size1 = 0;
            long size2 = 0;
            size1 = GetMemorySize();
            var set = new HashSet<int>();
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                set.Add(i);
            }

            sw.Stop();
//   set.TrimExcess();
            size2 = GetMemorySize();
            Units.Log($"Sys.HashSet      add(ms): {sw.ElapsedMilliseconds}       size(kb):{(size2-size1)/1024}");
            sw.Restart();
            foreach (var m in set)
            {
                //_ = m;
            }
            sw.Stop();
            Units.Log($"Sys.HashSet foreach(ms): {sw.ElapsedMilliseconds}");

            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                set.Remove(i);
            }
            sw.Stop();
            Units.Log($"Sys.HashSet remove(ms): {sw.ElapsedMilliseconds}");
            //==================================================
            size1 = GetMemorySize();
            var set2 = new DistinctSet<int>(2);
            sw.Restart();
            for (int i = 0; i != count; ++i)
            {
                set2.Add(i);
            }

            sw.Stop();
//set2.TrimExcess();            
            size2 = GetMemorySize();
            Units.Log($"myset                add(ms): {sw.ElapsedMilliseconds}         size(kb):{(size2 - size1) / 1024}");

            sw.Restart();
            set2.Enumerate(m => { });
            sw.Stop();

            Units.Log($"myset            foreach(ms): {sw.ElapsedMilliseconds}");

            sw.Restart();
            for (int i = 0; i < count; ++i)
            {
                set2.Remove(i);
            }
            sw.Stop();
            Units.Log($"myset          remove(ms): {sw.ElapsedMilliseconds}");


        }

        [TestMethod]
        public void AddTest()
        {
            var cont = 3;
            var set = new DistinctSet<int>(2);
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
            var n = 0;
            foreach (var m in set.ToArray())
            {
                Units.Log(m);
                Assert.IsTrue(m == n++);
            }
            n = 0;
            foreach (var m in set.ToArray())
            {
                Assert.IsTrue(set.Contains(n++));
            }
           
        }


        [TestMethod]
        public void IndexTest()
        {
            var cont =10;
            var set = new DistinctSet<int>(2);
            for(int i=0;i!=cont;++i)  //0-9
            {
                set.Add(i);
            }
 
            var n = 0;
            for (var i=0;i<cont;i++)
            {
                Units.Log(set[i]) ;
                Assert.IsTrue(set[i] == n++);
            }
            Units.Log("-------------0-9");

            for(int i=0;i<5;i++)  //5-9
            {
                set.Remove(i);
            }                                  
            for(int i=10;i!=15;++i) //10-14
            {
                set.Add(i);
            }          
            
            n = 5;
            for (var i = 0; i < 10; i++)
            {
                Units.Log(set[i]);
                Assert.IsTrue(set[i] == n++);
            }
            Units.Log("-------------5-14");



        }



        [TestMethod]
        public void AddRemoveTest()
        {

            var cont = 30000;
            var set = new DistinctSet<int>(2);
            var sw = Stopwatch.StartNew();
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
           
            //===========================================
            var n = 0;
            foreach (var m in set.ToArray())
            {
                var rm = set.Remove(n++);
              //  Assert.IsTrue(rm);
            }
            //reAdd
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
            n = 0;
            foreach (var m in set.ToArray())
            {
               // Units.Log(m);
               // Assert.IsTrue(m == n++);
            }
            n = 0;
            foreach (var m in set.ToArray())
            {
                // Units.Log("{0} == {1}", m, n++);
                var cm =set.Contains(m);
              //  Assert.IsTrue(cm);
            }
            var tt = sw.ElapsedMilliseconds;
            Units.Log("{0}, {1}",tt,set.Count);
            
        }


        [TestMethod]
        public void AddRemoveAtTest()
        {

            var cont = 30;
            var set = new DistinctSet<int>(2);
            var sw = Stopwatch.StartNew();
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
           
            //===========================================
            var n = 19;
            var removed = new HashSet<int>();
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }


            n = 9;
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            n = 0;
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+9-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            foreach (var m in set.ToArray())
            {
                 //Units.Log(m);
                 Assert.IsTrue(!removed.Contains(m));
            }
            //foreach (var m in removed.ToArray())
            //{
            //    // Units.Log(m);
            //    Assert.IsTrue(!set.Contains(m));
            //}

            var tt = sw.ElapsedMilliseconds;
            Units.Log("{0}, {1}",tt,set.Count);
            
        }
   [TestMethod]
        public void AddRemoveAtFromMidTest()
        {

            var cont = 30;
            var set = new DistinctSet<int>(2);
            var sw = Stopwatch.StartNew();
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
           
            //===========================================
            var n = 10;
            var removed = new HashSet<int>();
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }


            n = 9;
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            n = 0;
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+9-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            foreach (var m in set.ToArray())
            {
                 Units.Log(m);
                 Assert.IsTrue(!removed.Contains(m));
            }
            //foreach (var m in removed.ToArray())
            //{
            //    // Units.Log(m);
            //    Assert.IsTrue(!set.Contains(m));
            //}

            var tt = sw.ElapsedMilliseconds;
            Units.Log("{0}, {1}",tt,set.Count);
            
        }

   [TestMethod]
        public void AddRemoveAtFromStartTest()
        {

            var cont = 30;
            var set = new DistinctSet<int>(2);
            var sw = Stopwatch.StartNew();
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
           
            //===========================================
            var n = 0;
            var removed = new HashSet<int>();
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }


            n = 9;
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            n = 0;
            for  (var i =0;i<10;i++)
            {
                var rm = set.RemoveAt(n+9-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            foreach (var m in set.ToArray())
            {
                 Units.Log(m);
                 Assert.IsTrue(!removed.Contains(m));
            }
            //foreach (var m in removed.ToArray())
            //{
            //    // Units.Log(m);
            //    Assert.IsTrue(!set.Contains(m));
            //}

            var tt = sw.ElapsedMilliseconds;
            Units.Log("{0}, {1}",tt,set.Count);
            
        }
 [TestMethod]
 public void AddRemoveAtFromRomdomTest()
        {

            var cont = 30;
            var set = new DistinctSet<int>(2);
            var sw = Stopwatch.StartNew();
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
           
            //===========================================
            var n = 0;
            var removed = new HashSet<int>();
            for  (var i =0;i<2;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }
              n = 15;
              removed = new HashSet<int>();
            for  (var i =0;i<2;i++)
            {
                var rm = set.RemoveAt(n+10-i);
                removed.Add(rm);
                Units.Log(rm);
            }


            n = 15;
            for  (var i =0;i<2;i++)
            {
                var rm = set.RemoveAt(n+5-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            n = 6;
            for  (var i =0;i<2;i++)
            {
                var rm = set.RemoveAt(n+9-i);
                removed.Add(rm);
                Units.Log(rm);
            }
            n = 8;
            for  (var i =0;i<2;i++)
            {
                var rm = set.RemoveAt(n+9-i);
                removed.Add(rm);
                Units.Log(rm);
            }

            Units.Log("-----------------------------");
            foreach (var m in set.ToArray())
            {
                 Units.Log(m);
                 Assert.IsTrue(!removed.Contains(m));
            }
            //foreach (var m in removed.ToArray())
            //{
            //    // Units.Log(m);
            //    Assert.IsTrue(!set.Contains(m));
            //}

            var tt = sw.ElapsedMilliseconds;
            Units.Log("{0}, {1}",tt,set.Count);
            
        }


 [TestMethod]
        public void AddRemoveListTest()
        {

            var cont = 30000;
            var set = new List<int>(2);
            var sw = Stopwatch.StartNew();
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
           
            //===========================================
            var n = 0;
            foreach (var m in set.ToArray())
            {
                var rm = set.Remove(n++);
              //  Assert.IsTrue(rm);
            }
            //reAdd
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
            n = 0;
            foreach (var m in set.ToArray())
            {
               // Units.Log(m);
               // Assert.IsTrue(m == n++);
            }
            n = 0;
            foreach (var m in set.ToArray())
            {
                // Units.Log("{0} == {1}", m, n++);
             //   var cm =set.Contains(m);
              //  Assert.IsTrue(cm);
            }
            var tt = sw.ElapsedMilliseconds;
            Units.Log(tt);
        }



 [TestMethod]
        public void AddRemoveHashSetTest()
        {

            var cont = 30000;
            var set = new HashSet<int>(2);
            var sw = Stopwatch.StartNew();
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
           
            //===========================================
            var n = 0;
            
            //foreach (var m in set.ToArray())
            //{
            //    var rm = set.Remove(n++);
            //  //  Assert.IsTrue(rm);
            //}
            //reAdd
            for(int i=0;i!=cont;++i)
            {
                set.Add(i);
            }
            n = 0;
            //foreach (var m in set.ToArray())
            //{
            //   // Units.Log(m);
            //   // Assert.IsTrue(m == n++);
            //}
            //n = 0;
            //foreach (var m in set.ToArray())
            //{
            //    // Units.Log("{0} == {1}", m, n++);
            //    var cm =set.Contains(m);
            //  //  Assert.IsTrue(cm);
            //}
            var tt = sw.ElapsedMilliseconds;
            Units.Log(tt);
        }

    }
}
