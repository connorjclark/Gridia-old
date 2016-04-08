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
    /// Container view that provides functionality for scrolling content.
    /// </summary>
    [InternalView]
    public class Panel : ContentView
    {
        #region Fields
                
        public ScrollRect ScrollArea;
        public Scrollbar HorizontalScrollBar;
        public Scrollbar VerticalScrollBar;
        public ElementSize ScrollBarBreadth;
        public bool ScrollVertical;
        public bool ScrollHorizontal;
        public Alignment ContentAlignment;
        public Margin ContentMargin;
        public Sprite HorizontalScrollBarImage;
        public Sprite VerticalScrollBarImage;
        public UnityEngine.UI.Image.Type HorizontalScrollBarImageType;
        public UnityEngine.UI.Image.Type VerticalScrollBarImageType;
        public Sprite HorizontalScrollBarHandleImage;
        public Sprite VerticalScrollBarHandleImage;
        public UnityEngine.UI.Image.Type HorizontalScrollBarHandleImageType;
        public UnityEngine.UI.Image.Type VerticalScrollBarHandleImageType;
        public Color HorizontalScrollBarColor;
        public Color VerticalScrollBarColor;
        public Color HorizontalScrollBarHandleColor;
        public Color VerticalScrollBarHandleColor;

        [ChangeHandler("UpdateBehavior")]
        public bool ShowHorizontalScrollBar;

        [ChangeHandler("UpdateBehavior")]
        public bool ShowVerticalScrollBar;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Panel()
        {
            ShowHorizontalScrollBar = true;
            ShowVerticalScrollBar = true;
            ScrollVertical = true;
            ScrollHorizontal = true;
            ContentMargin = new Margin();
            HorizontalScrollBarColor = Color.white;
            VerticalScrollBarColor = Color.white;
            HorizontalScrollBarHandleColor = Color.white;
            VerticalScrollBarHandleColor = Color.white;
            ContentAlignment = MarkUX.Alignment.Center;
            HorizontalScrollBarImageType = UnityEngine.UI.Image.Type.Simple;
            VerticalScrollBarImageType = UnityEngine.UI.Image.Type.Simple;
            HorizontalScrollBarHandleImageType = UnityEngine.UI.Image.Type.Simple;
            VerticalScrollBarHandleImageType = UnityEngine.UI.Image.Type.Simple;
            ScrollBarBreadth = new ElementSize(20, ElementSizeUnit.Pixels);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            var scrollRect = ScrollArea.GetComponent<UnityEngine.UI.ScrollRect>();

            if (ShowHorizontalScrollBar)
            {
                HorizontalScrollBar.Activate();
                scrollRect.horizontalScrollbar = HorizontalScrollBar.GetComponent<UnityEngine.UI.Scrollbar>();
            }
            else
            {
                HorizontalScrollBar.Deactivate();
                scrollRect.horizontalScrollbar = null;
            } 

            if (ShowVerticalScrollBar)
            {
                VerticalScrollBar.Activate();
                scrollRect.verticalScrollbar = VerticalScrollBar.GetComponent<UnityEngine.UI.Scrollbar>();
            }
            else
            {
                VerticalScrollBar.Deactivate();
                scrollRect.verticalScrollbar = null;
            }

            base.UpdateBehavior();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<Panel ScrollHorizontal=""True"" ScrollVertical=""True"" ContentMargin=""0"">
                    <Region Margin=""{ContentMargin}"">
                        <Mask Margin=""0,0,20,20"">
                            <ScrollRect Id=""ScrollArea"" ScrollVertical=""{ScrollVertical}"" ScrollHorizontal=""{ScrollHorizontal}"">
                                <ContentContainer ResizeToContent=""True"" Alignment=""{ContentAlignment}"" />
                            </ScrollRect>
                        </Mask>    
  
                        <Scrollbar Id=""HorizontalScrollBar"" BackgroundImage=""{HorizontalScrollBarImage}"" BackgroundImageType=""{HorizontalScrollBarImageType}"" BackgroundColor=""{HorizontalScrollBarColor}"" ScrollBarHandleImage=""{HorizontalScrollBarHandleImage}"" ScrollBarHandleImageType=""{HorizontalScrollBarHandleImageType}"" ScrollBarHandleColor=""{HorizontalScrollBarHandleColor}"" Orientation=""Horizontal"" Margin=""0,0,20,0"" Breadth=""{ScrollBarBreadth}"" />
                        <Scrollbar Id=""VerticalScrollBar"" BackgroundImage=""{VerticalScrollBarImage}"" BackgroundImageType=""{VerticalScrollBarImageType}"" BackgroundColor=""{VerticalScrollBarColor}"" ScrollBarHandleImage=""{VerticalScrollBarHandleImage}"" ScrollBarHandleImageType=""{VerticalScrollBarHandleImageType}"" ScrollBarHandleColor=""{VerticalScrollBarHandleColor}"" Orientation=""Vertical"" Margin=""0,0,0,20"" Breadth=""{ScrollBarBreadth}"" />
                    </Region>
                </Panel>";
        }

        #endregion
    }
}
