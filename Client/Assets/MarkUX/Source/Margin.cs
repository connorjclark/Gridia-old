#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Represents left, top, right and bottom margins.
    /// </summary>
    [Serializable]
    public class Margin
    {
        #region Fields

        [SerializeField]
        private ElementSize _left;

        [SerializeField]
        private ElementSize _top;

        [SerializeField]
        private ElementSize _right;

        [SerializeField]
        private ElementSize _bottom;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Margin()
        {
            _left = new ElementSize();
            _top = new ElementSize();
            _right = new ElementSize();
            _bottom = new ElementSize();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Margin(float left, float top, float right = 0, float bottom = 0)
        {
            _left = new ElementSize(left, ElementSizeUnit.Pixels);
            _top = new ElementSize(top, ElementSizeUnit.Pixels);
            _right = new ElementSize(right, ElementSizeUnit.Pixels);
            _bottom = new ElementSize(bottom, ElementSizeUnit.Pixels);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Margin(float margin)
        {
            _left = new ElementSize(margin, ElementSizeUnit.Pixels);
            _top = new ElementSize(margin, ElementSizeUnit.Pixels);
            _right = new ElementSize(margin, ElementSizeUnit.Pixels);
            _bottom = new ElementSize(margin, ElementSizeUnit.Pixels);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Margin(ElementSize margin)
        {
            _left = margin;
            _top = margin;
            _right = margin;
            _bottom = margin;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Margin(ElementSize left, ElementSize top)
        {
            _left = left;
            _top = top;
            _right = new ElementSize();
            _bottom = new ElementSize();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Margin(ElementSize left, ElementSize top, ElementSize right)
            : this()
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = new ElementSize();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Margin(ElementSize left, ElementSize top, ElementSize right, ElementSize bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets left margin from left size.
        /// </summary>
        public static Margin FromLeft(ElementSize left)
        {
            return new Margin(left, new ElementSize(), new ElementSize(), new ElementSize());
        }

        /// <summary>
        /// Gets top margin from top size.
        /// </summary>
        public static Margin FromTop(ElementSize top)
        {
            return new Margin(new ElementSize(), top, new ElementSize(), new ElementSize());
        }

        /// <summary>
        /// Gets right margin from right size.
        /// </summary>
        public static Margin FromRight(ElementSize right)
        {
            return new Margin(new ElementSize(), new ElementSize(), right, new ElementSize());
        }

        /// <summary>
        /// Gets bottom margin from bottom size.
        /// </summary>
        public static Margin FromBottom(ElementSize bottom)
        {
            return new Margin(new ElementSize(), new ElementSize(), new ElementSize(), bottom);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets left margin.
        /// </summary>
        public ElementSize Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
            }
        }

        /// <summary>
        /// Gets or sets top margin.
        /// </summary>
        public ElementSize Top
        {
            get
            {
                return _top;
            }
            set
            {
                _top = value;
            }
        }

        /// <summary>
        /// Gets or sets right margin.
        /// </summary>
        public ElementSize Right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
            }
        }

        /// <summary>
        /// Gets or sets bottom margin.
        /// </summary>
        public ElementSize Bottom
        {
            get
            {
                return _bottom;
            }
            set
            {
                _bottom = value;
            }
        }

        #endregion
    }
}
