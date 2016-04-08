#region Using Statements
using System;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Enum indicating what search algorithm to use.
    /// </summary>
    public enum SearchAlgorithm
    {
        Default = 0,
        DepthFirst = 0,
        BreadthFirst = 1,
        ReverseDepthFirst = 2,
        ReverseBreadthFirst= 3
    }
}
