#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Content container view.
    /// </summary>
    [InternalView]
    public class ContentContainer : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateLayouts")]
        public bool ResizeToContent;
        public bool IncludeContainerInContent;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ContentContainer()
        {
            ResizeToContent = false;
            IncludeContainerInContent = true;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Called when a child layout has been updated.
        /// </summary>
        public void ChildLayoutUpdatedHasChanged()
        {
            UpdateLayout();
        }

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            if (ResizeToContent)
            {
                float maxWidth = 0f;
                float maxHeight = 0f;
                int childCount = transform.childCount;

                // get size of content and set content offsets and alignment
                for (int i = 0; i < childCount; ++i)
                {
                    var go = transform.GetChild(i);
                    var view = go.GetComponent<View>();

                    // get size of content
                    if (view.Width.Unit != ElementSizeUnit.Percents)
                    {
                        maxWidth = view.Width.Elements > maxWidth ? view.Width.Elements : maxWidth;
                    }

                    if (view.Height.Unit != ElementSizeUnit.Percents)
                    {
                        maxHeight = view.Height.Elements > maxHeight ? view.Height.Elements : maxHeight;
                    }
                }

                // add margins
                maxWidth += Margin.Left.Elements + Margin.Right.Elements;
                maxHeight += Margin.Top.Elements + Margin.Bottom.Elements;

                // adjust size to content
                Width = new ElementSize(maxWidth, ElementSizeUnit.Elements);
                Height = new ElementSize(maxHeight, ElementSizeUnit.Elements);
            }

            base.UpdateLayout();
        }


        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<ContentContainer ResizeToContent=""False"" IncludeContainerInContent=""True"" />";
        }

        #endregion
    }
}
