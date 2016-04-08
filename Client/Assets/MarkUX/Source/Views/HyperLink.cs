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
    /// Hyper link view for showing clickable text.
    /// </summary>
    [InternalView]
    [RemoveComponent(typeof(UnityEngine.UI.Image))]
    [AddComponent(typeof(Text))]
    public class HyperLink : View
    {
        #region Fields

        public ViewAction Click;
        public ViewAction MouseEnter;
        public ViewAction MouseExit;
        public ViewAction MouseDown;
        public ViewAction MouseUp;

        [ChangeHandler("UpdateLayouts")]
        public string Text;

        [ChangeHandler("UpdateLayout")]
        public AdjustToText AdjustToText;

        [ChangeHandler("UpdateBehavior")]
        public Font Font;

        [ChangeHandler("UpdateBehavior")]
        public FontStyle FontStyle;

        [ChangeHandler("UpdateBehavior")]
        public int FontSize;

        [ChangeHandler("UpdateBehavior")]
        public float LineSpacing;

        [ChangeHandler("UpdateBehavior")]
        public Alignment TextAlignment;

        [ChangeHandler("UpdateBehavior")]
        public Color FontColor;

        [ChangeHandler("UpdateBehavior")]
        public Color ShadowColor;
        public bool ShadowColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Vector2 ShadowDistance;
        public bool ShadowDistanceSet;

        [ChangeHandler("UpdateBehavior")]
        public Color OutlineColor;
        public bool OutlineColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Vector2 OutlineDistance;
        public bool OutlineDistanceSet;

        [ChangeHandler("UpdateBehavior")]
        public bool Disabled;
        public bool DisabledSet;

        // animation
        // color tint animation
        [ChangeHandler("UpdateBehavior")]
        public Color HighlightedFontColor;

        [ChangeHandler("UpdateBehavior")]
        public Color PressedFontColor;

        [ChangeHandler("UpdateBehavior")]
        public Color DisabledFontColor;

        public ViewAnimation HighlightFontColorAnimation;
        public ViewAnimation PressedFontColorAnimation;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public HyperLink()
        {
            Text = String.Empty;
            AdjustToText = MarkUX.AdjustToText.None;
            FontStyle = UnityEngine.FontStyle.Normal;
            FontSize = 18;
            LineSpacing = 1;
            TextAlignment = MarkUX.Alignment.Left;
            Height = new ElementSize(1, ElementSizeUnit.Elements);
            FontColor = ColorValueConverter.ColorCodes["lightblue"];
            Disabled = false;

            // animation
            // color tint animation
            HighlightedFontColor = ColorValueConverter.ColorCodes["lightblue1"];
            PressedFontColor = ColorValueConverter.ColorCodes["lightblue2"];
            DisabledFontColor = Color.black;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            var textComponent = GetComponent<Text>();
            if (textComponent == null)
                return;

            textComponent.text = Text;

            if (AdjustToText == AdjustToText.Width)
            {
                Width = new ElementSize(textComponent.preferredWidth, ElementSizeUnit.Pixels);
            }
            else if (AdjustToText == AdjustToText.Height)
            {
                Height = new ElementSize(textComponent.preferredHeight, ElementSizeUnit.Pixels);
            }
            else if (AdjustToText == AdjustToText.WidthAndHeight)
            {
                Width = new ElementSize(textComponent.preferredWidth, ElementSizeUnit.Pixels);
                Height = new ElementSize(textComponent.preferredHeight, ElementSizeUnit.Pixels);
            }

            base.UpdateLayout();
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();
            var textComponent = GetComponent<Text>();
            if (textComponent == null)
                return;

            if (Font != null)
            {
                textComponent.font = Font;
            }
            else
            {
                textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            }

            textComponent.fontSize = FontSize;
            textComponent.lineSpacing = LineSpacing;
            textComponent.supportRichText = false;
            textComponent.alignment = TextAnchor;
            textComponent.fontStyle = FontStyle;
#if !UNITY_4_6
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
#endif

            if (Disabled)
            {
                textComponent.color = DisabledFontColor;
            }
            else
            {
                textComponent.color = FontColor;
            }

            if (ShadowColorSet || ShadowDistanceSet)
            {
                var shadowComponent = GetComponent<Shadow>();
                if (shadowComponent == null)
                {
                    shadowComponent = gameObject.AddComponent<Shadow>();
                }

                shadowComponent.effectColor = ShadowColor;
                shadowComponent.effectDistance = ShadowDistance;
            }

            if (OutlineColorSet || OutlineDistanceSet)
            {
                var outlineComponent = GetComponent<Outline>();
                if (outlineComponent == null)
                {
                    outlineComponent = gameObject.AddComponent<Outline>();
                }

                outlineComponent.effectColor = OutlineColor;
                outlineComponent.effectDistance = OutlineDistance;
            }
        }

        /// <summary>
        /// Called when hyper-link is clicked.
        /// </summary>
        public void HyperLinkMouseClick()
        {
        }

        /// <summary>
        /// Called when mouse entering hyper-link.
        /// </summary>
        public void HyperLinkMouseEnter()
        {
            if (Disabled)
                return;

            HighlightFontColorAnimation.StartAnimation();
        }

        /// <summary>
        /// Called when mouse exits hyper-link.
        /// </summary>
        public void HyperLinkMouseExit()
        {
            if (Disabled)
                return;
            
            HighlightFontColorAnimation.ReverseAnimation();
        }

        /// <summary>
        /// Called on mouse down.
        /// </summary>
        public void HyperLinkMouseDown()
        {
            if (Disabled)
                return;
            
            PressedFontColorAnimation.StartAnimation();
        }

        /// <summary>
        /// Called on mouse up.
        /// </summary>
        public void HyperLinkMouseUp()
        {
            if (Disabled)
                return;

            PressedFontColorAnimation.ReverseAnimation();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<HyperLink FontStyle=""Normal"" FontSize=""18"" LineSpacing=""1"" FontColor=""LightBlue"" TextAlignment=""Left"" Width=""3em"" Height=""1em""
                                Click=""HyperLinkMouseClick"" MouseEnter=""HyperLinkMouseEnter"" MouseExit=""HyperLinkMouseExit"" MouseDown=""HyperLinkMouseDown"" MouseUp=""HyperLinkMouseUp"">
                        <ViewAnimation Id=""HighlightFontColorAnimation"">
                            <Animate Field=""FontColor"" To=""{HighlightedFontColor}"" ReverseSpeed=""0.5"" Duration=""0.05"" />
                        </ViewAnimation>
                        <ViewAnimation Id=""PressedFontColorAnimation"">
                            <Animate Field=""FontColor"" To=""{PressedFontColor}"" />
                        </ViewAnimation>
                    </HyperLink>";
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
