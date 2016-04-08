#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Contains information about the context in which a value conversion occurs.
    /// </summary>
    public class ValueConverterContext
    {
        #region Fields

        private string _baseDirectory;
        public static ValueConverterContext Empty = new ValueConverterContext();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ValueConverterContext()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets base directory.
        /// </summary>
        public string BaseDirectory
        {
            get 
            {
                return _baseDirectory;
            }
            set
            {
                _baseDirectory = value;
            }
        }

        #endregion
    }
}
