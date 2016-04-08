#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MarkUX.Animation;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Button view.
    /// </summary>
    [InternalView]
    public class Button : Frame
    {
        #region Fields

        public ViewAction Click;
        public ViewAction MouseEnter;
        public ViewAction MouseExit;
        public ViewAction MouseDown;
        public ViewAction MouseUp;

        [ChangeHandler("UpdateBehavior")]
        public bool Disabled;
        public bool DisabledSet;

        [ChangeHandler("UpdateBehavior")]
        public bool IsToggleButton;

        [ChangeHandler("UpdateBehavior")]
        public bool ToggleValue;

        public bool CanToggleOn;
        public bool CanToggleOff;

        // button label
        public string Text;
        public Margin TextMargin;
        public Font Font;
        public FontStyle FontStyle;
        public int FontSize;
        public Color FontColor;
        public Alignment TextAlignment;
        public Color ShadowColor;
        public Vector2 ShadowDistance;
        public Color OutlineColor;
        public Vector2 OutlineDistance;

        // animation
        // color tint animation
        [ChangeHandler("UpdateBehavior")]
        public Color HighlightedColor;
        public bool HighlightedColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Color PressedColor;
        public bool PressedColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Color DisabledColor;
        public bool DisabledColorSet;

        // sprite swap animation
        [ChangeHandler("UpdateBehavior")]
        public Sprite HighlightedImage;
        public bool HighlightedImageSet;

        [ChangeHandler("UpdateBehavior")]
        public Sprite PressedImage;
        public bool PressedImageSet;

        [ChangeHandler("UpdateBehavior")]
        public Sprite DisabledImage;
        public bool DisabledImageSet;

        public ViewAnimation HighlightImageAnimation;
        public ViewAnimation PressedImageAnimation;
        public ViewAnimation HighlightColorAnimation;
        public ViewAnimation PressedColorAnimation;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Button()
        {
            HighlightedImage = null;
            PressedImage = null;
            DisabledImage = null;
            ResizeToContent = false;
            Text = String.Empty;
            Disabled = false;
            DisabledSet = false;        
            IsToggleButton = false;
            ToggleValue = false;
            CanToggleOn = true;
            CanToggleOff = true;
        
            // button label
            TextMargin = new Margin();
            FontStyle = UnityEngine.FontStyle.Normal;
            FontSize = 18;
            FontColor = Color.black;
            TextAlignment = MarkUX.Alignment.Center;
            ShadowColor = Color.clear;
            ShadowDistance = Vector2.zero;
            OutlineColor = Color.clear;
            OutlineDistance = Vector2.zero;

            // animation
            // color tint animation
            HighlightedColor = Color.clear;
            HighlightedColorSet = false;
            PressedColor = Color.clear;
            PressedColorSet = false;
            DisabledColor = Color.clear;
            DisabledColorSet = false;
        }

        #endregion
     
        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            // init animation values
            if (HighlightedImageSet)
            {
                SetChanged(() => HighlightedImage);
            }

            if (PressedImageSet)
            {
                SetChanged(() => PressedImage);
            }

            if (HighlightedColorSet)
            {
                SetChanged(() => HighlightedColor);
            }

            if (PressedColorSet)
            {
                SetChanged(() => PressedColor);
            }

            // handle disabled and toggle states
            UpdateBackground = !(DisabledSet || IsToggleButton);
            if (DisabledSet)
            {
                if (Disabled)
                {
                    SetBackground(DisabledImage, null, DisabledColor, DisabledColorSet);
                }
                else
                {
                    SetBackground();
                }
            }
            else if (IsToggleButton)
            {
                if (ToggleValue)
                {
                    SetBackground(PressedImage, null, PressedColor, PressedColorSet);
                }
                else
                {
                    SetBackground();
                }
            }

            base.UpdateBehavior();
        }

        /// <summary>
        /// Called when mouse is clicked.
        /// </summary>
        public void ButtonMouseClick()        
        {
            // if toggle-button change state
            if (IsToggleButton)
            {
                if (ToggleValue == true && !CanToggleOff)
                    return;
                if (ToggleValue == false && !CanToggleOn)
                    return;

                SetValue(() => ToggleValue, !ToggleValue);
            }
        }

        /// <summary>
        /// Called when mouse enters.
        /// </summary>
        public void ButtonMouseEnter()
        {
            if (!Disabled)
            {
                if (HighlightedImageSet)
                {
                    HighlightImageAnimation.StartAnimation();
                }

                if (HighlightedColorSet)
                {
                    HighlightColorAnimation.StartAnimation();
                }
            }
        }

        /// <summary>
        /// Called when mouse exits.
        /// </summary>
        public void ButtonMouseExit()
        {
            if (!Disabled)
            {
                if (HighlightedImageSet)
                {
                    HighlightImageAnimation.ReverseAnimation();
                }

                if (HighlightedColorSet)
                {
                    HighlightColorAnimation.ReverseAnimation();
                }
            }
        }

        /// <summary>
        /// Called when mouse down.
        /// </summary>
        public void ButtonMouseDown()
        {
            if (!Disabled)
            {
                if (PressedImageSet)
                {
                    PressedImageAnimation.StartAnimation();
                }

                if (PressedColorSet)
                {
                    PressedColorAnimation.StartAnimation();
                }
            }
        }

        /// <summary>
        /// Called when mouse up.
        /// </summary>
        public void ButtonMouseUp()
        {
            if (!Disabled)
            {
                if (PressedImageSet)
                {
                    PressedImageAnimation.ReverseAnimation();
                }

                if (PressedColorSet)
                {
                    PressedColorAnimation.ReverseAnimation();
                }
            }
        }

        /// <summary>
        /// Gets embedded XML of the view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<Button Width=""4em"" Height=""1em"" Click=""ButtonMouseClick"" MouseEnter=""ButtonMouseEnter"" MouseExit=""ButtonMouseExit"" MouseDown=""ButtonMouseDown"" MouseUp=""ButtonMouseUp"">
                    <Label Text=""{Text}"" Margin=""{TextMargin}"" Font=""{Font}"" FontStyle=""{FontStyle}"" FontSize=""{FontSize}"" FontColor=""{FontColor}"" TextAlignment=""{TextAlignment}"" Width=""100%"" Height=""100%"" ShadowColor=""{ShadowColor}"" ShadowDistance=""{ShadowDistance}"" OutlineColor=""{OutlineColor}"" OutlineDistance=""{OutlineDistance}"" />
                    <ContentContainer />
                    <ViewAnimation Id=""HighlightImageAnimation"">
                        <Animate Field=""BackgroundImage"" To=""{HighlightedImage}"" />
                    </ViewAnimation>
                    <ViewAnimation Id=""PressedImageAnimation"">
                        <Animate Field=""BackgroundImage"" To=""{PressedImage}"" />
                    </ViewAnimation>
                    <ViewAnimation Id=""HighlightColorAnimation"">
                        <Animate Field=""BackgroundColor"" To=""{HighlightedColor}"" ReverseSpeed=""0.5"" Duration=""0.05"" />
                    </ViewAnimation>
                    <ViewAnimation Id=""PressedColorAnimation"">
                        <Animate Field=""BackgroundColor"" To=""{PressedColor}"" />
                    </ViewAnimation>
                </Button>";
        }

        #endregion
    }
}
