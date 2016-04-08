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
    /// Indicates that a view creates views of specified type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CreatesView : Attribute
    {
        #region Fields

        protected Type _type;
        protected string _style;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public CreatesView(Type type, string style = "")
        {
            _type = type;
            _style = style;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets type of view template referenced.
        /// </summary>
        public Type Type
        {
            get 
            {
                return _type;
            }
        }

        /// <summary>
        /// Gets style template should be instantiated with.
        /// </summary>
        public string Style
        {
            get
            {
                return _style;
            }
        }

        #endregion
    }
}
