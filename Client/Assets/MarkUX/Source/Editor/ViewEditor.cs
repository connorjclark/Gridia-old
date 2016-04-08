#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MarkUX.Editor
{
    /// <summary>
    /// Custom inspector for View components.
    /// </summary>
    [CustomEditor(typeof(View), true)]
    public class ViewEditor : UnityEditor.Editor
    {
        #region Fields
        #endregion

        #region Methods

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            View view = (View)target;

            // update view button
            if (GUILayout.Button("Update View"))
            {
                view.UpdateViews();
            }
        }

        #endregion

    }
}
