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
    /// Contains information about the path to an object.
    /// </summary>
    public class FieldPath
    {
        #region Fields

        public object _object;
        public List<KeyValuePair<string, int>> _path;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initalizes a new instance of the class.
        /// </summary>
        public FieldPath()
        {
            _object = null;
            _path = new List<KeyValuePair<string, int>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a field path from a string.
        /// </summary>
        public static FieldPath From(string fieldPathString)
        {
            var fieldPath = new FieldPath();
            return fieldPath;
        }

        #endregion

        #region Properties

        #endregion
    }
}
