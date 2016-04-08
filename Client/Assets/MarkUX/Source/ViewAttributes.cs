#region Using Statements
using System;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Tells the view processor that a view field shouldn't be set from xml.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class NotSetFromXml : Attribute
    {
    }

    /// <summary>
    /// Tells the view processor to add a script component when the view object is created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AddComponent : Attribute
    {
        #region Fields

        private Type _componentType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AddComponent(Type componentType)
        {
            _componentType = componentType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets component type.
        /// </summary>
        public Type ComponentType
        {
            get 
            {
                return _componentType;
            }
            set
            {
                _componentType = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Tells the view processor to remove a script component when the view object is created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RemoveComponent : Attribute
    {
        #region Fields

        private Type _componentType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public RemoveComponent(Type componentType)
        {
            _componentType = componentType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets component type.
        /// </summary>
        public Type ComponentType
        {
            get
            {
                return _componentType;
            }
            set
            {
                _componentType = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Sets a view field change handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ChangeHandler : Attribute
    {
        #region Fields

        private string _changeHandlerName;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ChangeHandler(string changeHandlerName)
        {
            _changeHandlerName = changeHandlerName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets component type.
        /// </summary>
        public string ChangeHandlerName
        {
            get
            {
                return _changeHandlerName;
            }
            set
            {
                _changeHandlerName = value;
            }
        }

        #endregion
    }
}
