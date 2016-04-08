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
    /// Contains a list item shown by the flow list view. Resizes itself to its content.
    /// </summary>
    [InternalView]
    public class FlowListItem : Button
    {
        #region Fields

        public int Index;
        public int ZeroBasedIndex;

        [NotSetFromXml]
        public object Item;

        [NotSetFromXml]
        public bool IsInitialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FlowListItem()
        {
            ResizeToContent = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            base.UpdateLayout();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<FlowListItem IsToggleButton=""True"" CanToggleOff=""False"" CanToggleOn=""False"" Click=""ButtonMouseClick"" MouseEnter=""ButtonMouseEnter"" MouseExit=""ButtonMouseExit"" MouseDown=""ButtonMouseDown"" MouseUp=""ButtonMouseUp"">
                        <Label Text=""{Text}"" Font=""{Font}"" FontStyle=""{FontStyle}"" FontSize=""{FontSize}"" FontColor=""{FontColor}"" TextAlignment=""{TextAlignment}"" Width=""100%"" Height=""100%"" ShadowColor=""{ShadowColor}"" ShadowDistance=""{ShadowDistance}"" OutlineColor=""{OutlineColor}"" OutlineDistance=""{OutlineDistance}"" />
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
                    </FlowListItem>";
        }

        #endregion
    }
}
