/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
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

        #region Public Methods

        void AssertIllegalArgument<T>(IEvaluator<T> evaluator, string defaultMessage=null);

        void AssertNullArgument<T>(IEvaluator<T> evaluator, string defaultMessage=null);

        void ArgumentOutOfRange<T>(IEvaluator<T> evaluator, string defaultMessage=null);

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

        #region Implemented Interfaces

        #region IEvaluator<T>

        public void ArgumentOutOfRange<T>(IEvaluator<T> evaluator, string defaultMessage)
        {
            if (string.IsNullOrEmpty(evaluator.Message) || !string.IsNullOrEmpty(defaultMessage))
            {
                throw new ArgumentOutOfRangeException(evaluator.Param, evaluator.Message ?? defaultMessage);
            }
            throw new ArgumentOutOfRangeException(evaluator.Param);
        }

        public void AssertIllegalArgument<T>(IEvaluator<T> evaluator, string defaultMessage = null)
        {
            if (string.IsNullOrEmpty(evaluator.Message) || !string.IsNullOrEmpty(defaultMessage))
            {
                throw new ArgumentException(evaluator.Message ?? defaultMessage, evaluator.Param);
            }
            throw new ArgumentException(evaluator.Param);
        }

        public void AssertNullArgument<T>(IEvaluator<T> evaluator, string defaultMessage = "cannot be null")
        {
            if (string.IsNullOrEmpty(evaluator.Message) || !string.IsNullOrEmpty(defaultMessage))
            {
                throw new ArgumentNullException(evaluator.Param, evaluator.Message ?? defaultMessage);
            }
            throw new ArgumentNullException(evaluator.Param);
        }

        #endregion

        #endregion
    }

    public static class Validate
    {
        #region Public Methods

        public static IEvaluator<T> That<T>(T value)
        {
            var stackTrace = new StackTrace();
            var parameter = "param";
            if (stackTrace.FrameCount > 0)
            {
                var parameters = stackTrace.GetFrame(1).GetMethod().GetParameters().FirstOrDefault();
                parameter = parameters == null ? "param" : parameters.Name;
            }
            return new Evaluator<T>(value, parameter);
        }

        public static IEvaluator<T> That<T>(T value, string message)
        {
            var stackTrace = new StackTrace();
           var parameter = "param";
            if (stackTrace.FrameCount > 0)
            {
                var parameters = stackTrace.GetFrame(1).GetMethod().GetParameters().FirstOrDefault();
                parameter = parameters == null ? "param" : parameters.Name;
            }
            return new Evaluator<T>(value, parameter, message);
        }

        #endregion
    }

    public static class ValidateExtensions
    {
        #region Public Methods

        public static void IsAbsoluteUri(this IEvaluator<Uri> evaluator)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
            if (evaluator.Value.IsAbsoluteUri)
            {
                evaluator.AssertIllegalArgument(evaluator, "must be an AbsoluteUri");
            }
        }

        public static void IsNotNull<T>(this IEvaluator<T> evaluator) where T : class
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
        }

        public static void IsNotNull(this IEvaluator<IEnumerable> evaluator)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
        }

        public static void IsNotNullOrEmpty(this IEvaluator<string> evaluator)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator, "string cannot be null");
            }
            if (evaluator.Value == string.Empty)
            {
                evaluator.AssertIllegalArgument(evaluator, "string cannot be empty");
            }
        }

        public static void ContainsLessThan(this IEvaluator<ICollection> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
            if (evaluator.Value.Count > count)
            {
                evaluator.ArgumentOutOfRange(evaluator, string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsLessThan(this IEvaluator<IList> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
            if (evaluator.Value.Count < count)
            {
                evaluator.ArgumentOutOfRange(evaluator, string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsLessThan(this IEvaluator<IEnumerable> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
            if (evaluator.Value.Cast<object>().Count() < count)
            {
                evaluator.ArgumentOutOfRange(evaluator, string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsGreaterThan(this IEvaluator<ICollection> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
            if (evaluator.Value.Count > count)
            {
                evaluator.ArgumentOutOfRange(evaluator, string.Format("expected greater than {0} items", count));
            }
        }

        public static void ContainsGreaterThan(this IEvaluator<IList> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
            if (evaluator.Value.Count > count)
            {
                evaluator.ArgumentOutOfRange(evaluator, string.Format("expected greater than {0} items", count));
            }
        }

        public static void ContainsGreaterThan(this IEvaluator<IEnumerable> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument(evaluator);
            }
            if (evaluator.Value.Cast<object>().Count()>count)
            {
                evaluator.ArgumentOutOfRange(evaluator, string.Format("expected greater than {0} items", count));
            }
        }
        
        #endregion
    }
}