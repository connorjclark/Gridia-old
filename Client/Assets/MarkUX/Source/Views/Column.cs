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
    /// A column item shown by the datagrid view.
    /// </summary>
    [InternalView]    
    public class Column : ContentView
    {
        #region Fields

        public int Index;
        public int ZeroBasedIndex;

        [NotSetFromXml]
        public object Item;

        [NotSetFromXml]
        public bool IsInitialized;

        // column label
        [ChangeHandler("UpdateBehavior")]
        public string Text;
        public Margin TextOffset;
        public Font Font;
        public FontStyle FontStyle;
        public int FontSize;
        public Color FontColor;
        public Alignment TextAlignment;
        public Color ShadowColor;
        public Vector2 ShadowDistance;
        public Color OutlineColor;
        public Vector2 OutlineDistance;

        public Label ColumnLabel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Column()
        {
            // column label
            Text = String.Empty;
            TextOffset = new Margin();
            FontStyle = UnityEngine.FontStyle.Normal;
            FontSize = 18;
            FontColor = Color.black;
            TextAlignment = MarkUX.Alignment.Center;
            ShadowColor = Color.clear;
            ShadowDistance = Vector2.zero;
            OutlineColor = Color.clear;
            OutlineDistance = Vector2.zero;
            AlwaysBlockRaycast = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();

            if (transform.childCount > 1)
            {
                // hide text label
                ColumnLabel.Deactivate();
            }
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Column AlwaysBlockRaycast=""True"">
                        <Label Id=""ColumnLabel"" Text=""{Text}"" Offset=""{TextOffset}"" Font=""{Font}"" FontStyle=""{FontStyle}"" FontSize=""{FontSize}"" FontColor=""{FontColor}"" TextAlignment=""{TextAlignment}"" Width=""100%"" Height=""100%"" ShadowColor=""{ShadowColor}"" ShadowDistance=""{ShadowDistance}"" OutlineColor=""{OutlineColor}"" OutlineDistance=""{OutlineDistance}"" />
                    </Column>";
        }

        #endregion
    }
}
