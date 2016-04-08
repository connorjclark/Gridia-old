#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Globalization;
#endregion

namespace MarkUX.ValueConverters
{
    /// <summary>
    /// Value converter for int type.
    /// </summary>
    public class IntValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public IntValueConverter()
        {
            _type = typeof(int);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for int type.
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
                    var convertedValue = System.Convert.ToInt32(stringValue, CultureInfo.InvariantCulture);
                    return new ConversionResult(convertedValue);
                }
                catch (Exception e)
                {
                    return ConversionFailed(value, e);
                }
            }
            else
            {
                // attempt to convert using system type converter
                try
                {
                    var convertedValue = System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
                    return new ConversionResult(convertedValue);
                }
                catch (Exception e)
                {
                    return ConversionFailed(value, e);
                }
            }
        }

        #endregion
    }
}
