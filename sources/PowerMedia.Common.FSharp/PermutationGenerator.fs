namespace PowerMedia.Common.Data

open System
open System.Collections.Generic
open System.Linq
open System.Text

type public PermutationGenerator2 = 
    static member GetPermutations elementsToPermutate =
        elementsToPermutate