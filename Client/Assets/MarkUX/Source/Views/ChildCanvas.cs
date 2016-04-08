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
    /// Child canvas view.
    /// </summary>
    [InternalView]
    [RemoveComponent(typeof(Image))]
    [AddComponent(typeof(UnityEngine.Canvas))]
    [AddComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
    public class ChildCanvas : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateBehavior")]
        public bool OverrideSort;

        [ChangeHandler("UpdateBehavior")]
        public int SortingOrder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ChildCanvas()
        {
            OverrideSort = false;
            SortingOrder = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();

            var canvas = GetComponent<UnityEngine.Canvas>();
            canvas.overrideSorting = OverrideSort;
            if (OverrideSort)
            {
                canvas.sortingOrder = SortingOrder;
            }
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<ChildCanvas />";
        }

        #endregion
    }
}
