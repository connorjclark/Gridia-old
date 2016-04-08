#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MarkUX
{
    /// <summary>
    /// View that contains template content.
    /// </summary>
    public class TemplateView : ContentView
    {
        #region Fields

        [NotSetFromXml]
        public GameObject Template;

        #endregion

        #region Methods

        /// <summary>
        /// Returns template view type.
        /// </summary>
        public virtual Type GetTemplateViewType()
        {
            return typeof(View);
        }

        #endregion
    }
}
