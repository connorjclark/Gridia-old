#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Globalization;
using System.Collections;
#endregion

namespace MarkUX.ValueConverters
{
    /// <summary>
    /// Value converter for generic list of objects.
    /// </summary>
    public class GenericListValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GenericListValueConverter()
        {
            _type = typeof(List<object>);
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
                return new ConversionResult(new List<object>());
            }

            try
            {
                var list = new List<object>();
                if (value is IEnumerable)
                {                    
                    foreach (var e in (value as IEnumerable))
                    {
                        list.Add(e);
                    }

                    return new ConversionResult(list);
                }
                else
                {
                    list.Add(value);
                    return new ConversionResult(list);
                }
            }
            catch (Exception e)
            {
                return ConversionFailed(value, e);
            }
        }

        #endregion
    }
}
