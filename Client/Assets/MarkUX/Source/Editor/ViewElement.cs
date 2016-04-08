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
    /// Contains information about a view xml element.
    /// </summary>
    internal class ViewElement
    {
        #region Fields

        private string _assetName;
        private string _name;
        private XElement _element;
        private List<ViewElement> _dependencies;
        private List<string> _dependencyNames;
        private bool _permanentMark;
        private bool _temporaryMark;
        private Type _viewType;
        private List<FieldInfo> _viewActionFields;
        private bool _isInternal;

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets asset file name.
        /// </summary>
        public string AssetName
        {
            get { return _assetName; }
            set { _assetName = value; }
        }

        /// <summary>
        /// Gets or sets name of view element.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets XML element.
        /// </summary>
        public XElement Element
        {
            get
            {
                return _element;
            }
            set
            {
                _element = value;
            }
        }

        /// <summary>
        /// Gets or sets view element dependencies.
        /// </summary>
        public List<ViewElement> Dependencies
        {
            get
            {
                return _dependencies;
            }
            set
            {
                _dependencies = value;
            }
        }

        /// <summary>
        /// Gets or sets view dependencies names.
        /// </summary>
        public List<string> DependencyNames
        {
            get
            {
                return _dependencyNames;
            }
            set
            {
                _dependencyNames = value;
            }
        }

        /// <summary>
        /// Gets or sets mark used when determening dependencies.
        /// </summary>
        public bool PermanentMark
        {
            get
            {
                return _permanentMark;
            }
            set
            {
                _permanentMark = value;
            }
        }

        /// <summary>
        /// Gets or sets mark used when determening dependencies.
        /// </summary>
        public bool TemporaryMark
        {
            get
            {
                return _temporaryMark;
            }
            set
            {
                _temporaryMark = value;
            }
        }

        /// <summary>
        /// Gets or sets view type.
        /// </summary>
        public Type ViewType
        {
            get
            {
                return _viewType;
            }
            set
            {
                _viewType = value;
            }
        }

        /// <summary>
        /// Gets or sets view action fields.
        /// </summary>
        public List<FieldInfo> ViewActionFields
        {
            get
            {
                return _viewActionFields;
            }
            set
            {
                _viewActionFields = value;
            }
        }

        /// <summary>
        /// Gets or sets bool indicating if view is internal.
        /// </summary>
        public bool IsInternal
        {
            get { return _isInternal; }
            set { _isInternal = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public ViewElement()
        {
            _dependencies = new List<ViewElement>();
            _dependencyNames = new List<string>();
            _viewActionFields = new List<FieldInfo>();
        }

        #endregion
    }
}
