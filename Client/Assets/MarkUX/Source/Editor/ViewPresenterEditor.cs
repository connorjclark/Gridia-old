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
    /// Custom inspector for ViewPresenter components.
    /// </summary>
    [CustomEditor(typeof(ViewPresenter))]
    public class ViewPresenterEditor : UnityEditor.Editor
    {
        #region Methods

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var viewPresenter = (ViewPresenter)target;

            // main view selection
            int selectedViewIndex = viewPresenter.Views.IndexOf(viewPresenter.MainView) + 1;

            // .. add empty selection
            var mainViewOptions = new List<string>(viewPresenter.Views);
            mainViewOptions.Insert(0, "-- no main view --");

            // .. add drop down logic
            int newSelectedViewIndex = EditorGUILayout.Popup("Main View", selectedViewIndex, mainViewOptions.ToArray());
            viewPresenter.MainView = newSelectedViewIndex > 0 ? viewPresenter.Views[newSelectedViewIndex - 1] : String.Empty;
            if (newSelectedViewIndex != selectedViewIndex)
            {
                // .. trigger reload on the views
                ViewPostprocessor.ProcessViewAssets();
            }

            // theme selection
            int selectedThemeIndex = viewPresenter.Themes.IndexOf(viewPresenter.Theme) + 1;

            // .. add empty selection
            var themeOptions = new List<string>(viewPresenter.Themes);
            themeOptions.Insert(0, "-- no theme --");

            // .. add drop down logic
            int newSelectedThemeIndex = EditorGUILayout.Popup("Theme", selectedThemeIndex, themeOptions.ToArray());
            viewPresenter.Theme = newSelectedThemeIndex > 0 ? viewPresenter.Themes[newSelectedThemeIndex - 1] : String.Empty;
            if (newSelectedThemeIndex != selectedThemeIndex)
            {
                // .. trigger reload on the views
                ViewPostprocessor.ProcessViewAssets();
            }

            // reload button
            if (GUILayout.Button("Reload Views"))
            {
                // .. trigger reload on all views
                ViewPostprocessor.ProcessViewAssets();
            }

            viewPresenter.ElementSize = EditorGUILayout.FloatField("Element Size", viewPresenter.ElementSize);
            bool newDrawGrid = EditorGUILayout.Toggle("Draw Grid", viewPresenter.DrawGrid);
            if (newDrawGrid != viewPresenter.DrawGrid)
            {
                viewPresenter.DrawGrid = newDrawGrid;
                ViewPostprocessor.ProcessViewAssets();
            }

            bool newDisableUndo = EditorGUILayout.Toggle("Disable Undo (saves memory in editor)", viewPresenter.DisableUndo);
            viewPresenter.DisableUndo = newDisableUndo;
        }

        #endregion
    }
}
