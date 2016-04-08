#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Region view.
    /// </summary>
    [InternalView]
    [RemoveComponent(typeof(Image))]
    public class Region : ContentView
    {
        #region Fields
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Region()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Region />";
        }

        #endregion

        #region Properties
        #endregion
    }
}
