#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MarkUX.Animation;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// ScrollRect view.
    /// </summary>
    [InternalView]
    [AddComponent(typeof(UnityEngine.UI.ScrollRect))]
    public class ScrollRect : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateBehavior")]
        public bool ScrollVertical;

        [ChangeHandler("UpdateBehavior")]
        public bool ScrollHorizontal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ScrollRect()
        {
            UpdateBackground = false;
            ScrollVertical = true;
            ScrollHorizontal = true;
        }

        #endregion
     
        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public void ChildLayoutUpdatedHasChanged()
        {
            UpdateLayout();
        }

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            base.UpdateLayout();
                        
            var scrollRect = GetComponent<UnityEngine.UI.ScrollRect>();
            if (scrollRect.content == null && transform.childCount > 0)
            {
                var t = transform.GetChild(0);
                if (t != null)
                {
                    scrollRect.content = t.GetComponent<RectTransform>();
                }
            }

            // workaround for blocking of drag events in child views
            UnblockDragEvents();
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();

            var scrollRect = GetComponent<UnityEngine.UI.ScrollRect>();
            scrollRect.vertical = ScrollVertical;
            scrollRect.horizontal = ScrollHorizontal;

            var image = GetComponent<UnityEngine.UI.Image>();
            image.color = Color.clear;
        }

        /// <summary>
        /// Workaround for blocking of drag events in child views.
        /// </summary>
        private void UnblockDragEvents()
        {
            this.ForEachChild<View>(x =>
            {
                var eventTrigger = x.GetComponent<EventTrigger>();

                if (eventTrigger == null)
                    return;

#if UNITY_4_6 || UNITY_5_0
                var triggers = eventTrigger.delegates;
#else
                var triggers = eventTrigger.triggers;
#endif      

                if (triggers == null)
                    return;

                // check if view has drag event entries
                bool hasDragEntries = false;
                foreach (var entry in triggers)
                {
                    if (entry.eventID == EventTriggerType.BeginDrag ||
                        entry.eventID == EventTriggerType.EndDrag ||
                        entry.eventID == EventTriggerType.Drag ||
                        entry.eventID == EventTriggerType.InitializePotentialDrag)
                    {
                        hasDragEntries = true;
                    }
                }

                // unblock drag events if the view doesn't handle drag events
                if (!hasDragEntries)
                {
                    ScrollRect scrollRect = this;

                    // unblock initialize potential drag 
                    var initializePotentialDragEntry = new EventTrigger.Entry();
                    initializePotentialDragEntry.eventID = EventTriggerType.InitializePotentialDrag;
                    initializePotentialDragEntry.callback = new EventTrigger.TriggerEvent();
                    initializePotentialDragEntry.callback.AddListener(eventData =>
                    {
                        scrollRect.SendMessage("OnInitializePotentialDrag", eventData);
                    });
                    triggers.Add(initializePotentialDragEntry);

                    // unblock begin drag
                    var beginDragEntry = new EventTrigger.Entry();
                    beginDragEntry.eventID = EventTriggerType.BeginDrag;
                    beginDragEntry.callback = new EventTrigger.TriggerEvent();
                    beginDragEntry.callback.AddListener(eventData =>
                    {
                        scrollRect.SendMessage("OnBeginDrag", eventData);
                    });
                    triggers.Add(beginDragEntry);

                    // drag
                    var dragEntry = new EventTrigger.Entry();
                    dragEntry.eventID = EventTriggerType.Drag;
                    dragEntry.callback = new EventTrigger.TriggerEvent();                    
                    dragEntry.callback.AddListener(eventData =>
                    {
                        scrollRect.SendMessage("OnDrag", eventData);
                    });
                    triggers.Add(dragEntry);

                    // end drag
                    var endDragEntry = new EventTrigger.Entry();
                    endDragEntry.eventID = EventTriggerType.EndDrag;
                    endDragEntry.callback = new EventTrigger.TriggerEvent();
                    endDragEntry.callback.AddListener(eventData =>
                    {
                        scrollRect.SendMessage("OnEndDrag", eventData);
                    });
                    triggers.Add(endDragEntry);
                }
            });
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<ScrollRect UpdateBackground=""False"" />";
        }

        #endregion
    }
}
