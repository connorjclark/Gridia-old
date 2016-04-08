#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Provides separation between UI and view logic.
    /// </summary>
    [Serializable]
    public class ViewAction
    {
        #region Fields

        [SerializeField]
        private string _name;

        [SerializeField]
        private GameObject _source;

        [SerializeField]
        private List<ViewActionEntry> _entries;

        [SerializeField]
        private bool _triggeredByEventSystem;

        [SerializeField]
        private EventTriggerType _eventTriggerType;

        public static Dictionary<string, EventTriggerType> EventTriggerTypes;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes static instance of the class.
        /// </summary>
        static ViewAction()
        {
            EventTriggerTypes = new Dictionary<string, EventTriggerType>();
            EventTriggerTypes.Add("BeginDrag", EventTriggerType.BeginDrag);
            EventTriggerTypes.Add("Cancel", EventTriggerType.Cancel);
            EventTriggerTypes.Add("Deselect", EventTriggerType.Deselect);            
            EventTriggerTypes.Add("Drag", EventTriggerType.Drag);
            EventTriggerTypes.Add("Drop", EventTriggerType.Drop);
            EventTriggerTypes.Add("EndDrag", EventTriggerType.EndDrag);
            EventTriggerTypes.Add("InitializePotentialDrag", EventTriggerType.InitializePotentialDrag);            
            EventTriggerTypes.Add("Move", EventTriggerType.Move);            
            EventTriggerTypes.Add("Click", EventTriggerType.PointerClick);
            EventTriggerTypes.Add("PointerClick", EventTriggerType.PointerClick);
            EventTriggerTypes.Add("MouseClick", EventTriggerType.PointerClick);            
            EventTriggerTypes.Add("PointerDown", EventTriggerType.PointerDown);
            EventTriggerTypes.Add("MouseDown", EventTriggerType.PointerDown);
            EventTriggerTypes.Add("PointerEnter", EventTriggerType.PointerEnter);
            EventTriggerTypes.Add("MouseEnter", EventTriggerType.PointerEnter);
            EventTriggerTypes.Add("PointerExit", EventTriggerType.PointerExit);
            EventTriggerTypes.Add("MouseExit", EventTriggerType.PointerExit);
            EventTriggerTypes.Add("PointerUp", EventTriggerType.PointerUp);
            EventTriggerTypes.Add("MouseUp", EventTriggerType.PointerUp);
            EventTriggerTypes.Add("Scroll", EventTriggerType.Scroll);
            EventTriggerTypes.Add("Select", EventTriggerType.Select);
            EventTriggerTypes.Add("Submit", EventTriggerType.Submit);
            EventTriggerTypes.Add("UpdateSelected", EventTriggerType.UpdateSelected);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewAction(GameObject viewObject = null, string name = "")
        {
            _name = name;
            _source = viewObject;
            _entries = new List<ViewActionEntry>();
            _triggeredByEventSystem = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Triggers the view action with base event data.
        /// </summary>
        public void Trigger(BaseEventData eventData = null)
        {
            //Debug.Log(String.Format("Triggered action \"{0}.{1}\".", _source.name, _name));

            // go through the entries and call them
            foreach (var entry in _entries)
            {
                entry.Invoke(_source, eventData, null);
            }
        }

        /// <summary>
        /// Triggers the view action with action data.
        /// </summary>
        public void Trigger(ActionData actionData)
        {
            //Debug.Log(String.Format("Triggered action \"{0}.{1}\".", _source.name, _name));

            // go through the entries and call them
            foreach (var entry in _entries)
            {
                entry.Invoke(_source, null, actionData);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets view action name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets view object that the action belongs to.
        /// </summary>
        public GameObject ViewObject
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Gets or sets view action entries.
        /// </summary>
        public List<ViewActionEntry> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        /// <summary>
        /// Gets or sets boolean indicating if action is triggered by the event system.
        /// </summary>
        public bool TriggeredByEventSystem
        {
            get { return _triggeredByEventSystem; }
            set { _triggeredByEventSystem = value; }
        }

        /// <summary>
        /// Gets or sets type of event that triggers action by the event system.
        /// </summary>
        public EventTriggerType EventTriggerType
        {
            get { return _eventTriggerType; }
            set { _eventTriggerType = value; }
        }

        #endregion
    }
}
