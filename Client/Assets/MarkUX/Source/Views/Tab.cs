#region Using Statements
using MarkUX.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Tab view that resides in a TabPanel.
    /// </summary>
    [InternalView]
    public class Tab : ContentView
    {
        #region Fields

        public ElementSize TabLength;
        public bool TabLengthSet;

        public string Title;
        public bool TitleSet;
        public Font Font;
        public bool FontSet;
        public FontStyle FontStyle;
        public bool FontStyleSet;
        public int FontSize;
        public bool FontSizeSet;
        public Alignment TextAlignment;
        public bool TextAlignmentSet;
        public Color FontColor;
        public bool FontColorSet;
        public Color ShadowColor;
        public bool ShadowColorSet;
        public Vector2 ShadowDistance;
        public bool ShadowDistanceSet;
        public Color OutlineColor;
        public bool OutlineColorSet;
        public Vector2 OutlineDistance;
        public bool OutlineDistanceSet;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tab()
        {
            TabLength = new ElementSize(100, ElementSizeUnit.Pixels);
            Title = "Tab";
            FontStyle = UnityEngine.FontStyle.Normal;
            FontSize = 18;
            TextAlignment = MarkUX.Alignment.Center;
            Width = new ElementSize(1, ElementSizeUnit.Percents);
            Height = new ElementSize(1, ElementSizeUnit.Percents);
            FontColor = Color.black;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Tab />";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets text anchor.
        /// </summary>
        public TextAnchor TextAnchor
        {
            get
            {
                switch (TextAlignment)
                {
                    case Alignment.TopLeft:
                        return TextAnchor.UpperLeft;
                    case Alignment.Top:
                        return TextAnchor.UpperCenter;
                    case Alignment.TopRight:
                        return TextAnchor.UpperRight;
                    case Alignment.Left:
                        return TextAnchor.MiddleLeft;
                    case Alignment.Right:
                        return TextAnchor.MiddleRight;
                    case Alignment.BottomLeft:
                        return TextAnchor.LowerLeft;
                    case Alignment.Bottom:
                        return TextAnchor.LowerCenter;
                    case Alignment.BottomRight:
                        return TextAnchor.LowerRight;
                    case Alignment.Center:
                    default:
                        return TextAnchor.MiddleCenter;
                }
            }
        }

        #endregion
    }
}
