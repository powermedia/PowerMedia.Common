using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerMedia.Common.Data
{
    
    public static class PermutationGenerator
    {
     public static HashSet<IList<T>> GetPermutations<T>(IList<T> elementsToPermutate)
        {
            if (elementsToPermutate == null)
            {
                throw new ArgumentNullException("elementsToPermutate");
            }
            IList<T> prefix = new List<T>();
            HashSet<IList<T>> permutations = new HashSet<IList<T>>();
            GetPermutationsInternal<T>(elementsToPermutate, prefix, permutations,  elementsToPermutate.Count);
            return permutations;
        }

        //FIXME: performance
        private static void GetPermutationsInternal<T>(IList<T> elements, IList<T> prefix, HashSet<IList<T>> permutations, int expectedPermutationLength)
        {
            if (prefix.Count == expectedPermutationLength)
            {
                permutations.Add(prefix);
                return;
            }
            int elementsCount = elements.Count;
            for (int i = 0; i < elementsCount; i++)
            {
                T element = elements[i];
                List<T> subSetOfElements = new List<T>(elements); //FIXME: most lagging here
                subSetOfElements.RemoveAt(i);
                List<T> newPrefix = new List<T>(prefix) { element };
                GetPermutationsInternal<T>(subSetOfElements, newPrefix, permutations, expectedPermutationLength);
            }
        }
    }
}
