#region Using Statements
using System;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Enum indicating how items are selected.
    /// </summary>
    public enum SelectionMode
    {
        Click = 0,
        DoubleClick = 1,
        DoubleClickFocusAndSelect = 2,
        None = 3
    }
}
