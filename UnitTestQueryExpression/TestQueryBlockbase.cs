using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Sag.Data.Common.Query;
using Sag.Data.Common;
using UnitsComm;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace UnitTestQueryExpression
{

    [TestClass]
    public class TestQueryBlockbase
    {
        Type itemtype = typeof(string);
        List<ValueItem> createitem(int icount, Type type, int plus = 0)
        {
            var list = new List<ValueItem>();
            if (type == null) type = itemtype;
            for (var i = 0; i < icount; i++)
            {
                var m = new ValueItem();
                m.Value = "itme_" + (i+plus).ToString();
             
              //  unit.Log("created item:  {0}   hascode:   {1}", m.Value, m.GetHashCode().ToString());
                list.Add(m);
            }
            return list;
        }

        class eq : QueryEqualComparer<NodeBlock>
        {
 
        }

        NodeBlock createblock(int icount, int bcount, Type typeitem, Type typeblock,InsertionBehavior itemUnique,InsertionBehavior blockUnique)
        {
            var block = new NodeBlock();
            var subblocklist = new List<NodeBlock>();

            var blockItemUnique = itemUnique;
            var blockSubblockUnique = blockUnique;

            var subItemUnique = itemUnique;
            var subBblockUnique =blockUnique;

            block.SetEqualComparer ( new eq());
               int xx;
            for (int b = 0; b < bcount; b++)
            {
                var subitms = new List<ValueItem>();
                
                    for (int i = 0; i < icount; i++)
                    {
                        var subItemValue = "item_in_subblock_" + i.ToString();
                        //unit.Log("create sub items for subblock:  {0} ",subItemValue);
                        subitms.Add(new ValueItem() { Value = subItemValue });
                    }

                var subblock = new NodeBlock();
 
                xx = (subblock.AddItem(QueryArithmetic.Add, subItemUnique, subitms.ToArray()));
                   //if(!xx) unit.Log("sublock {0} add item first_array time    false   !",b.ToString());



                for (int i=0;i<subitms.Count;i++)
                {
                    var m = subitms[i];
                    xx = (subblock.AddItem(QueryArithmetic.Add, subItemUnique, m));
                    //if(!xx) unit.Log("block {0}  add item  {1}   again   false,  Value:  {2}" ,b.ToString(), m.GetHashCode().ToString(),m.Value);

                }

                subblocklist.Add(subblock);
                
            }

            xx= block.AddBlock(QueryArithmetic.Add, subItemUnique, subblocklist.ToArray());
            //if (xx) unit.Log("parent block  add sublock_array  fiale  block count  {0} ", block.BlockCount);


            var its = createitem(icount, typeblock, 0);
                for(int i=0;i<its.Count;i++)
                {
                xx = (block.AddItem(QueryArithmetic.Add, blockItemUnique, its[i]));
//                      if(!xx)  unit.Log("subblock {0} :  add item_array    false:   hashCode: {1}   Value:  {2}",1.ToString(), its[i].GetHashCode().ToString(),its[i].Value);
                        
                }
            for (int i=0;i<subblocklist.Count;i++)
            {
                xx = block.AddBlock(QueryArithmetic.Add, blockSubblockUnique, subblocklist[i]);
                //if(!xx)
                //unit.Log("block add subblock {0}    again false:  block Allcount:  {1}",
                //        subblocklist[i].GetHashCode().ToString(),block.AllCount);

            }

            if (blockItemUnique== InsertionBehavior.Overwrite)
                unit.Log("if true : blockCount=  {0}  ", block.BlockCount == 1);
            else
                unit.Log("if true : blockCount=  {0}  ", block.BlockCount == bcount*2);

            if (itemUnique==InsertionBehavior.Overwrite)
                unit.Log("if true : itemCount=  {0}  \n\r", block.ItemCount == icount);
            else
                unit.Log("if true : itemCount=  {0}  \n\r", block.ItemCount == icount);

            
            

            return block;
        }

        #region test itembase

        [TestMethod]
        public void TestQueryItemEqual()
        {
            //true test all same
            var item1 = new ValueItem() { Value = "item1"};
            var item2 = new ValueItem() { Value = "item1"};

            var b1 = item1.Equals((object)item2);

            unit.Log("true test all same_with_object:   " + b1.ToString());
            var b2 = item1.Equals(item2);
            unit.Log("true test all same_with_type:   " + b2.ToString());

            //true test Value1!=Value2,type1=type2 
            var item3 = new ValueItem() { Value = "item3"};
            var item4 = new ValueItem() { Value = "item3" };

            b1 = item3.Equals((object)item4);
            unit.Log("true test smae_Value_with_object:   " + b1.ToString());
            b2 = item3.Equals(item4);
            unit.Log("true test same_Value_with_type:   " + b2.ToString());


            //false test Value1!=Value2,type1=type2 
            var item5 = new ValueItem() { Value = "item5"};
            var item6 = new ValueItem() { Value = "item6"};

            b1 = item5.Equals((object)item6);
            unit.Log("false test smae_type_with_object: {0} result: {1}  ", "item5", b1.ToString());
            b2 = item5.Equals(item6);
            unit.Log("false test same_type_with_type:  {0} result: {1}  ", "item6", b2.ToString());


            //false test Value1!=Value2,type1!=type2 
            var item7 = new ValueItem() { Value = "item7"};
            var item8 = new ValueItem() { Value = "item8"  };


            b1 = item5.Equals((object)item6);
            unit.Log("false test all not smae_with_object: {0} result: {1}  ", "item7", b1.ToString());
            b2 = item5.Equals(item6);
            unit.Log("false test all not smae_with_type:  {0} result: {1}  ", "item8", b2.ToString());


        }


        [TestMethod]
        public void TestQueryItemEqualOperater()
        {
            //true test all same,default type
            var item1 = new ValueItem() { Value = "item1"};
            var item2 = new ValueItem() { Value = "item1"};
            Assert.IsTrue(item1==item2);
            Assert.IsFalse(item1!=item2);

            //unqual Flag,
            var item3 = new ValueItem() { Value = "item3"};
            var item4 = new ValueItem() { Value = "item3"};
            Assert.IsFalse(item3==item4);
            Assert.IsTrue(item3!=item4);

            //unequal value
            var item5 = new ValueItem() { Value = "item5"};
            var item6 = new ValueItem() { Value = "item6"};
            Assert.IsFalse(item5==item6);
            Assert.IsTrue(item5!=item6);

            //unqual typeas
            var item7 = new ValueItem() { Value = "item7"};
            var item8 = new ValueItem() { Value = "item7"};
            Assert.IsTrue(item7==item8);
            Assert.IsFalse(item7 != item8);

        }

        [TestMethod]
        public void TestItemGetHashCode()
        {
            var item1 = new ValueItem() { Value = "item1" };
            var item2 = new ValueItem() { Value = new List<DateTime>() { DateTime.Now} };
            var item3 = new ValueItem() { Value = new string[] { "a","b"} };
            var item4 = new ValueItem() { Value = new []{1,2 } };

            item1.GetHashCode();
            item1.GetHashCode();
            item2.GetHashCode();
            item3.GetHashCode();
            item4.GetHashCode();

        }

        [TestMethod]
        public void TestQueryItemEqualOperaterWithObjectType()
        {
            //true test all same
            var item1 = new ValueItem() { Value = "item1"};
            var item2 = new ValueItem() { Value = "item1"};

            //以下将不会使用重载的运行算,因为左值不是QueryPropertyItem类型
            var b1 = (object)item1!=(object)item2;
            unit.Log("false test all same_with_object:   " + b1.ToString());
            var b2 = (object)item1==(object)item2;
            unit.Log("true test all same_with_type:   " + b2.ToString());

            //以下将不会使用重载的运行算,因为左值不是QueryPropertyItem类型
            b1 = (object)item1!=item2;
            unit.Log("false test all same_with_object:   " + b1.ToString());
             b2 = (object)item1==item2;
            unit.Log("true test all same_with_type:   " + b2.ToString());


            b1 = item1 != (object)item2;
            unit.Log("false test all same_with_object:   " + b1.ToString());
            b2 = item1 == (object)item2;
            unit.Log("true test all same_with_type:   " + b2.ToString());

            

        }

        #endregion testitembase


        #region test blockbase

        [TestMethod]
        public void TestBlockAdd()
        {
            var blockCount1 = 10000;
            var blockCount2 = 3;
            var blockCount3 = 2;

            var subBlockCount1 = 5;
            var subBlockCount2 = 2;
            var subBlockCount3 = 5;

            var itemCount = 5;

            var sumItem = 0;
            var sumBlock = 0;
            

            var blocklist1 = new List<NodeBlock>();
            var blocklist2 = new List<NodeBlock>();
            var blocklist3 = new List<NodeBlock>();

            var isUniqueItem = InsertionBehavior.Duplicates;
            var isUniqueBlock = InsertionBehavior.Overwrite;

            for (int i=0;i<blockCount1;i++)
            {
                var qx = new NodeBlock();
                qx.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount, typeof(string), 10+i).ToArray());
                for(int j=0;j<subBlockCount1;j++)
                {
                    var qj = new NodeBlock();
                    qj.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount+3, typeof(string), 1).ToArray());
                    var qjsub1 = new NodeBlock();
                    qjsub1.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount+2, typeof(DateTime), 2).ToArray());
                    var qjsub2 = new NodeBlock();
                    qjsub2.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount, typeof(DateTime), 2).ToArray());
                    qj.Add(QueryArithmetic.Add, isUniqueBlock, qjsub1, qjsub2);//qj 包含 qsub
                    qx.Add(QueryArithmetic.Add, isUniqueBlock, qj);            //qx 包含qj

                    sumItem += itemCount * 4;
                    sumBlock += 3;
                }
                sumBlock += 1;

                blocklist1.Add(qx);
               // unit.Log("blocklist1. HashCode [{0}]:  ", qx.GetHashCode());

            }

            for (int i = 0; i < blockCount2; i++)
            {
                var qx = new NodeBlock();
                qx.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount, typeof(string), 0).ToArray());
                for (int j = 0; j < subBlockCount1; j++)
                {
                    var qj = new NodeBlock();
                    qj.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount+3, typeof(string), 1).ToArray());
                    var qjsub1 = new NodeBlock();
                    qjsub1.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount+2, typeof(DateTime), 2).ToArray());
                    var qjsub2 = new NodeBlock();
                    qjsub2.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount, typeof(DateTime), 2).ToArray());
                    qj.Add(QueryArithmetic.Add, isUniqueBlock, qjsub1, qjsub2);//qj 包含 qsub
                    qx.Add(QueryArithmetic.Add, isUniqueBlock, qj); //qx 包含qj
                    sumItem += itemCount * 4;
                    sumBlock += 3;
                }
                sumBlock += 1;
                blocklist2.Add(qx);
              //  unit.Log("blocklist2. HashCode [{0}]:  ", qx.GetHashCode());
            }


            for (int i = 0; i < blockCount3; i++)
            {
                var qx = new NodeBlock();
                qx.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount, typeof(string), 10).ToArray());
                for (int j = 0; j < subBlockCount3; j++)
                {
                    var qj = new NodeBlock();
                    qj.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount+3, typeof(string), 11).ToArray());
                    var qjsub1 = new NodeBlock();
                    qjsub1.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount+2, typeof(DateTime), 22+i).ToArray());
                    var qjsub2 = new NodeBlock();
                    qjsub2.Add(QueryArithmetic.Add, isUniqueItem, createitem(itemCount+2, typeof(DateTime), 33).ToArray());
                    qj.Add(QueryArithmetic.Add, isUniqueBlock, qjsub1, qjsub2);//qj 包含 qsub
                    qx.Add(QueryArithmetic.Add, isUniqueBlock, qj); //qx 包含qj
                    sumItem += itemCount * 4;
                    sumBlock += 3;
                }

                sumBlock += 1;
                blocklist3.Add(qx);
               // unit.Log("blocklist3. HashCode [{0}]:  ", qx.GetHashCode());
            }

            var ql = new List<NodeBlock>();
            ql.AddRange(blocklist1);
            ql.AddRange(blocklist2);
            ql.AddRange(blocklist3);
            sumBlock += 1;

            var allbllockCount = blockCount1 + blockCount2 + blockCount3;
            var uniquebllockCount = blockCount1 + blockCount3 + 1;


            var q = new NodeBlock();
            isUniqueBlock = InsertionBehavior.Overwrite;
            isUniqueItem = InsertionBehavior.Duplicates;
            q.Add(QueryArithmetic.Add, isUniqueItem, createitem(4, typeof(string), 0).ToArray());
            q.Add(QueryArithmetic.Add, isUniqueItem, createitem(4, typeof(string), 0).ToArray());
            q.Add(QueryArithmetic.Add, isUniqueBlock, ql.ToArray());
            sumBlock += 1;

            var uniqualItemCount = 4;
            var allitemCount = 8;
            //unit.Log("\n\rblock allow Uniqueitem   :  {0}   , Uniqueblock :  {1}  \n\r",
            //    q.IsUniqueItems.ToString(), q.IsUniqueBlocks.ToString());
            //unit.Log("items1_count:  {0}   ,  items2_count:  {1}  ,block_itemCount:   {2} ,block_subblockCount {3} "
            //    , items1.Count.ToString(), items2.Count.ToString(), q.ItemCount.ToString(),q.BlockCount);


            unit.Log("\r");
            for (int i = 0; i < q.ItemCount; i++)
            {
               // unit.Log("q.Items [{0}]  :  {1}   operater: {2}  ", q.GetItemPair(i).Expression.Value, q.GetItemPair(i).GetHashCode(), q.GetItemPair(i).Operater);
            }


            if (isUniqueItem== InsertionBehavior.Overwrite)
                unit.Log("result: isUniqueItems  {0},   items: {1}    isTrue:  {2}  \r",
                    isUniqueItem, q.ItemCount, q.ItemCount == uniqualItemCount);
            else
                unit.Log("result: isUniqueItems  {0},   items: {1}    isTrue:  {2}  \r",
                   isUniqueItem, q.ItemCount, q.ItemCount == allitemCount);

          
            for(int i=0;i<q.BlockCount;i++)
            {
               // unit.Log("q.block  [{0}]  : hashCode:  {1}     operater : {2} ", i,  q.GetBlockPair(i).GetHashCode(),q.GetBlockPair(i).Operater);
            }


            if (isUniqueBlock== InsertionBehavior.Overwrite)
                unit.Log("result: IsUniqueBlocks  {0},   items: {1}   blockCount {2}   isTrue:  {3}",
                    isUniqueBlock, q.ItemCount, q.BlockCount, q.BlockCount == uniquebllockCount);
            else
                unit.Log("result: isUniqueItems  {0},   items: {1}  blockCount  {2}   isTrue:  {3}",
                    isUniqueBlock, q.ItemCount, q.BlockCount, q.BlockCount == allbllockCount);

            unit.Log("sumItems: {0}  sumBlocks :   {1}", sumItem, sumBlock);

        }
        

        [TestMethod]
        public void TestBlockRemove()
        {
            var q = new NodeBlock();
            var ids=  q.AddItem(QueryArithmetic.Add,  InsertionBehavior.Duplicates, createitem(3, typeof(string)).ToArray());


            var items = createitem(10, typeof(string)).ToArray();
            var ids2=q.AddItem(QueryArithmetic.Add,  InsertionBehavior.Overwrite, items);

            unit.Log("\n");
            for (var i = 0; i < q.ItemCount; i++)
                unit.Log("Auto: {0}   index: {1}   {2}", ((ValueItem)q.GetItemPair(i).Node).Value,q.GetItemPair(i).Node.GetHashCode(), q.ToString());

            q = new NodeBlock();
            var blockList = new List<NodeBlock>();
            for (var i = 0; i < 10; i++)
            {
                var qx = new NodeBlock();
                qx.AddItem(QueryArithmetic.Add,  InsertionBehavior.Duplicates, createitem(11, typeof(string)).ToArray());
                var qxx = new NodeBlock();
                qxx.AddBlock( QueryArithmetic.Add, InsertionBehavior.Duplicates,qx);
                blockList.Add(qxx);
                
            }
            ids = q.AddBlock(QueryArithmetic.Add,  InsertionBehavior.Duplicates, blockList.ToArray());

            blockList.Clear();
            for (var i = 0; i < 10; i++)
            {
                var qx = new NodeBlock();
                qx.AddItem(QueryArithmetic.Add,  InsertionBehavior.Duplicates, createitem(10, typeof(string),i).ToArray());

                var qxx = new NodeBlock();
                qxx.AddBlock(QueryArithmetic.Add,  InsertionBehavior.Duplicates, qx);
                qxx.AddItem(QueryArithmetic.Add,  InsertionBehavior.Duplicates, new ValueItem() { Value="qxx"});

                var qxxx = new NodeBlock();
                qxxx.AddBlock( QueryArithmetic.Add, InsertionBehavior.Duplicates,qxx);
                var qq = new NodeBlock();
                qq.Add(QueryArithmetic.Add,  InsertionBehavior.Duplicates, qxxx);
                qq.Add(QueryArithmetic.Add,  InsertionBehavior.Duplicates, qxxx);
                blockList.Add(qq);
            }
            ids2 = q.AddBlock(QueryArithmetic.Add,  InsertionBehavior.Duplicates, blockList.ToArray());

            unit.Log("\n");
            for (var i = 0; i < q.BlockCount  ; i++)
                unit.Log("Auto: {0}   index: {1}  ", q.GetBlockPair(i).Node.GetHashCode(),q.GetBlockPair(i).GetHashCode());

            unit.Log("qxxx block   count {0}    qxxx item count    {1}", q.GetBlockPair(15).Node.BlockCount, q.GetBlockPair(15).Node.ItemCount);

            unit.Log("{0}", DateTime.Now.ToString());
        }



        #endregion test blockbase




    }
}
