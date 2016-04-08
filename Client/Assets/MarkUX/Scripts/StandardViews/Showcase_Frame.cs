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
    /// View that showcases the FlowList.
    /// </summary>
    [InternalView]
    public class Showcase_Frame : View
    {
        #region Fields

        public ViewAnimation ResizeRegionAnimation;
        private bool _isMaximized = false;

        #endregion

        #region Methods

        /// <summary>
        /// Called when the user clicks on "resize"-button.
        /// </summary>
        public void ResizeRegion()
        {
            if (!_isMaximized)
            {
                ResizeRegionAnimation.StartAnimation();
            }
            else
            {
                ResizeRegionAnimation.ReverseAnimation();
            }
            _isMaximized = !_isMaximized;
        }

        #endregion
    }
}
