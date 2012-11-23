using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace TheCodeKing.Utils.Contract
{
    public interface IEvaluator<out T>
    {
        #region Properties

        string Message { get; }
        string Param { get; }
        T Value { get; }

        #endregion
    }

    public class Evaluator<T> : IEvaluator<T>
    {
        #region Constants and Fields

        private readonly string message;
        private readonly string param;
        private readonly T value;

        #endregion

        #region Constructors and Destructors

        public Evaluator(T value, string param)
            : this(value, param, null)
        {
        }

        public Evaluator(T value, string param, string message)
        {
            this.value = value;
            this.message = message;
            this.param = param;
        }

        #endregion

        #region Properties

        public string Message
        {
            get { return message; }
        }

        public string Param
        {
            get { return param; }
        }

        public T Value
        {
            get { return value; }
        }

        #endregion
    }

    public static class Validate
    {
        #region Public Methods

        public static void IsNotNull<T>(this IEvaluator<T> evaluator) where T : class
        {
            if (evaluator.Value == null)
            {
                AssertNullArgument(evaluator);
            }
        }

        public static void IsNotNull(this IEvaluator<Uri> evaluator)
        {
            if (evaluator.Value == null)
            {
                AssertNullArgument(evaluator);
            }
            if (evaluator.Value.IsAbsoluteUri)
            {
                AssertIllegalArgument(evaluator);
            }
        }

        public static void IsNotNull(this IEvaluator<IEnumerable> evaluator)
        {
            if (evaluator.Value == null)
            {
                AssertNullArgument(evaluator);
            }
        }

        public static void IsNotNullOrEmpty(this IEvaluator<string> evaluator)
        {
            if (evaluator.Value == null)
            {
                AssertNullArgument(evaluator);
            }
            if (evaluator.Value == string.Empty)
            {
                AssertIllegalArgument(evaluator);
            }
        }

        public static IEvaluator<T> That<T>(T value)
        {
            var stackTrace = new StackTrace();
            var parameter = stackTrace.GetFrame(1).GetMethod().GetParameters().First();

            return new Evaluator<T>(value, parameter.Name);
        }

        public static IEvaluator<T> That<T>(T value, string message)
        {
            var stackTrace = new StackTrace();
            var parameters = stackTrace.GetFrame(1).GetMethod().GetParameters().First();

            return new Evaluator<T>(value, parameters.Name, message);
        }

        #endregion

        #region Methods

        private static void AssertIllegalArgument<T>(IEvaluator<T> evaluator)
        {
            if (string.IsNullOrEmpty(evaluator.Message))
            {
                throw new ArgumentException(evaluator.Message, evaluator.Param);
            }
            throw new ArgumentException(evaluator.Param);
        }

        private static void AssertNullArgument<T>(IEvaluator<T> evaluator)
        {
            if (string.IsNullOrEmpty(evaluator.Message))
            {
                throw new ArgumentNullException(evaluator.Message, evaluator.Param);
            }
            throw new ArgumentNullException(evaluator.Param);
        }

        #endregion
    }
}