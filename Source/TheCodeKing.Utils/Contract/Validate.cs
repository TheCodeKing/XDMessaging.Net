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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TheCodeKing.Utils.Contract
{
    public interface IEvaluator<T>
    {
        #region Properties

        string Message { get; }
        string Param { get; }
        T Value { get; }

        #endregion

        #region Public Methods

        void AssertIllegalArgument(string defaultMessage=null);

        void AssertNullArgument(string defaultMessage=null);

        void ArgumentOutOfRange(string defaultMessage=null);

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

        public void ArgumentOutOfRange(string defaultMessage)
        {
            if (string.IsNullOrEmpty(Message) || !string.IsNullOrEmpty(defaultMessage))
            {
                throw new ArgumentOutOfRangeException(Param, Message ?? defaultMessage);
            }
            throw new ArgumentOutOfRangeException(Param);
        }

        public void AssertIllegalArgument(string defaultMessage = null)
        {
            if (string.IsNullOrEmpty(Message) || !string.IsNullOrEmpty(defaultMessage))
            {
                throw new ArgumentException(Message ?? defaultMessage, Param);
            }
            throw new ArgumentException(Param);
        }

        public void AssertNullArgument(string defaultMessage = "cannot be null")
        {
            if (string.IsNullOrEmpty(Message) || !string.IsNullOrEmpty(defaultMessage))
            {
                throw new ArgumentNullException(Param, Message ?? defaultMessage);
            }
            throw new ArgumentNullException(Param);
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
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.IsAbsoluteUri)
            {
                evaluator.AssertIllegalArgument("must be an AbsoluteUri");
            }
        }

        public static void IsNotNull<T>(this IEvaluator<T> evaluator) where T : class
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
        }

        public static void IsNotNull(this IEvaluator<IEnumerable> evaluator)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
        }

        public static void IsNotNullOrEmpty(this IEvaluator<string> evaluator)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument("string cannot be null");
            }
            if (evaluator.Value == string.Empty)
            {
                evaluator.AssertIllegalArgument("string cannot be empty");
            }
        }

        public static void ContainsLessThan(this IEvaluator<ICollection> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count >= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsLessThan(this IEvaluator<IList> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count >= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsGreaterThan(this IEvaluator<ICollection> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count <= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected greater than {0} items", count));
            }
        }

        public static void ContainsGreaterThan(this IEvaluator<IList> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count <= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected greater than {0} items", count));
            }
        }

        public static void ContainsGreaterThan(this IEvaluator<Array> evaluator, uint size)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Length <= size)
            {
                evaluator.ArgumentOutOfRange();
            }
        }

        public static void ContainsLessThan(this IEvaluator<Array> evaluator, uint size)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Length >= size)
            {
                evaluator.ArgumentOutOfRange();
            }
        }


        public static void ContainsGreaterThan<TE>(this IEvaluator<ICollection<TE>> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count <= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected greater than {0} items", count));
            }
        }

        public static void ContainsGreaterThan<TE>(this IEvaluator<IList<TE>> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count <= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected greater than {0} items", count));
            }
        }

        public static void ContainsGreaterThan<TE>(this IEvaluator<IEnumerable<TE>> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count() <= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected greater than {0} items", count));
            }
        }

        public static void ContainsGreaterThan<TE>(this IEvaluator<TE[]> evaluator, uint size)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Length <= size)
            {
                evaluator.ArgumentOutOfRange();
            }
        }

        public static void ContainsLessThan<TE>(this IEvaluator<ICollection<TE>> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count >= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsLessThan<TE>(this IEvaluator<IList<TE>> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count >= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsLessThan<TE>(this IEvaluator<IEnumerable<TE>> evaluator, int count)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Count() >= count)
            {
                evaluator.ArgumentOutOfRange(string.Format("expected less than {0} items", count));
            }
        }

        public static void ContainsLessThan<TE>(this IEvaluator<TE[]> evaluator, uint size)
        {
            if (evaluator.Value == null)
            {
                evaluator.AssertNullArgument();
            }
            if (evaluator.Value.Length >= size)
            {
                evaluator.ArgumentOutOfRange();
            }
        }
        
        #endregion
    }
}