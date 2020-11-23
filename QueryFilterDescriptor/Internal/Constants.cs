using System;

namespace Sag.Data.Common.Query.Internal
{
    internal static partial class Constants
    {
        public const string Const_QueryBlock_AutoReducePtyName = "AutoReduce";
        public const string Const_QueryBlock_TypeAsPtyName = "TypeAs";
        public const string Const_QueryBlock_ItemsPtyName = "Items";
        public const string Const_QueryBlock_BlocksPtyName = "Blocks";

        public const string Const_QueryItem_FlagPtyName = "Flag";

        public const string Const_NodeItem_TypeAsPtyName = "TypeAs";
        public const string Const_NodeItem_ValuePtyName = "Value";


        public const string Const_PropertyItem_ClassTypeName = "ClassType";
        public const string Const_PropertyItem_NamePtyName = "Name";
        public const string Const_PropertyItem_TypeAsPtyName = "TypeAs";

        public const string Const_NodeOperatorPair_OperatorPtyName = "Operator";
        public const string Const_NodeOperatorPair_NodePtyName = "Node";




        public static readonly Type Static_TypeOfQueryNode = typeof(QueryNode);
        public static readonly Type Static_TypeOfQueryItem = typeof(QueryItem);
        public static readonly Type Static_TypeofQueryBlockOpenGeneric = typeof(QueryBlock<,,>);
        public static readonly Type Static_TypeOfNodeOperatorOpenGeneric = typeof(NodeOperatorPair<,>);
        public static readonly Type Static_TypeOfNodeCollectionOpenGeneric = typeof(NodeCollection<,>);


        public static readonly Type Static_TypeOfObject = typeof(object);
        public static readonly Type Static_TypeOfString = typeof(string);
        public static readonly Type Static_TypeOfChar = typeof(char);
        public static readonly Type Static_TypeOfBool = typeof(bool);
        public static readonly Type Static_TypeOfDateTime = typeof(DateTime);
        public static readonly Type Static_TypeOfDateTimeOfSet = typeof(DateTimeOffset);
        public static readonly Type Static_TypeOfTimeSpan = typeof(TimeSpan);
        public static readonly Type Static_TypeOfByte = typeof(Byte);
        public static readonly Type Static_TypeOfInt32 = typeof(Int32);
        public static readonly Type Static_TypeOfInt64 = typeof(Int64);
        public static readonly Type Static_TypeOfSingle = typeof(Single);
        //public static readonly Type Static_TypeOfFloat = Static_TypeOfSingle;
        public static readonly Type Static_TypeOfDouble = typeof(Double);
        public static readonly Type Static_TypeOfDecimal = typeof(Decimal);

    }
}
