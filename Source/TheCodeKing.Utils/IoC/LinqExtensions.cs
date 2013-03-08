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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheCodeKing.Utils.IoC
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> WrapExceptions<T>(this IEnumerable<T> values, Func<Exception, Exception> exception)
        {
            using (var enumerator = values.GetEnumerator())
            {
                bool next = true;
                while (next)
                {
                    try
                    {
                        next = enumerator.MoveNext();
                    }
                    catch (Exception e)
                    {
                        throw exception(e);
                    }

                    if (next)
                    {
                        yield return enumerator.Current;
                    }
                }
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                        Func<TSource, TKey> keySelector,
                                                                        IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }
    }
}