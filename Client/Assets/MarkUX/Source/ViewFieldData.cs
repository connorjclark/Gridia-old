#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Contains data about a view field.
    /// </summary>
    public class ViewFieldData
    {
        #region Fields

        public FieldInfo ClassFieldInfo;
        public FieldInfo ObjectFieldInfo;
        public object Object;

        #endregion
    }
}
