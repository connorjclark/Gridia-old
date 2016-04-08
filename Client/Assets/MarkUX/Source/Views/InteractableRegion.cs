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
    /// Interactable region view.
    /// </summary>
    [InternalView]
    [RemoveComponent(typeof(Image))]
    public class InteractableRegion : ContentView
    {
        #region Fields

        public ViewAction Click;
        public ViewAction BeginDrag;
        public ViewAction EndDrag;
        public ViewAction Drag;
        public ViewAction InitializePotentialDrag;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public InteractableRegion()
        {
            AlwaysBlockRaycast = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<InteractableRegion AlwaysBlockRaycast=""True"" />";
        }

        #endregion
    }
}
