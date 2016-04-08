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
    /// View that showcases the ComboBox.
    /// </summary>
    [InternalView]
    public class Showcase_ComboBox : View
    {
        #region Fields

        public List<ComboBox_Item> ComboBoxItems;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            ComboBoxItems = new List<ComboBox_Item>();
            ComboBoxItems.Add(new ComboBox_Item { CraftingMaterial = "Stone" });
            ComboBoxItems.Add(new ComboBox_Item { CraftingMaterial = "Wood" });
            ComboBoxItems.Add(new ComboBox_Item { CraftingMaterial = "Glass" });
            ComboBoxItems.Add(new ComboBox_Item { CraftingMaterial = "Gold" });
            ComboBoxItems.Add(new ComboBox_Item { CraftingMaterial = "Mithril Ore" });
        }

        #endregion
    }

    /// <summary>
    /// Example data for showcasing combo-box.
    /// </summary>
    public class ComboBox_Item
    {
        public string CraftingMaterial;
    }
}
