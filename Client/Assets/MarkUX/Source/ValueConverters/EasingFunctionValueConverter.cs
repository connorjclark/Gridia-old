#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using MarkUX.Animation;
#endregion

namespace MarkUX.ValueConverters
{
    /// <summary>
    /// Value converter for EasingFunction type.
    /// </summary>
    public class EasingFunctionValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public EasingFunctionValueConverter()
        {
            _type = typeof(EasingFunctionType);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for EasingFunction type.
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
                    var convertedValue = Enum.Parse(typeof(EasingFunctionType), stringValue, true);
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
