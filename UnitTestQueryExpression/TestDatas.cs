using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestQueryExpression
{
    public class TestDataEntity
    {
        public string strProperty { get; set; }
        public int intProperty { get; set; }

        public double doubleProperty { get; set; }
        public DateTime dateTimeProperty { get; set; }
        
        //public List<string> lstStrProperty { get; set; }

        //public List<int> lstIntProperty { get; set; }
        //public List<double> lstDoubleProperty { get; set; }

    }

    public class TestDataSet:HashSet<TestDataEntity>
    {

    }

    public class DataGeter
    {
       static public TestDataSet GetTestEntitySet ()
        {
            var recordCount = 1000;
            var dset = new TestDataSet();
            for(int i=0;i<recordCount;i++)
            {
                var ent = new TestDataEntity()
                {
                    dateTimeProperty = DateTime.UtcNow.AddDays(i)
                  , doubleProperty = 1000d * (double)i / 3d
                  , intProperty = i
                  //, lstDoubleProperty = new List<double>() { 100d * (double)i / 3d, 100d * 3d / (double)i, 100d * 0.2d * (double)i }
                  //, lstIntProperty = new List<int>() { i, 10, i * 2, i * 3, i * 4 }
                  //, lstStrProperty = new List<string>() { i.ToString(), (i * 2).ToString(), (i * 3).ToString() }
                  , strProperty = "strProperty:" + i.ToString()
                };
                dset.Add(ent);
            }
            return dset;
        }
    }
}
