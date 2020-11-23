using System;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 方法参数
    /// </summary>
    public sealed class FunctionArgument
    {
        private Type _type;
        internal FunctionArgument(bool canNull, Type type)
        {
            Type = type;
            IsCantNullValue = canNull;
        }
        internal FunctionArgument(Type type)
        {
            IsCantNullValue = false;
        }
        public Type Type { get => _type; protected set => _type = value ?? throw new NullReferenceException(); }

        public object DefaultValue { get; protected set; }

        public bool IsCantNullValue { get; protected set; }

        public bool IsOut { get; protected set; }

        public bool IsOptional { get; protected set; }

        public bool IsRetval { get; protected set; }

    }


}
