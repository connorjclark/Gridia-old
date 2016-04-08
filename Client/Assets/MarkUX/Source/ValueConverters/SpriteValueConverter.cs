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
    /// Value converter for Sprite type.
    /// </summary>
    public class SpriteValueConverter : AssetValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SpriteValueConverter()
        {
            _type = typeof(Sprite);
        }

        #endregion
    }
}
