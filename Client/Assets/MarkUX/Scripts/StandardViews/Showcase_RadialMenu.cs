#region Using Statements
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.UnityProject
{
    /// <summary>
    /// View that showcases the radial menu.
    /// </summary>
    [InternalView]
    public class Showcase_RadialMenu : View
    {
        #region Fields

        public RadialMenu ContextRadialMenu;

        #endregion

        #region Methods

        /// <summary>
        /// Called when the user clicks on the interactable surface.
        /// </summary>
        public void ToggleRadialMenu(PointerEventData eventData)
        {
            ContextRadialMenu.ToggleAt(eventData.position);
        }

        #endregion
    }
}
