#region Using Statements
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.UnityProject
{
    /// <summary>
    /// View that showcases the RadioButton.
    /// </summary>
    [InternalView]
    public class Showcase_RadioButton : View
    {
        #region Fields

        public string SelectedRadioButton;

        #endregion

        #region Methods

        /// <summary>
        /// Called when the user clicks on a check-box.
        /// </summary>
        public void RadioButtonClick(RadioButton source)
        {
            SetValue(() => SelectedRadioButton, source.Id);
        }

        #endregion
    }
}
