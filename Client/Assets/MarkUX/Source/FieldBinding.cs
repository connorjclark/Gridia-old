#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Contains information about field bindings.
    /// </summary>
    [Serializable]
    public class FieldBinding
    {
        #region Fields

        public string SourceFieldPathString;
        public string TargetFieldPathString;
        public GameObject Source;
        public GameObject Target;
        public string FormatString;
        public bool IsDynamic;
        public bool Update;

        [NonSerialized]
        public FieldInfo SourceViewField;

        [NonSerialized]
        public FieldInfo SourceObjectField;

        [NonSerialized]
        public FieldInfo TargetViewField;

        [NonSerialized]
        public FieldInfo TargetObjectField;

        [NonSerialized]
        public View SourceView;

        [NonSerialized]
        public View TargetView;
        
        [NonSerialized]
        public object SourceObject;

        [NonSerialized]
        public object TargetObject;

        [NonSerialized]
        public bool Initialized;

        [NonSerialized]
        public bool Remove;

        #endregion

        #region Constructor

        /// <summary>
        /// Initalizes a new instance of the class.
        /// </summary>
        public FieldBinding(string sourceFieldName, string targetFieldName, GameObject source, GameObject target, string formatString, bool isDynamic)
        {
            SourceFieldPathString = sourceFieldName;
            TargetFieldPathString = targetFieldName;
            Source = source;
            Target = target;
            FormatString = formatString;
            Initialized = false;
            IsDynamic = isDynamic;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes field binding information.
        /// </summary>
        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }

            var actualSource = Source;
            var actualTarget = Target;

            // check if this binding should be moved, to do this we look at source
            if (Source != null)
            {
                SourceView = Source.GetComponent<View>();
                var lastViewPath = ParseFieldPath(SourceFieldPathString, SourceView, out SourceObject, out SourceObjectField, out SourceViewField);

                if (lastViewPath.HasValue && lastViewPath.Value.Key != SourceView)
                {
                    // create new binding at path and initialize it
                    View newSource = lastViewPath.Value.Key;
                    var fieldBinding = new FieldBinding(lastViewPath.Value.Value, TargetFieldPathString, newSource.gameObject, Target, FormatString, false);
                    fieldBinding.Initialize();
                    newSource.FieldBindings.Add(fieldBinding);

                    // tag this up for removal
                    Remove = true;
                    actualSource = newSource.gameObject;
                }
            }

            if (Target != null)
            {
                TargetView = Target.GetComponent<View>();
                var lastViewPath = ParseFieldPath(TargetFieldPathString, TargetView, out TargetObject, out TargetObjectField, out TargetViewField);

                if (lastViewPath.HasValue && lastViewPath.Value.Key != TargetView)
                {
                    // update target
                    TargetView = lastViewPath.Value.Key;
                    Target = TargetView.gameObject;
                    TargetFieldPathString = lastViewPath.Value.Value;
                    actualTarget = TargetView.gameObject;
                }
            }

            // make sure source and target isn't the same
            if (actualSource != null && actualTarget != null && actualSource == actualTarget)
            {
                Debug.LogError(String.Format("[MarkUX.375] Invalid field binding: [{0}].{1} <-> [{2}].{3}. Source and target view can't be the same.", actualSource.name, SourceFieldPathString, actualTarget.name, TargetFieldPathString));

                // tag this binding up for removal
                Remove = true;
            }

            Initialized = true;
        }

        /// <summary>
        /// Parses a field path (e.g. SourceViewField.IndexedValue[2].TargetObject.TargetField) and returns target object and field info.
        /// </summary>
        public static KeyValuePair<View, string>? ParseFieldPath(string fieldPath, View sourceView, out object targetObject, out FieldInfo objectFieldInfo, out FieldInfo viewFieldInfo)
        {
            KeyValuePair<View, string> lastViewFieldPath = new KeyValuePair<View, string>(sourceView, fieldPath);
            bool isViewField = true;
            object currentObject = sourceView;
            viewFieldInfo = null;
            objectFieldInfo = null;
            targetObject = null;

            if (String.IsNullOrEmpty(fieldPath))
                return null;

            var fields = fieldPath.Split('.');
            for (int i = 0; i < fields.Length; ++i)
            {
                // parse index
                int index = -1;
                string fieldName = fields[i];

                int end = fields[i].IndexOf("]");
                if (end > 0)
                {
                    int start = fields[i].IndexOf('[') + 1;
                    if (!Int32.TryParse(fieldPath.Substring(start, end - start), out index))
                    {
                        Debug.LogError(String.Format("[MarkUX.301] {0}: Unable to parse field path \"{1}\".", sourceView.Name, fieldPath));
                        return null;
                    }

                    fieldName = fields[i].Substring(0, start - 1);
                }

                var fieldInfo = currentObject.GetType().GetField(fieldName);
                if (fieldInfo == null)
                {
                    // handle special case if the field name is "Item" (reference to list item)
                    if (String.Equals(fieldName, "Item", StringComparison.OrdinalIgnoreCase))
                    {
                        objectFieldInfo = null;
                        return null;
                    }

                    // if it's the first field and not last - check if the view is in the hierarchy
                    if (i == 0 && fields.Length != 1)
                    {
                        var result = lastViewFieldPath.Key.FindView(fieldName, true);
                        if (result != null)
                        {
                            // view found
                            lastViewFieldPath = new KeyValuePair<View,string>(result, String.Join(".", fields.Skip(1).ToArray()));
                            currentObject = result;
                            continue;
                        }
                    }

                    objectFieldInfo = null;
                    Debug.LogError(String.Format("[MarkUX.302] {0}: Unable to parse field path \"{1}\". Couldn't find field/view \"{2}\".", sourceView.Name, fieldPath, fields[i]));
                    return null;
                }

                // is this a class field?
                if (isViewField)
                {
                    // yes. 
                    viewFieldInfo = fieldInfo;
                }

                // check if field is of type View
                if (fieldInfo.FieldType.IsSubclassOf(typeof(View)) || fieldInfo.FieldType == typeof(View))
                {
                    // next field will be a view field
                    isViewField = true;

                    if (i == fields.Length - 1)
                    {
                        // last field can't be a view field
                        Debug.LogError(String.Format("[MarkUX.303] {0}: Unable to parse field path \"{1}\". Last member access can't be a view.", sourceView.Name, fieldPath));
                        return null;
                    }
                    
                    lastViewFieldPath = new KeyValuePair<View, string>(viewFieldInfo.GetValue(currentObject) as View, String.Join(".", fields.Skip(i + 1).ToArray()));
                }
                else
                {
                    isViewField = false;
                }

                // is this the last field?
                if (i == fields.Length - 1)
                {
                    // yes. it's the object field
                    if (index != -1)
                    {
                        Debug.LogError(String.Format("[MarkUX.304] {0}: Unable to parse field path \"{1}\". Last member access can't be indexed.", sourceView.Name, fieldPath));
                        return null;
                    }

                    objectFieldInfo = fieldInfo;
                    targetObject = currentObject;
                }
                else
                {
                    // get next object
                    currentObject = fieldInfo.GetValue(currentObject);
                    if (index != -1)
                    {
                        // indexed object
                        var getItemMethod = currentObject.GetType().GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (getItemMethod == null)
                        {
                            Debug.LogError(String.Format("[MarkUX.305] {0}: Unable to parse field path \"{1}\". Unable to retrieve indexed object \"{2}\".", sourceView.Name, fieldPath, fields[i]));
                            return null;
                        }
                        currentObject = getItemMethod.Invoke(currentObject, new object[] { index });
                    }
                }

                if (currentObject == null)
                {
                    // object along path was null
                    return null;
                }
            }

            return lastViewFieldPath;
        }

        /// <summary>
        /// Propagates value from source to target.
        /// </summary>
        public void PropagateValue(List<KeyValuePair<View, FieldInfo>> callstack = null)
        {
            if (SourceObjectField == null)
                return;

            // get source value
            var value = SourceObjectField.GetValue(SourceObject);

            // check if target has a format string
            if (!String.IsNullOrEmpty(FormatString))
            {
                value = String.Format(FormatString, value);
            }

            // init callstack                        
            if (callstack == null)
            {
                callstack = new List<KeyValuePair<View, FieldInfo>>();
                callstack.Add(new KeyValuePair<View, FieldInfo>(SourceView, SourceViewField));
            }

            //Debug.Log(String.Format("Propagating value \"{0}\" from {1} to {2}.", value, SourceFieldPathString, TargetFieldPathString));

            // update target object
            TargetView.SetValue(TargetViewField, TargetObjectField, TargetObject, value, callstack, false, false, false);
        }

        #endregion
    }
}
