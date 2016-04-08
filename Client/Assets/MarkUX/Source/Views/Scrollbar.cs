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
    /// Scrollbar view.
    /// </summary>
    [InternalView]
    [AddComponent(typeof(UnityEngine.UI.Scrollbar))]
    public class Scrollbar : View
    {
        #region Fields

        public Sprite SlidingAreaImage;
        public Sprite HandleImage;

        public View SlidingArea;
        public View Handle;

        [ChangeHandler("UpdateLayout")]
        public ElementSize Breadth;

        [ChangeHandler("UpdateLayout")]
        public Orientation Orientation;

        [ChangeHandler("UpdateBehavior")]
        public Sprite ScrollBarHandleImage;

        [ChangeHandler("UpdateBehavior")]
        public UnityEngine.UI.Image.Type ScrollBarHandleImageType;

        [ChangeHandler("UpdateBehavior")]
        public Color ScrollBarHandleColor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Scrollbar()
        {
            Breadth = new ElementSize(20, ElementSizeUnit.Pixels);
            Orientation = Orientation.Horizontal;
            ScrollBarHandleImageType = UnityEngine.UI.Image.Type.Simple;
            ScrollBarHandleColor = Color.white;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            var scrollbar = GetComponent<UnityEngine.UI.Scrollbar>();

            // adjust scrollbar to orientation 
            if (Orientation == Orientation.Horizontal)
            {
                Width = new ElementSize(1, ElementSizeUnit.Percents);
                Height = new ElementSize(Breadth.Pixels, ElementSizeUnit.Pixels);
                Alignment = Alignment.Bottom;

                scrollbar.direction = UnityEngine.UI.Scrollbar.Direction.LeftToRight;
            }
            else
            {
                Width = new ElementSize(Breadth.Pixels, ElementSizeUnit.Pixels);
                Height = new ElementSize(1, ElementSizeUnit.Percents);
                Alignment = Alignment.Right;

                scrollbar.direction = UnityEngine.UI.Scrollbar.Direction.BottomToTop;
            }
                                    
            base.UpdateLayout();
        }

        /// <summary>
        /// Updates behavior of view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();

            // set scrollbar component values
            var scrollbar = GetComponent<UnityEngine.UI.Scrollbar>();
            scrollbar.targetGraphic = Handle.GetComponent<UnityEngine.UI.Image>();
            scrollbar.handleRect = Handle.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Scrollbar Breadth=""20"" ScrollBarHandleColor=""White"">
                        <Region Id=""SlidingArea"">
                            <Image Id=""Handle"" BackgroundImage=""{ScrollBarHandleImage}"" BackgroundImageType=""{ScrollBarHandleImageType}"" BackgroundColor=""{ScrollBarHandleColor}"" UpdateRectTransform=""False"" />
                        </Region>
                    </Scrollbar>";
        }

        #endregion

        #region Properties

        #endregion
    }
}
