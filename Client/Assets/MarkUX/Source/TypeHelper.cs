#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Helper methods for finding and instantiating objects through reflection.
    /// </summary>
    public static class TypeHelper
    {
        #region Methods

        /// <summary>
        /// Gets view field info from expression string.
        /// </summary>
        public static ViewFieldData GetViewFieldData(View sourceView, string viewFieldPath)
        {
            ViewFieldData fieldData = new ViewFieldData();

            if (String.IsNullOrEmpty(viewFieldPath) || sourceView == null)
                return null;

            Type viewType = typeof(View);
            var viewFields = viewFieldPath.Split('.');
            fieldData.Object = sourceView;

            // if first part of path can't be found we assume it's a reference to the source view
            int startIndex = 0;
            if (viewFields.Length > 1)
            {
                // yes. get first view field
                var firstViewField = viewFields[0];

                // is this a field within the source view?
                var fieldInfo = sourceView.GetType().GetField(firstViewField);
                if (fieldInfo == null)
                {
                    startIndex = 1; // no. assume it refers to the view itself
                }
            }

            // parse view field path
            for (int i = startIndex; i < viewFields.Length; ++i)
            {
                bool isLastField = (i == viewFields.Length - 1);
                string viewField = viewFields[i];
                                
                var fieldInfo = fieldData.Object.GetType().GetField(viewField);
                if (viewType.IsAssignableFrom(fieldData.Object.GetType()))
                {
                    fieldData.ClassFieldInfo = fieldInfo;
                }

                if (fieldInfo == null)
                {
                    Debug.LogError(String.Format("[MarkUX.378] Unable to parse view field path \"{0}\". Field \"{1}\" not found.", viewFieldPath, viewFields[i]));
                    return null;
                }

                if (isLastField)
                {
                    fieldData.ObjectFieldInfo = fieldInfo;
                    break;
                }
                else
                {
                    fieldData.Object = fieldInfo.GetValue(fieldData.Object);
                }

                if (fieldData.Object == null)
                {
                    // object along path was null
                    return null;
                }
            }

            return fieldData;
        }

        /// <summary>
        /// Gets all types derived from specified base type.
        /// </summary>
        public static IEnumerable<Type> FindDerivedTypes(Type baseType)
        {
            var derivedTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetLoadableTypes())
                {
                    try
                    {
                        if (baseType.IsAssignableFrom(type))
                        {
                            derivedTypes.Add(type);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return derivedTypes;
        }

        /// <summary>
        /// Extension method for getting loadable types from an assembly.
        /// </summary>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
            {
                return Enumerable.Empty<Type>();
            }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        /// <summary>
        /// Instiantiates a type.
        /// </summary>
        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        #endregion
    }
}