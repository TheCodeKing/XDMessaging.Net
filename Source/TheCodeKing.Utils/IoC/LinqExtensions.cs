using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheCodeKing.Utils.IoC
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> SkipExceptions<T>(this IEnumerable<T> values)
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
                    catch
                    {
                        continue;
                    }

                    if (next)
                    {
                        yield return enumerator.Current;
                    }
                }
            }
        }
    }
}
