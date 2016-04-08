#region Using Statements
using MarkUX.Animation;
using System;
using System.Collections;
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
    /// Contains a list item shown by the list view.
    /// </summary>
    [InternalView]
    public class ListItem : Button
    {
        #region Fields

        public int Index;
        public int ZeroBasedIndex;

        [NotSetFromXml]
        public object Item;

        [NotSetFromXml]
        public bool IsInitialized;

        public Label ItemLabel;
        public bool ShowItemLabel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ListItem()
        {
            ResizeToContent = true;
            ShowItemLabel = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<ListItem IsToggleButton=""True"" CanToggleOff=""False"" Click=""ButtonMouseClick"" MouseEnter=""ButtonMouseEnter"" MouseExit=""ButtonMouseExit"" MouseDown=""ButtonMouseDown"" MouseUp=""ButtonMouseUp"">
                        <Label Id=""ItemLabel"" Text=""{Text}"" Margin=""{TextMargin}"" Font=""{Font}"" FontStyle=""{FontStyle}"" FontSize=""{FontSize}"" FontColor=""{FontColor}"" TextAlignment=""{TextAlignment}"" Width=""100%"" Height=""100%"" ShadowColor=""{ShadowColor}"" ShadowDistance=""{ShadowDistance}"" OutlineColor=""{OutlineColor}"" OutlineDistance=""{OutlineDistance}"" Enabled=""{ShowItemLabel}"" />
                        <ViewAnimation Id=""HighlightImageAnimation"">
                            <Animate Field=""BackgroundImage"" To=""{HighlightedImage}"" />
                        </ViewAnimation>
                        <ViewAnimation Id=""PressedImageAnimation"">
                            <Animate Field=""BackgroundImage"" To=""{PressedImage}"" />
                        </ViewAnimation>
                        <ViewAnimation Id=""HighlightColorAnimation"">
                            <Animate Field=""BackgroundColor"" To=""{HighlightedColor}"" ReverseSpeed=""0.25"" Duration=""0.1"" />
                        </ViewAnimation>
                        <ViewAnimation Id=""PressedColorAnimation"">
                            <Animate Field=""BackgroundColor"" To=""{PressedColor}"" />
                        </ViewAnimation>
                    </ListItem>";
        }

        #endregion
    }
}
