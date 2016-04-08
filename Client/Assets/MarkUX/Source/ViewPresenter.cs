#region Using Statements
using System.Collections;
using System;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;
using MarkUX.Animation;
using MarkUX.Views;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Manages and presents views on a canvas.
    /// </summary>
    public class ViewPresenter : MonoBehaviour
    {
        #region Fields

        [HideInInspector]
        public float ElementSize;

        [HideInInspector]
        public bool DrawGrid;

        [HideInInspector]
        public string MainView;

        [HideInInspector]
        public string Theme;
        
        [HideInInspector]
        public List<string> Views;

        [HideInInspector]
        public List<string> Themes;

        [HideInInspector]
        public GameObject LayoutRoot;

        [HideInInspector]
        public GameObject TemplateRoot;

        [HideInInspector]
        public bool DisableUndo;

        private bool _initialized = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewPresenter()
        {
            ElementSize = 40;
            DrawGrid = false;
            Views = new List<string>();
            Themes = new List<string>();
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Called on exit.
        /// </summary>
        public void OnApplicationQuit()
        {
            CleanUp();
        }

        /// <summary>
        /// Initializes the view engine.
        /// </summary>
        public void Awake()
        {
            // initialize view data
            ViewData.Initialize(ElementSize);

            // initialize views
            InitializeViews();            
        }
                
        /// <summary>
        /// Calls the initial update on all the views.
        /// </summary>
        public void Update()
        {
            // call update once more after the views have been presented
            if (!_initialized)
            {
                // update views
                UpdateViews();

                // trigger any change handlers
                TriggerChangeHandlers();

                _initialized = true;
            }
        }

        /// <summary>
        /// Initializes children.
        /// </summary>
        public void InitializeViews()
        {
            gameObject.ForEachChild<View>(x => { x.InitializeInternal(); x.Initialize(); });
        }

        /// <summary>
        /// Goes through all views (reverse breadth first) and updates them.
        /// </summary>
        public void UpdateViews()
        {
            gameObject.ForEachChild<View>(x => x.UpdateView(), true, null, SearchAlgorithm.ReverseBreadthFirst);
        }

        /// <summary>
        /// Goes through all views (reverse breadth first) and triggers their change handlers. We need to do this because inactive views don't have their 
        /// change handlers triggered.
        /// </summary>
        private void TriggerChangeHandlers()
        {
            gameObject.ForEachChild<View>(x => x.TriggerChangeHandlers(), true, null, SearchAlgorithm.ReverseBreadthFirst);
        }

        /// <summary>
        /// Goes through the views and cleans them up.
        /// </summary>
        public void CleanUp()
        {
            gameObject.ForEachChild<View>(x => x.CleanUp(), true, null, SearchAlgorithm.ReverseBreadthFirst);
        }

        /// <summary>
        /// Gets view template.
        /// </summary>
        public GameObject GetViewTemplate(Type viewType, string style = null)
        {
            bool filterByStyle = !String.IsNullOrEmpty(style);

            // try find view template
            View template = null;
            TemplateRoot.ForEachChild<View>(x =>
            {
                if (x.GetType() != viewType)
                    return;

                if (filterByStyle && !String.Equals(x.Style, style, StringComparison.OrdinalIgnoreCase))
                    return;

                template = x;
            }, false);
            
            return template != null ? template.gameObject : null;
        }

        #endregion
    }
}
