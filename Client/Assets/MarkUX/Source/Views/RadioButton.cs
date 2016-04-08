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
    /// Radio button view.
    /// </summary>
    [InternalView]
    public class RadioButton : View
    {
        #region Fields

        public ViewAction Click;

        [ChangeHandler("UpdateBehavior")]
        public bool Checked;
        public ElementSize Spacing;

        [NotSetFromXml]
        public Group RadioButtonGroup;

        [NotSetFromXml]
        public Image RadioButtonImageView;

        // radio button image        
        public ElementSize RadioButtonWidth;
        public ElementSize RadioButtonHeight;
        public Margin RadioButtonOffset;
        public Color RadioButtonColor;
        public Sprite RadioButtonImage;
        public UnityEngine.UI.Image.Type RadioButtonImageType;
        public Color RadioButtonPressedColor;
        public Sprite RadioButtonPressedImage;

        // radio button label
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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public RadioButton()
        {
            Spacing = new ElementSize();
            Height = new ElementSize(40, ElementSizeUnit.Pixels);
            RadioButtonWidth = new ElementSize(40, ElementSizeUnit.Pixels);
            RadioButtonHeight = new ElementSize(40, ElementSizeUnit.Pixels);
            RadioButtonOffset = new Margin();
            TextMargin = new Margin();
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
            // adjust width to RadioButtonGroup
            Width = new ElementSize(RadioButtonGroup.ActualWidth, ElementSizeUnit.Pixels);
            base.UpdateLayout();
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            // handle disabled and toggle states
            if (Checked)
            {
                RadioButtonImageView.SetValue(() => RadioButtonImageView.Path, RadioButtonPressedImage);
                RadioButtonImageView.SetValue(() => RadioButtonImageView.Color, RadioButtonPressedColor);
            }
            else
            {
                RadioButtonImageView.SetValue(() => RadioButtonImageView.Path, RadioButtonImage);
                RadioButtonImageView.SetValue(() => RadioButtonImageView.Color, RadioButtonColor);
            }

            base.UpdateBehavior();
        }

        /// <summary>
        /// Called when check-box is clicked.
        /// </summary>
        public void RadioButtonClick()
        {
            if (!Checked)
            {
                // deselect all radio buttons
                if (LayoutParentView != null)
                {
                    LayoutParentView.ForEachChild<RadioButton>(x =>
                    {
                        if (x.Checked)
                        {
                            x.SetValue(() => x.Checked, false);
                        }
                    }, false);
                }

                SetValue(() => Checked, true);
            }            
        }

        /// <summary>
        /// Gets embedded XML of the view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<RadioButton Click=""RadioButtonClick"">
                    <Group Id=""RadioButtonGroup"" Orientation=""Horizontal"" Spacing=""{Spacing}"">
                        <Image Id=""RadioButtonImageView"" Width=""{RadioButtonWidth}"" Height=""{RadioButtonHeight}""
                               Offset=""{RadioButtonOffset}"" BackgroundImageType=""{RadioButtonImageType}"" />
                        <Label Text=""{Text}"" Font=""{Font}"" FontStyle=""{FontStyle}"" FontSize=""{FontSize}"" FontColor=""{FontColor}"" TextAlignment=""{TextAlignment}"" AdjustToText=""Width"" Height=""100%"" ShadowColor=""{ShadowColor}"" ShadowDistance=""{ShadowDistance}"" OutlineColor=""{OutlineColor}"" OutlineDistance=""{OutlineDistance}"" />
                    </Group>
                </RadioButton>";
        }

        #endregion
    }
}
