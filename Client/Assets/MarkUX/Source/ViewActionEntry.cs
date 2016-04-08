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
    /// Contains information about method to be called when an action is triggered.
    /// </summary>
    [Serializable]
    public class ViewActionEntry
    {
        #region Fields

        [SerializeField]
        private string MethodName;

        [SerializeField]
        public GameObject ViewObject;

        private View _view;
        private MethodInfo _viewActionMethod;
        private bool _initialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Initalizes a new instance of the class.
        /// </summary>
        public ViewActionEntry(string methodName, GameObject viewObject)
        {
            MethodName = methodName;
            ViewObject = viewObject;
            _initialized = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invokes the view action method.
        /// </summary>
        public void Invoke(GameObject source, BaseEventData eventData, ActionData actionData)
        {
            if (!_initialized)
            {
                _initialized = true;
                _view = ViewObject.GetComponent<View>();

                // look for a method with the same name as the entry
                _viewActionMethod = _view.GetType().GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (_viewActionMethod == null)
                {
                    return;
                }
            }

            if (_viewActionMethod != null)
            {
                //Debug.Log(String.Format("Invoking method \"{0}.{1}\".", _view.Name, _name));

                // check set action parameters 
                ParameterInfo[] parsInfo = _viewActionMethod.GetParameters();
                object[] pars = parsInfo.Length > 0 ? new object[parsInfo.Length] : null;
                for (int i = 0; i < parsInfo.Length; ++i)
                {
                    if (parsInfo[i].ParameterType == typeof(GameObject))
                    {
                        if (parsInfo[i].Name.IndexOf("parent", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            pars[i] = _view != null ? _view.gameObject : null;                            
                        }
                        else
                        {
                            pars[i] = source;
                        }
                    }
                    else if (parsInfo[i].ParameterType.IsSubclassOf(typeof(View)) || parsInfo[i].ParameterType == typeof(View))
                    {
                        if (parsInfo[i].Name.IndexOf("parent", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (_view != null)
                            {
                                if(_view.GetType() != parsInfo[i].ParameterType &&
                                    !_view.GetType().IsSubclassOf(parsInfo[i].ParameterType))
                                {
                                    Debug.LogError(String.Format("[MarkUX.353] View action \"{0}.{1}\" has parameter \"{2}\" with invalid type. Expected type (or baseclass of) \"{3}\".", _view.Name, MethodName, parsInfo[i].Name, _view.GetType().Name));
                                    return;
                                }
                            }

                            pars[i] = _view;
                        }
                        else
                        {
                            var sourceView = source != null ? source.GetComponent<View>() : null;
                            if (sourceView != null)
                            {
                                if (sourceView.GetType() != parsInfo[i].ParameterType &&
                                    !sourceView.GetType().IsSubclassOf(parsInfo[i].ParameterType))
                                {
                                    Debug.LogError(String.Format("[MarkUX.354] View action \"{0}.{1}\" has parameter \"{2}\" with invalid type. Expected type (or baseclass of) \"{3}\".", _view.Name, MethodName, parsInfo[i].Name, sourceView.GetType().Name));
                                    return;
                                }
                            }

                            pars[i] = source != null ? source.GetComponent<View>() : null;
                        }
                    }
                    else if (parsInfo[i].ParameterType.IsSubclassOf(typeof(BaseEventData)) || parsInfo[i].ParameterType == typeof(BaseEventData))
                    {
                        pars[i] = eventData;
                    }
                    else if (parsInfo[i].ParameterType.IsSubclassOf(typeof(ActionData)) || parsInfo[i].ParameterType == typeof(ActionData))
                    {
                        pars[i] = actionData;
                    }
                    else
                    {
                        Debug.LogError(String.Format("[MarkUX.311] View action \"{0}.{1}\" has parameter \"{2}\" with invalid type. Only GameObject or subtypes of View, ActionData and BaseEventData are allowed.", _view.Name, MethodName, parsInfo[i].Name));
                        return;
                    }
                }                               

                _viewActionMethod.Invoke(_view, pars);
            }
        }

        #endregion
    }
}
