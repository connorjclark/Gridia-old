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
    /// View that showcases the List.
    /// </summary>
    [InternalView]
    public class Showcase_List : View
    {
        #region Fields

        public Label ListItemClickLabel;
        public Label StaticListItemClickLabel;
        public List<Showcase_ListItem> ListItems;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            System.Random random = new System.Random();
            ListItems = new List<Showcase_ListItem>();
            for (int i = 0; i < 9; ++i)
            {
                ListItems.Add(new Showcase_ListItem
                {
                    Width = new ElementSize(random.Next(200, 350), ElementSizeUnit.Pixels),
                    Height = new ElementSize(40, ElementSizeUnit.Pixels),
                });
            }
        }

        /// <summary>
        /// Called when user clicks on flow-list item.
        /// </summary>
        public void ListItemClicked(ListSelectionActionData eventData)
        {
            ListItemClickLabel.SetValue(() => ListItemClickLabel.Text, String.Format("Item {0} selected", eventData.ListItem.Index));
        }

        /// <summary>
        /// Called when user clicks on static flow-list item.
        /// </summary>
        public void StaticListItemClicked(ListSelectionActionData eventData)
        {
            StaticListItemClickLabel.SetValue(() => StaticListItemClickLabel.Text, String.Format("Item {0} selected", eventData.ListItem.Index));
        }

        #endregion
    }

    /// <summary>
    /// Example data for showcasing flow-list.
    /// </summary>
    public class Showcase_ListItem
    {
        public ElementSize Width;
        public ElementSize Height;
    }
}
