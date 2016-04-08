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
    /// Image view.
    /// </summary>
    [InternalView]
    public class Image : View
    {
        #region Fields

        [ChangeHandler("UpdateBehavior")]
        public Color Color;
        public bool ColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Sprite Path;
        public bool PathSet;

        [ChangeHandler("UpdateBehavior")]
        public UnityEngine.UI.Image.Type Type;
        public bool TypeSet;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Image()
        {
            Path = null;
            Color = Color.clear;
            Type = UnityEngine.UI.Image.Type.Simple;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the behavior and appearance of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            if (ColorSet)
            {
                BackgroundColor = Color;
                BackgroundColorSet = ColorSet;
            }

            if (PathSet)
            {
                BackgroundImage = Path;
            }

            if (TypeSet)
            {
                BackgroundImageType = Type;
            }

            base.UpdateBehavior();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return "<Image />";
        }

        #endregion
    }
}
