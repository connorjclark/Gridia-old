#region Using Statements
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Represents size in pixels, elements or percentage.
    /// </summary>
    [Serializable]
    public class ElementSize
    {
        #region Fields

        [SerializeField]
        private float _value;

        [SerializeField]
        private ElementSizeUnit _unit;

        [SerializeField]
        private bool _fill;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSize()
        {
            _value = 0f;
            _unit = ElementSizeUnit.Pixels;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSize(float value, ElementSizeUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSize(ElementSize elementSize)
        {
            _value = elementSize.Value;
            _unit = elementSize.Unit;
            _fill = elementSize.Fill;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets element size with the specified pixel size.
        /// </summary>
        public static ElementSize GetPixels(float pixels)
        {
            return new ElementSize(pixels, ElementSizeUnit.Pixels);
        }

        /// <summary>
        /// Gets element size with the specified element size.
        /// </summary>
        public static ElementSize GetElements(float elements)
        {
            return new ElementSize(elements, ElementSizeUnit.Elements);
        }

        /// <summary>
        /// Gets element size with the specified percent size.
        /// </summary>
        public static ElementSize GetPercents(float percents)
        {
            return new ElementSize(percents, ElementSizeUnit.Percents);
        }

        /// <summary>
        /// Converts elements to pixels.
        /// </summary>
        public static float ElementsToPixels(float elements)
        {                        
            return ViewData.ElementSize * elements;
        }

        /// <summary>
        /// Converts pixels to elements.
        /// </summary>
        public static float PixelsToElements(float pixels)
        {
            return pixels / ViewData.ElementSize;
        }

        /// <summary>
        /// Parses string into element size.
        /// </summary>
        public static ElementSize Parse(string value)
        {
            ElementSize elementSize = new ElementSize();
            string trimmedValue = value.Trim();
            if (trimmedValue == "*")
            {
                elementSize.Value = 1;
                elementSize.Unit = ElementSizeUnit.Percents;
                elementSize.Fill = true;
            }
            else if (trimmedValue.EndsWith("em", StringComparison.OrdinalIgnoreCase))
            {
                int lastIndex = trimmedValue.LastIndexOf("em", StringComparison.OrdinalIgnoreCase);
                elementSize.Value = System.Convert.ToSingle(trimmedValue.Substring(0, lastIndex), CultureInfo.InvariantCulture);
                elementSize.Unit = ElementSizeUnit.Elements;
            }
            else if (trimmedValue.EndsWith("%"))
            {
                int lastIndex = trimmedValue.LastIndexOf("%", StringComparison.OrdinalIgnoreCase);
                elementSize.Value = System.Convert.ToSingle(trimmedValue.Substring(0, lastIndex), CultureInfo.InvariantCulture) / 100.0f;
                elementSize.Unit = ElementSizeUnit.Percents;
            }
            else if (trimmedValue.EndsWith("px"))
            {
                int lastIndex = trimmedValue.LastIndexOf("px", StringComparison.OrdinalIgnoreCase);
                elementSize.Value = System.Convert.ToSingle(trimmedValue.Substring(0, lastIndex), CultureInfo.InvariantCulture);
                elementSize.Unit = ElementSizeUnit.Pixels;
            }
            else
            {
                elementSize.Value = System.Convert.ToSingle(trimmedValue, CultureInfo.InvariantCulture);
                elementSize.Unit = ElementSizeUnit.Pixels;
            }

            return elementSize;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets element size value.
        /// </summary>
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Gets or sets element size in pixels.
        /// </summary>
        public float Pixels
        {
            get
            {
                if (_unit == ElementSizeUnit.Pixels)
                {
                    return _value;
                }
                else if (_unit == ElementSizeUnit.Elements)
                {
                    return ElementsToPixels(_value);
                }
                else
                {
                    return 0f;
                }
            }
        }

        /// <summary>
        /// Gets or sets element size in elements.
        /// </summary>
        public float Elements
        {
            get
            {
                if (_unit == ElementSizeUnit.Pixels)
                {
                    return PixelsToElements(_value);
                }
                else if (_unit == ElementSizeUnit.Elements)
                {
                    return _value;
                }
                else
                {
                    return 0f;
                }
            }
        }

        /// <summary>
        /// Gets or sets element size in percents.
        /// </summary>
        public float Percent
        {
            get
            {
                return _unit == ElementSizeUnit.Percents ? _value : 0f;
            }
        }
        
        /// <summary>
        /// Gets or sets element size unit.
        /// </summary>
        public ElementSizeUnit Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        /// <summary>
        /// Gets or sets boolean indicating if element size is to fill out remaining space (used by DataGrid).
        /// </summary>
        public bool Fill
        {
            get
            {
                return _fill;
            }
            set
            {
                _fill = value;
            }
        }

        #endregion
    }
}
