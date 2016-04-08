#region Using Statements
using System;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Enum indicating horizontal and vertical alignment.
    /// </summary>
    [Flags]
    public enum Alignment
    {
        Center = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right
    }
}
