#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
#endregion

namespace MarkUX.ValueConverters
{
    /// <summary>
    /// Value converter for Margin type.
    /// </summary>
    public class MarginValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MarginValueConverter()
        {
            _type = typeof(Margin);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Margin type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
            {
                return base.Convert(value, context);
            }

            if (value.GetType() == typeof(string))
            {
                var stringValue = (string)value;
                try
                {
                    string[] valueList;
                    valueList = stringValue.Split(',').ToArray();
                    Margin convertedValue = null;
                    if (valueList.Length == 1)
                    {
                        convertedValue = new Margin(ElementSize.Parse(valueList[0]));
                    }
                    else if (valueList.Length == 2)
                    {
                        convertedValue = new Margin(
                            ElementSize.Parse(valueList[0]),
                            ElementSize.Parse(valueList[1]));
                    }
                    else if (valueList.Length == 3)
                    {
                        convertedValue = new Margin(
                            ElementSize.Parse(valueList[0]),
                            ElementSize.Parse(valueList[1]),
                            ElementSize.Parse(valueList[2]));
                    }
                    else if (valueList.Length == 4)
                    {
                        convertedValue = new Margin(
                            ElementSize.Parse(valueList[0]),
                            ElementSize.Parse(valueList[1]),
                            ElementSize.Parse(valueList[2]),
                            ElementSize.Parse(valueList[3]));
                    }
                    else
                    {
                        return StringConversionFailed(value);
                    }

                    return new ConversionResult(convertedValue);
                }
                catch (Exception e)
                {
                    return ConversionFailed(value, e);
                }
            }

            return ConversionFailed(value);
        }

        #endregion
    }
}
