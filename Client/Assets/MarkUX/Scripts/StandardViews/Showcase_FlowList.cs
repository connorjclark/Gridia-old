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
    public class Showcase_FlowList : View
    {
        #region Fields

        public Label FlowListItemClickLabel;
        public Label StaticFlowListItemClickLabel;
        public List<Showcase_FlowListItem> FlowListItems;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            System.Random random = new System.Random();
            FlowListItems = new List<Showcase_FlowListItem>();
            for (int i = 0; i < 30; ++i)
            {
                FlowListItems.Add(new Showcase_FlowListItem
                {
                    Width = new ElementSize(random.Next(40, 80), ElementSizeUnit.Pixels),
                    Height = new ElementSize(random.Next(40, 80), ElementSizeUnit.Pixels),
                });
            }
        }

        /// <summary>
        /// Called when user clicks on flow-list item.
        /// </summary>
        public void FlowListItemClicked(FlowListSelectionActionData eventData)
        {
            FlowListItemClickLabel.SetValue(() => FlowListItemClickLabel.Text, String.Format("Item {0} selected", eventData.FlowListItem.Index));
        }

        /// <summary>
        /// Called when user clicks on static flow-list item.
        /// </summary>
        public void StaticFlowListItemClicked(FlowListSelectionActionData eventData)
        {
            StaticFlowListItemClickLabel.SetValue(() => StaticFlowListItemClickLabel.Text, String.Format("Item {0} selected", eventData.FlowListItem.Index));
        }

        #endregion
    }

    /// <summary>
    /// Example data for showcasing flow-list.
    /// </summary>
    public class Showcase_FlowListItem
    {
        public ElementSize Width;
        public ElementSize Height;
    }
}
