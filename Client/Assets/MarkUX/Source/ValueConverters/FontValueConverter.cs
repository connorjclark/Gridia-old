#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
#endregion

namespace MarkUX.ValueConverters
{
    /// <summary>
    /// Value converter for Font type.
    /// </summary>
    public class FontValueConverter : AssetValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FontValueConverter()
        {
            _type = typeof(Font);
        }

        #endregion
    }
}
