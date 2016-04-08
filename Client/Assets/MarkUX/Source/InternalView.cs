#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Indicates that the view shouldn't be used as a top-level (main) view and hides it from the ViewEngine's drop-down of views.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class InternalView : Attribute
    {
    }
}
