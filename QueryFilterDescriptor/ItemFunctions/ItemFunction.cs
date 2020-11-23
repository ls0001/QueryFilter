using System;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// <inheritdoc cref="IItemFunction"/>
    /// </summary>
    public abstract class ItemFunction : IItemFunction
    {
        private object[] _Arguments;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="returnType"><inheritdoc cref="ReturnType"/></param>
        /// <param name="argsValues"><inheritdoc cref="ArgumentsValues"/></param>
        /// <param name="declaringArgs"><inheritdoc cref="DeclaringArguments"/></param>
        protected ItemFunction(string name, Type returnType, object[] argsValues, FunctionArgument[] declaringArgs)
        {
            Name = name;
            DeclaringArguments = declaringArgs;   //必须先于ArgumentsValues设置，Arguments依赖与它。
            ArgumentsValues = argsValues;
            ReturnType = returnType;
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 方法签名参数的值
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public object[] ArgumentsValues
        {
            get => _Arguments;
            internal protected set
            {
                CheckArguments(value);
                _Arguments = value;
            }
        }

        /// <summary>
        /// 获取参数类型列表
        /// </summary>
        public Type[] ArgumentsTypes
        {
            get
            {
                var args = DeclaringArguments;
                var tp = new Type[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    tp[i] = args[i].Type;
                }
                return tp;
            }
        }

        /// <summary>
        /// 获取返回值类型
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// 对应方法定义的签名参数
        /// </summary>
        protected FunctionArgument[] DeclaringArguments { get; }


        /// <summary>
        /// 检测参数值的合法性
        /// </summary>
        /// <param name="arguments">参数值列表</param>
        /// <returns></returns>
        protected bool CheckArguments(object[] arguments)
        {
            var args = arguments;
            var declaringArgs = DeclaringArguments;
            var argDeclareCount = declaringArgs?.Length ?? 0;
            if ((args?.Length ?? 0) != argDeclareCount)
                throw new ArgumentOutOfRangeException(MsgStrings.ArgumentsIndexOutOfRange($"{this.GetType().FullName}", this.Name));
            //throw new ArgumentOutOfRangeException(($"{this.GetType().FullName}, {this.MethodName}"));
            else if (args == null || declaringArgs == null)
            {
                return true;
            }

            for (int i = 0; i < argDeclareCount; i++)
            {
                var declArg = declaringArgs?[i];
                var val = args[i];
                if (val == null && !declArg.IsCantNullValue)
                    throw new ArgumentNullException(MsgStrings.ValueCannotNull($"{this.GetType().FullName}:\n\r{this.Name}:Arguments{i}"));
                //throw new ArgumentNullException(($"{this.GetType().FullName}:\n\r{this.MethodName}:Arguments{i}"));

                if (val != null && val.GetType() != declArg.Type)
                    throw new ArgumentException(MsgStrings.InvalidParamType(this.GetType().FullName, $"{this.Name}:Arguments[{i}]: {declArg?.Type?.Name}\n\r", declArg?.Type));
                //throw new ArgumentException(($"{this.MethodName}:Arguments[{i}]: {declArg.Type?.Name}\n\r, argTypes[i].Type"));

            }
            return true;
        }

        protected static object[] ConvertArgsValueToObjectArray<T>(T[] values)
        {
            if (values == null)
                return null;
            var tmpValues = new object[values.Length];
            values.CopyTo(tmpValues, 0);
            return tmpValues;
        }


    }


}
