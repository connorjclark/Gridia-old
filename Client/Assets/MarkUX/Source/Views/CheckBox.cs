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
    /// CheckBox button view.
    /// </summary>
    [InternalView]
    public class CheckBox : View
    {
        #region Fields

        public ViewAction Click;

        [ChangeHandler("UpdateBehavior")]
        public bool Checked;
        public ElementSize Spacing;

        [NotSetFromXml]
        public Group CheckBoxGroup;

        [NotSetFromXml]
        public Image CheckBoxImageView;

        // check-box image        
        public ElementSize CheckBoxWidth;
        public ElementSize CheckBoxHeight;
        public Margin CheckBoxOffset;
        public Color CheckBoxColor;
        public Sprite CheckBoxImage;
        public UnityEngine.UI.Image.Type CheckBoxImageType;
        public Color CheckBoxPressedColor;
        public Sprite CheckBoxPressedImage;

        // check-box label
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
        public CheckBox()
        {
            Spacing = new ElementSize();
            Height = new ElementSize(40, ElementSizeUnit.Pixels);
            CheckBoxWidth = new ElementSize(40, ElementSizeUnit.Pixels);
            CheckBoxHeight = new ElementSize(40, ElementSizeUnit.Pixels);
            CheckBoxOffset = new Margin();
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
            // adjust width to CheckBoxGroup
            Width = new ElementSize(CheckBoxGroup.ActualWidth, ElementSizeUnit.Pixels);
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
                CheckBoxImageView.SetValue(() => CheckBoxImageView.Path, CheckBoxPressedImage);
                CheckBoxImageView.SetValue(() => CheckBoxImageView.Color, CheckBoxPressedColor);
            }
            else
            {
                CheckBoxImageView.SetValue(() => CheckBoxImageView.Path, CheckBoxImage);
                CheckBoxImageView.SetValue(() => CheckBoxImageView.Color, CheckBoxColor);
            }

            base.UpdateBehavior();
        }

        /// <summary>
        /// Called when check-box is clicked.
        /// </summary>
        public void CheckBoxClick()
        {
            SetValue(() => Checked, !Checked);
        }

        /// <summary>
        /// Gets embedded XML of the view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<CheckBox Click=""CheckBoxClick"">
                    <Group Id=""CheckBoxGroup"" Orientation=""Horizontal"" Spacing=""{Spacing}"">
                        <Image Id=""CheckBoxImageView"" Width=""{CheckBoxWidth}"" Height=""{CheckBoxHeight}""
                               Offset=""{CheckBoxOffset}"" BackgroundImageType=""{CheckBoxImageType}"" />
                        <Label Text=""{Text}"" Font=""{Font}"" FontStyle=""{FontStyle}"" FontSize=""{FontSize}"" FontColor=""{FontColor}"" TextAlignment=""{TextAlignment}"" AdjustToText=""Width"" Height=""100%"" ShadowColor=""{ShadowColor}"" ShadowDistance=""{ShadowDistance}"" OutlineColor=""{OutlineColor}"" OutlineDistance=""{OutlineDistance}"" />
                    </Group>
                </CheckBox>";
        }

        #endregion
    }
}
