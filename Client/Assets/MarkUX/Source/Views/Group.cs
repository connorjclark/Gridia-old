#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Groups views vertically or horizontally. Resizes itself to its content.
    /// </summary>
    [InternalView]
    public class Group : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateLayouts")]
        public Orientation Orientation;

        [ChangeHandler("UpdateLayouts")]
        public ElementSize Spacing;

        [ChangeHandler("UpdateLayout")]
        public Alignment ContentAlignment;
        public bool ContentAlignmentSet;

        [ChangeHandler("UpdateLayout")]
        public SortDirection SortDirection;

        protected View GroupContentContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Group()
        {
            Spacing = new ElementSize();
            Orientation = Orientation.Vertical;
            SortDirection = SortDirection.Ascending;
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
            float maxWidth = 0f;
            float maxHeight = 0f;
            float totalWidth = 0f;
            float totalHeight = 0f;
            bool percentageWidth = false;
            bool percentageHeight = false;

            bool isHorizontal = Orientation == Orientation.Horizontal;

            var children = new List<View>();
            var childrenToBeSorted = new List<View>();
            GroupContentContainer.ForEachChild<View>(x =>
            {
                // should this be sorted?
                if (x.SortIndex != 0)
                {
                    // yes. 
                    childrenToBeSorted.Add(x);
                    return;
                }

                children.Add(x);
            }, false);

            if (SortDirection == SortDirection.Ascending)
            {
                children.AddRange(childrenToBeSorted.OrderBy(x => x.SortIndex));
            }
            else
            {
                children.AddRange(childrenToBeSorted.OrderByDescending(x => x.SortIndex));
            }

            // get size of content and set content offsets and alignment
            int childCount = children.Count;
            int childIndex = 0;
            for (int i = 0; i < childCount; ++i)
            {
                var view = children[i];

                // don't group disabled views
                if (view.Enabled == false)
                    continue;

                if (view.Width.Unit == ElementSizeUnit.Percents)
                {
                    if (isHorizontal)
                    {
                        Debug.LogWarning(String.Format("[MarkUX.349] Unable to group view \"{0}\" horizontally as it doesn't specify its width in pixels or elements.", view.Name));
                        continue;
                    }
                    else
                    {
                        percentageWidth = true;
                    }
                }

                if (view.Height.Unit == ElementSizeUnit.Percents)
                {
                    if (!isHorizontal)
                    {
                        Debug.LogWarning(String.Format("[MarkUX.350] Unable to group view \"{0}\" vertically as it doesn't specify its height in pixels or elements.", view.Name));
                        continue;
                    }
                    else
                    {
                        percentageHeight = true;
                    }
                }

                // set offsets and alignment
                var offset = new Margin(
                    new ElementSize(isHorizontal ? totalWidth + Spacing.Elements * childIndex : 0f, ElementSizeUnit.Elements),
                    new ElementSize(!isHorizontal ? totalHeight + Spacing.Elements * childIndex : 0f, ElementSizeUnit.Elements));
                view.OffsetFromParent = offset;

                // set desired alignment if it is valid for the orientation otherwise use defaults
                var alignment = isHorizontal ? Alignment.Left : Alignment.Top;
                var desiredAlignment = ContentAlignmentSet ? ContentAlignment : view.Alignment;
                if (isHorizontal && (desiredAlignment == Alignment.Top || desiredAlignment == Alignment.Bottom
                    || desiredAlignment == Alignment.TopLeft || desiredAlignment == Alignment.BottomLeft))
                {
                    view.Alignment = alignment | desiredAlignment;
                }
                else if (!isHorizontal && (desiredAlignment == Alignment.Left || desiredAlignment == Alignment.Right
                    || desiredAlignment == Alignment.TopLeft || desiredAlignment == Alignment.TopRight))
                {
                    view.Alignment = alignment | desiredAlignment;
                }
                else
                {
                    view.Alignment = alignment;
                }

                // get size of content
                if (!percentageWidth)
                {
                    totalWidth += view.Width.Elements;
                    maxWidth = view.Width.Elements > maxWidth ? view.Width.Elements : maxWidth;
                }

                if (!percentageHeight)
                {
                    totalHeight += view.Height.Elements;
                    maxHeight = view.Height.Elements > maxHeight ? view.Height.Elements : maxHeight;
                }

                // update child layout
                view.UpdateLayout();
                ++childIndex;
            }

            // set width and height 
            float totalSpacing = childCount > 1 ? (childIndex - 1) * Spacing.Elements : 0f;

            if (!WidthSet)
            {
                // if width is not explicitly set then adjust to content
                if (!percentageWidth)
                {
                    // add margins
                    totalWidth += isHorizontal ? totalSpacing : 0f;
                    totalWidth += Margin.Left.Elements + Margin.Right.Elements;
                    maxWidth += Margin.Left.Elements + Margin.Right.Elements;

                    // adjust width to content
                    Width = new ElementSize(isHorizontal ? totalWidth : maxWidth, ElementSizeUnit.Elements);
                }
                else
                {
                    Width = new ElementSize(1, ElementSizeUnit.Percents);
                }
            }

            if (!HeightSet)
            {
                // if height is not explicitly set then adjust to content
                if (!percentageHeight)
                {
                    // add margins
                    totalHeight += !isHorizontal ? totalSpacing : 0f;
                    totalHeight += Margin.Top.Elements + Margin.Bottom.Elements;
                    maxHeight += Margin.Top.Elements + Margin.Bottom.Elements;

                    // adjust height to content
                    Height = new ElementSize(!isHorizontal ? totalHeight : maxHeight, ElementSizeUnit.Elements);
                }
                else
                {
                    Height = new ElementSize(1, ElementSizeUnit.Percents);
                }
            }

            base.UpdateLayout();
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            GroupContentContainer = this;
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return "<Group Spacing=\"0\" Orientation=\"Vertical\" />";
        }

        #endregion
    }
}
