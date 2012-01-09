using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace PowerMedia.Common.Collections
{

    public static class CollectionUtils
    {
        /// <summary>
        /// Processes an input enumerable collection and returns a list consisting of all the objects from the collection that fulfill a condition represented by isConditionTrue predicate function.
        /// </summary>
        /// <typeparam name="TOutput">Collection's elements type.</typeparam>
        /// <param name="lstInput"></param>
        /// <param name="isConditionTrue"></param>
        /// <returns></returns>
        public static List<TOutput> CopyListElementsThatFulfilCondition<TOutput>(IEnumerable<TOutput> lstInput, Predicate<TOutput> isConditionTrue)
        {
            var lstOutput = new List<TOutput>();
            IEnumerator<TOutput> enumerator = lstInput.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (isConditionTrue(enumerator.Current))
                {
                    lstOutput.Add((TOutput)enumerator.Current);
                }
            }

            return lstOutput;
        }

        public static List<TOutput> ConvertIEnumerableToList<TOutput>( IEnumerable<TOutput> collection )
        {
            return CopyListElementsThatFulfilCondition(collection, item => true);
        }

        public static List<TOutput> CopyOnlyNonNullElements<TOutput>(IEnumerable<TOutput> collection)
        {
            return CopyListElementsThatFulfilCondition(collection, item => item != null);
        }

        /// <summary>
        /// Converts all of the enumerable collection's elements to a given type (TResult).
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<TOutput> ConvertListElements<TOutput>(IEnumerable inputList)
        {
            IEnumerator enumerator = inputList.GetEnumerator();
            var lstOutput = new List<TOutput>();

            while (enumerator.MoveNext())
            {
                lstOutput.Add((TOutput)enumerator.Current);
            }

            return lstOutput;
        }

        public static List<TOutput> ConvertListElements<TInput, TOutput>(IEnumerable<TInput> inputList, Converter<TInput, TOutput> converter)
        {
            IEnumerator<TInput> enumerator = inputList.GetEnumerator();
            var lstOutput = new List<TOutput>();

            while (enumerator.MoveNext())
            {
                lstOutput.Add(converter(enumerator.Current));
            }
            return lstOutput;
        }
        
        public static IEnumerable<Pair<TSource, TResult>> CartesianProduct<TSource, TResult>(this IEnumerable<TSource> collection1, IEnumerable<TResult> collection2)
        {
            if (collection1 == null || collection2 == null)
            {
                throw new ArgumentNullException();
            }
			            
            foreach(var element1 in collection1)
            {
            	foreach(var element2 in collection2)
            	{
            		yield return new Pair<TSource, TResult>(element1, element2);
            	}
            }
        }

        public static IList<T> SortedCopy<T>(this IList<T> list) where T : IComparable
        {
            List<T> result = list.ToList();
            result.Sort((e1, e2) => e1.CompareTo(e2));

            return result;
        }

        public static bool IsEmptyOrNull<T>(IList<T> list)
        {
            if (list == null)
            {
                return true;
            }
            if (list.Count == 0)
            {
                return true;
            }
            return false;
        }

    }
}
