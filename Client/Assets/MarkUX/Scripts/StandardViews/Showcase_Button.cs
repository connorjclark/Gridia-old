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
    /// View that showcases the Button.
    /// </summary>
    [InternalView]
    public class Showcase_Button : View
    {
        #region Fields
                
        public int ButtonClickCount = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Called when the user clicks on the button.
        /// </summary>
        public void ButtonClick()
        {
            SetValue(() => ButtonClickCount, ButtonClickCount + 1);
        }

        #endregion
    }
}
