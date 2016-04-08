#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MarkUX.Animation;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Mask view.
    /// </summary>
    [InternalView]
    [AddComponent(typeof(UnityEngine.UI.Mask))]
    public class Mask : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateBehaviour")]
        public bool ShowMask;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Mask()
        {
            ShowMask = true;
        }

        #endregion
     
        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            var maskComponent = GetComponent<UnityEngine.UI.Mask>();
            if (maskComponent == null)
                return;

            maskComponent.showMaskGraphic = ShowMask;
            maskComponent.enabled = BackgroundColor.a > 0; // enable mask if alpha > 0

            base.UpdateBehavior();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Mask BackgroundColor=""#03ffffff"" ShowMask=""True""/>";
        }

        #endregion
    }
}
