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
    /// View that showcases the CheckBox.
    /// </summary>
    [InternalView]
    public class Showcase_CheckBox : View
    {
        #region Fields

        public int CheckedCount = 0;
        public Group CheckBoxGroup;

        #endregion

        #region Methods

        /// <summary>
        /// Called when the user clicks on a check-box.
        /// </summary>
        public void CheckBoxClick(CheckBox source)
        {
            int count = 0;
            CheckBoxGroup.ForEachChild<CheckBox>(x =>
            {
                if (x.Checked)
                {
                    ++count;
                }
            }, false);

            SetValue(() => CheckedCount, count);
        }

        #endregion
    }
}
