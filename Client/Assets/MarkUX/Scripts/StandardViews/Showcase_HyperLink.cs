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
    /// View that showcases the hyper-link.
    /// </summary>
    [InternalView]
    public class Showcase_HyperLink : View
    {
        #region Fields
                
        public int LinkClickCount = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Called when the user clicks on the link.
        /// </summary>
        public void LinkClick()
        {
            SetValue(() => LinkClickCount, LinkClickCount + 1);
        }

        #endregion
    }
}
