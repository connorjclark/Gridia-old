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
    /// Indicates that the field is an asset.
    /// </summary>
    public class AssetValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AssetValueConverter()
        {
            _type = typeof(AssetValueConverter);
        }

        #endregion
    }
}
