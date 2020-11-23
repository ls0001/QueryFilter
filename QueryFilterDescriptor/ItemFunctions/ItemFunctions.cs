using System;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{


    #region DateTimeFunctions

    public abstract class DateTimeAddDouble : ItemFunction
    {
        private static readonly FunctionArgument[] _declaringAgments = new FunctionArgument[] { new FunctionArgument(false, Constants.Static_TypeOfDouble) };

        #region .ctor
        public DateTimeAddDouble(string methodName, Type returnType, params double[] ArgumentsValues)
            : base(methodName, returnType, ConvertArgsValueToObjectArray(ArgumentsValues), _declaringAgments) { }

        #endregion .ctor

    }

    public abstract class DateTimeAddInt32 : ItemFunction
    {

        private static readonly FunctionArgument[] _DeclaringAgments = new FunctionArgument[] { new FunctionArgument(false, Constants.Static_TypeOfInt32) };

        #region .ctor
        public DateTimeAddInt32(string methodName, Type returnType, params int[] ArgumentsValues)
            : base(methodName, returnType, ConvertArgsValueToObjectArray(ArgumentsValues), _DeclaringAgments) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddTimeSpan : ItemFunction
    {
        private const string _methodName = "Add";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;
        private static readonly FunctionArgument[] _declaringAgments = new FunctionArgument[] { new FunctionArgument(false, Constants.Static_TypeOfTimeSpan) };

        #region .ctor
        public DateTimeAddTimeSpan(params TimeSpan[] ArgumentsValues) 
            : base(_methodName, _returnType, ConvertArgsValueToObjectArray(ArgumentsValues),_declaringAgments) {}

        #endregion .ctor

    }

    public sealed class DateTimeAddYears : DateTimeAddInt32
    {

        private const string _methodName = "AddYears";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;
        #region .ctor
        public DateTimeAddYears(params int[] years) : base(_methodName, _returnType, years) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddMonths : DateTimeAddInt32
    {

        private const string _methodName = "AddMonths";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;

        #region .ctor
        public DateTimeAddMonths(params int[] months) : base(_methodName, _returnType, months) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddDays : DateTimeAddDouble
    {

        private const string _methodName = "AddDays";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;
        #region .ctor
        public DateTimeAddDays(params double[] days) : base(_methodName, _returnType, days) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddHours : DateTimeAddDouble
    {

        private const string _methodName = "AddHours";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;

        #region .ctor
        public DateTimeAddHours(params double[] hours) : base(_methodName, _returnType, hours) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddMinutes : DateTimeAddDouble
    {
        private const string _methodName = "AddMinutes";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;

        #region .ctor
        public DateTimeAddMinutes(params double[] Minutes) : base(_methodName, _returnType, Minutes) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddSeconds : DateTimeAddDouble
    {
        private const string _methodName = "AddSeconds";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;

        #region .ctor
        public DateTimeAddSeconds(params double[] seconds) : base(_methodName, _returnType, seconds) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddMilliseconds : DateTimeAddDouble
    {
        private const string _methodName = "AddMilliseconds";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;

        #region .ctor
        public DateTimeAddMilliseconds(params double[] Millis) : base(_methodName, _returnType, Millis) { }

        #endregion .ctor

    }

    public sealed class DateTimeAddTicks : DateTimeAddDouble
    {
        private const string _methodName = "AddTicks";
        private static readonly Type _returnType = Constants.Static_TypeOfDateTime;

        #region .ctor
        public DateTimeAddTicks(params double[] ticks) : base(_methodName, _returnType, ticks) { }

        #endregion .ctor

    }


    #endregion DateTimeFunctions


    public enum DateTimeFunction
    {
        AddYears=101,
        AddMonths,
        AddDays,
        AddHours,
        AddMinutes,
        AddSeconds,
        AddMilliseconds,
        AddTicks,
        AddTimeSpan,

    }


}
