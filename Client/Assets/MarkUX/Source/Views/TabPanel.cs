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
    /// Displays content in a tab-panel.
    /// </summary>
    [InternalView]
    public class TabPanel : ContentView
    {
        #region Fields

        public ElementSize TabListHeight;
        public ElementSize TabHeight;
        public Margin TabContentMargin;
        public Alignment TabAlignment;
        public Margin TabListOffset;        

        // tab content
        public Color TabContentBackgroundColor;
        public Sprite TabContentBackgroundImage;
        public UnityEngine.UI.Image.Type TabContentBackgroundImageType;

        // tab item
        public Color TabItemColor;
        public Sprite TabItemImage;
        public UnityEngine.UI.Image.Type TabItemImageType;
        public Color SelectedTabItemColor;
        public Sprite SelectedTabItemImage;
        public Color HighlightedTabItemColor;
        public Sprite HighlightedTabItemImage;
        public ElementSize TabItemsSpacing;

        // tab item header
        public Font TabItemFont;
        public Margin TabItemTextMargin;
        public FontStyle TabItemFontStyle;
        public int TabItemFontSize;
        public Color TabItemFontColor;
        public Alignment TabItemTextAlignment;
        public Color TabItemShadowColor;
        public Vector2 TabItemShadowDistance;
        public Color TabItemOutlineColor;
        public Vector2 TabItemOutlineDistance;

        public List TabList;
        public List<TabItem> TabItems;
        public ViewSwitcher TabSwitcher;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TabPanel()
        {
            TabListHeight = new ElementSize(40, ElementSizeUnit.Pixels);
            TabHeight = new ElementSize(40, ElementSizeUnit.Pixels);
            TabContentMargin = new Margin(0, 40, 0, 0);
            TabAlignment = Alignment.Bottom;
            TabItemTextMargin = new Margin();
            TabListOffset = new Margin();
            TabItemsSpacing = new ElementSize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();
        }

        /// <summary>
        /// Called when the user clicks on a tab.
        /// </summary>
        public void TabClick(ListSelectionActionData eventData)
        {
            // switch tab content
            TabSwitcher.SwitchTo(eventData.ListItem.ZeroBasedIndex);
        }

        /// <summary>
        /// Called when the tab items have changed.
        /// </summary>
        public void TabItemsChanged()
        {
            // go through each flow list item and copy values from tabs
            int childIndex = 0;
            TabList.ListMask.ForEachChild<ListItem>(x =>
            {
                if (!x.Enabled)
                    return; // skip template

                // copy tab values
                var tabGo = TabSwitcher.ContentContainer.transform.GetChild(childIndex);
                if (tabGo == null)
                    return;

                var tab = tabGo.GetComponent<Tab>();
                if (tab.TitleSet)
                {                     
                    x.SetValue(() => x.Text, tab.Title);
                }

                if (tab.FontSet)
                {
                    x.SetValue(() => x.Font, tab.Font);
                }

                if (tab.FontStyleSet)
                {
                    x.SetValue(() => x.FontStyle, tab.FontStyle);
                }

                if (tab.FontSizeSet)
                {
                    x.SetValue(() => x.FontSize, tab.FontSize);
                }

                if (tab.TextAlignmentSet)
                {
                    x.SetValue(() => x.TextAlignment, tab.TextAlignment);
                }

                if (tab.FontColorSet)
                {
                    x.SetValue(() => x.FontColor, tab.FontColor);
                }

                if (tab.ShadowColorSet)
                {
                    x.SetValue(() => x.ShadowColor, tab.ShadowColor);
                }

                if (tab.ShadowDistanceSet)
                {
                    x.SetValue(() => x.ShadowDistance, tab.ShadowDistance);
                }

                if (tab.OutlineColorSet)
                {
                    x.SetValue(() => x.OutlineColor, tab.OutlineColor);
                }

                if (tab.OutlineDistanceSet)
                {
                    x.SetValue(() => x.OutlineDistance, tab.OutlineDistance);
                }

                if (tab.TabLengthSet)
                {
                    x.SetValue(() => x.Width, tab.TabLength);
                }

                ++childIndex;                
            }, false);
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();                        

            // generate tabs based on tab content and orientation
            TabItems = new List<TabItem>();
            TabSwitcher.ContentContainer.ForEachChild<Tab>(x =>
            {
                TabItems.Add(new TabItem { Title = x.Title });
            }, false);            
        }

        /// <summary>
        /// Gets embedded XML of the view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"
                <TabPanel> 
                  <Region Id=""TabContent"" Margin=""{TabContentMargin}"" BackgroundColor=""{TabContentBackgroundColor}"" 
                          BackgroundImage=""{TabContentBackgroundImage}"" BackgroundImageType=""{TabContentBackgroundImageType}"">
                    <ViewSwitcher Id=""TabSwitcher"">
                      <ContentContainer IncludeContainerInContent=""False"" />
                    </ViewSwitcher>
                  </Region>

                  <List Id=""TabList"" Offset=""{TabListOffset}"" Orientation=""Horizontal"" Items=""{TabItems}"" Width=""100%"" Height=""{TabListHeight}"" Alignment=""Top"" SelectionChanged=""TabClick"" SelectedIndex=""0""
                      ItemsChanged=""TabItemsChanged"" ContentAlignment=""{TabAlignment}"" Spacing=""{TabItemsSpacing}""
                      ItemColor=""{TabItemColor}"" ItemImage=""{TabItemImage}"" ItemImageType=""{TabItemImageType}"" SelectedItemColor=""{SelectedTabItemColor}""
                      SelectedItemImage=""{SelectedTabItemImage}"" HighlightedItemColor=""{HighlightedTabItemColor}"" HighlightedItemImage=""{HighlightedTabItemImage}""
                      ItemFont=""{TabItemFont}""
                      ItemTextMargin=""{TabItemTextMargin}""
                      ItemFontStyle=""{TabItemFontStyle}""
                      ItemFontSize=""{TabItemFontSize}""
                      ItemFontColor=""{TabItemFontColor}""
                      ItemTextAlignment=""{TabItemTextAlignment}""
                      ItemShadowColor=""{TabItemShadowColor}""
                      ItemShadowDistance=""{TabItemShadowDistance}""
                      ItemOutlineColor=""{TabItemOutlineColor}""
                      ItemOutlineDistance=""{TabItemOutlineDistance}"">

                    <ListItem Id=""Template"" Width=""100"" Height=""{TabHeight}"" />
                  </List>
                </TabPanel>";
        }

        #endregion
    }

    /// <summary>
    /// Represents a tab in the tab-panel.
    /// </summary>    
    public class TabItem
    {
        public string Title; 
    }
}
