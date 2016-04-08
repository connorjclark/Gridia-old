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
    /// View that showcases the neat theme.
    /// </summary>
    public class Showcase_StandardViews : View
    {
        #region Fields
                
        public ViewSwitcher ViewSwitcher;            

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Called when the user clicks on a menu item.
        /// </summary>
        public void SectionSelected(FlowListSelectionActionData eventData)
        {
            ViewSwitcher.SwitchTo(eventData.FlowListItem.Text);
        } 

        #endregion
    }
}
