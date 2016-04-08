#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MarkUX.Editor
{
    /// <summary>
    /// Contains information about a theme xml element.
    /// </summary>
    internal class ThemeElement
    {
        #region Fields

        private string _id;
        private string _style;
        private Type _viewType;
        private Type _parentViewType;
        private string _baseDirectory;
        private Dictionary<string, string> _values;

        #endregion

        #region Properties

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Style
        {
            get { return _style; }
            set { _style = value; }
        }

        public Type ViewType
        {
            get { return _viewType; }
            set { _viewType = value; }
        }

        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set { _baseDirectory = value; }
        }

        public Dictionary<string, string> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public Type ParentViewType
        {
            get { return _parentViewType; }
            set { _parentViewType = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public ThemeElement()
        {
            _values = new Dictionary<string, string>();
        }

        #endregion

    }
}
