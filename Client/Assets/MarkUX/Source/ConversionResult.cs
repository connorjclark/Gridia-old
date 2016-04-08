#region Using Statements
using System;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Contains the result of a value conversion.
    /// </summary>
    public class ConversionResult
    {
        #region Fields
                
        private object _convertedObject;
        private bool _success;
        private string _errorMessage;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ConversionResult()
        {
            _success = true;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ConversionResult(object convertedObject)
        {
            _convertedObject = convertedObject;
            _success = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets converted object.
        /// </summary>
        public object ConvertedObject
        {
            get 
            {
                return _convertedObject;
            }
            set 
            {
                _convertedObject = value;
            }
        }

        /// <summary>
        /// Gets or sets boolean indicating if conversion was successful.
        /// </summary>
        public bool Success
        {
            get 
            {
                return _success;
            }
            set 
            {
                _success = value;
            }
        }

        /// <summary>
        /// Gets or sets string containing eventual error message if conversion was unsuccessful.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        #endregion
    }
}
