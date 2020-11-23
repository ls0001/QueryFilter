using System;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 对项进行独立计算的函数
    /// </summary>
    public interface IItemFunction
    {
        /// <summary>
        /// 方法名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取方法签名参数的值
        /// </summary>
        public object[] ArgumentsValues { get; }

        /// <summary>
        /// 参数类型列表
        /// </summary>
        public Type[] ArgumentsTypes { get; }

        /// <summary>
        /// 返回值类型
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// 是否可在指定类型上执行
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsCanInvokeBy(Type type)
        {
            return type.GetMethod(Name, ArgumentsTypes) != null;
        }
    }


}
