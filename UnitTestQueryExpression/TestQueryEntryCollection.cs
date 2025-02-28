using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sag.Data.Common;
using Sag.Data.Common.Query;
using UnitsComm;

namespace UnitTestQueryExpression
{
    //[TestClass]
    //public class TestQueryEntryCollection
    //{
    //    [TestMethod]
    //    public void AddNewTest()
    //    { 
    //        QueryPropertyItem q;
    //        var count =100;
    //        var rept = 10;
    //        var c = new QueryPartsCollection<QueryArithmetic,QueryPropertyItem>();
    //        for (var i = 0; i < count; i++)
    //        {                
    //            for (var j = 0; j < rept; j++)
    //            {   
    //                var index= i;
    //                q = new QueryPropertyItem(index);
    //               c.Add(QueryArithmetic.Add, q, InsertionBehavior.Duplicates);

    //            }   
    //        }

    //        Assert.IsTrue(c.Count == count * rept);
    //        c = new QueryPartsCollection<QueryArithmetic, QueryPropertyItem>();
    //        for (var i = 0; i < count; i++)
    //        {
    //            for (var j = 0; j < rept; j++)
    //            {
    //                var index = i;
    //                q = new QueryPropertyItem(index);
    //                c.Add(QueryArithmetic.Add, q, InsertionBehavior.Overwrite);

    //            }
    //        }

    //        Assert.IsTrue(c.Count == count);
    //        c = new QueryPartsCollection<QueryArithmetic, QueryPropertyItem>();
    //        for (var i = 0; i < count; i++)
    //        {
    //            for (var j = 0; j < rept; j++)
    //            {
    //                var index = i;
    //                q = new QueryPropertyItem(index);
    //                c.Add(QueryArithmetic.Add, q, InsertionBehavior.IgnoreExists);

    //            }
    //        }

    //        Assert.IsTrue(c.Count == count);

    //        c.Clear();
    //        c.Add(QueryArithmetic.Add, new QueryPropertyItem(2));
    //        Assert.IsTrue(c.Count == 1);


    //    }

    //    [TestMethod]
    //    public void FindEntryTest()
    //    {
    //        QueryPropertyItem q;
    //        var count = 10;
    //        var rept = 3;
    //        var c = new QueryPartsCollection<QueryArithmetic, QueryPropertyItem>();
    //        for (var i = 0; i < count; i++)
    //        {
    //            for (var j = 0; j < rept; j++)
    //            {
    //                q = new QueryPropertyItem(i);
    //                c.Add(QueryArithmetic.Add, q, InsertionBehavior.Duplicates);
    //            }
    //        }
    //        var fr = c.IndexOf(QueryArithmetic.Add, new QueryPropertyItem(1),false);
    //        Assert.IsTrue(fr.Length == rept);
    //        for (var i = 0; i < rept; i++)
    //        {
    //             Assert.IsTrue(fr[i] == i+1*rept);
             
    //        }

    //        var fr1 = c.IndexOf(QueryArithmetic.Add, c[2].Expression, true);
    //        Assert.IsTrue(fr1[0] == 2 && fr1.Length==1);
                        

    //    }
 
    //    [TestMethod]
    //    public void TestRemove()
    //    {
    //        QueryPropertyItem q;
    //        var count = 6;
    //        var rept = 2;
    //        var qlist = new List<QueryPropertyItem>();
    //        var c = new QueryPartsCollection<QueryArithmetic, QueryPropertyItem>();
    //        for (var i = 0; i < count; i++)
    //        {
    //            for (var j = 0; j < rept; j++)
    //            {
    //                q = new QueryPropertyItem(i);
    //                c.Add(QueryArithmetic.Add, q, InsertionBehavior.Duplicates);
    //                qlist.Add(q);
    //            }
    //        }
    //        //传入引用对象,非引用删除一个key
    //        var fr = c.Remove(QueryArithmetic.Add,qlist[1],false);
    //        Assert.IsTrue(fr >0 && c.Count==(count-1)*rept);
    //        Assert.IsTrue(c.IndexOf(QueryArithmetic.Add, qlist[1]).Length == 0);

            
    //        //传入引用对象,引用删除
    //        var fr_0 = c.Remove(QueryArithmetic.Add, qlist[1], true);
    //        Assert.IsTrue(fr_0 == 0 && c.Count == (count - 1) * rept);
    //        Assert.IsTrue(c.IndexOf(QueryArithmetic.Add, qlist[1]).Length == 0);

    //        count--;
    //        //传入非引用对象,引用删除末尾key的第一个
    //        var fr1 = c.Remove(QueryArithmetic.Add, new QueryPropertyItem(count),true);
    //        Assert.IsTrue(fr1==0);
    //        Assert.IsTrue(c.Count == count*rept);

            
    //        //传入非引用对象,非引用删除末尾key
    //        var fr2 = c.Remove(QueryArithmetic.Add, new QueryPropertyItem(count),false);
    //        Assert.IsTrue(fr2==rept);
    //        Assert.IsTrue(c.Count == (count-1)*rept);
    //        //前面已删除完一整个key,再次查找它
    //        Assert.IsTrue(c.IndexOf(QueryArithmetic.Add, new QueryPropertyItem(count)).Length == 0);

    //        count--;
    //        //再次传入引用对象,引用删除,倒数第2个key的第2个
    //        var fr3 = c.Remove(QueryArithmetic.Add, qlist[(count-1)*rept+1], true);
    //        Assert.IsTrue(fr3==1);
    //        Assert.IsTrue(c.Count == count*rept-1);
    //        //采用非引用方式查找它
    //        q = qlist[(count - 1) * rept + 1];
    //        Assert.IsTrue(c.IndexOf(QueryArithmetic.Add, new QueryPropertyItem(q.Name),false).Length == rept-1);

    //        //查找一个不存在的
    //        Assert.IsTrue(c.IndexOf(QueryArithmetic.Add, new QueryPropertyItem("xcccc")).Length == 0);


    //        var f0 = c.IndexOf(QueryArithmetic.Add, qlist[0]);
    //        var f3 = c.IndexOf(QueryArithmetic.Add, qlist[3]);
    //        Assert.IsTrue(f0.Length==0 && f3.Length==rept );

    //    }


    //    /*
    //    [TestMethod]
    //    public void TestMemberList()
    //    {
    //        var itemCount = 20;
    //        var strlst = new List<String>(itemCount);
    //        for (var i = 0; i < itemCount; i++)
    //        {
    //            strlst.Add(i.ToString());

    //        }
    //        var c = strlst;
    //        var memc = new MemberList<string>(c);
    //        Assert.IsTrue(memc.Count == itemCount);
    //        for(var i=0;i<itemCount;i++)
    //        {
    //           Assert.IsTrue( memc[i] == strlst[i]);
    //        }

    //        var mem_a = new MemberList<int>();
    //        for(var i=0;i<itemCount;i++)
    //        {
    //            mem_a.Add(i+1);
    //        }

    //        var arr = mem_a.ToArray(2, 10);
    //        for (var i = 0; i < 10; i++)
    //            Assert.IsTrue(arr[i] == i + 3);

    //        mem_a.RemoveAt(5);
    //        foreach (var m in mem_a)
    //            Assert.IsTrue(m != 6);

    //        mem_a.Clear();
    //        Assert.IsTrue(mem_a.Count == 0);

    //        mem_a.Add(22);
    //        Assert.IsTrue(mem_a.Count == 1 && mem_a[0] == 22);
    //        mem_a.Add(33);
    //        Assert.IsTrue(mem_a.Count == 2 && mem_a[1] == 33);
    //        mem_a.RemoveAt(0);
    //        Assert.IsTrue(mem_a.Count == 1 && mem_a[0] == 33);

    //        itemCount = 100000;
    //        for (var i = 0; i < itemCount; i++)
    //            mem_a.Add(i);

    //        mem_a.RemoveAt(5000);
    //        Assert.IsTrue(mem_a.Count == itemCount);
    //        Assert.IsTrue(mem_a[5000] == 5000);
    //    }

    //    */
    //}


    
}
