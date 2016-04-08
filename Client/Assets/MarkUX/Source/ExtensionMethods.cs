#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Gets a list of children. 
        /// </summary>
        public static List<T> GetChildren<T>(this GameObject gameObject, bool recursive = true, View parent = null, SearchAlgorithm searchAlgorithm = SearchAlgorithm.DepthFirst) where T : View
        {
            return gameObject.GetChildren<T>(null, recursive, parent, searchAlgorithm);
        }

        /// <summary>
        /// Gets a list of children. 
        /// </summary>
        public static List<T> GetChildren<T>(this GameObject gameObject, Func<T, bool> predicate = null, bool recursive = true, View parent = null, SearchAlgorithm searchAlgorithm = SearchAlgorithm.DepthFirst) where T : View
        {
            List<T> children = new List<T>();
            gameObject.ForEachChild<T>(x =>
            {
                if (predicate == null || predicate(x))
                {
                    children.Add(x);
                }
            }, recursive, parent, searchAlgorithm);

            return children;
        }

        /// <summary>
        /// Performs an action on view children of a game object.
        /// </summary>
        /// <typeparam name="T">Types of views to do action on.</typeparam>
        /// <param name="gameObject">Root game object.</param>
        /// <param name="action">Action to perform.</param>
        /// <param name="recursive">Boolean indicating if children of children should be traversed.</param>
        /// <param name="contentChild">Boolean indicating if views must be content children (i.e. appearing as content in the view).</param>
        /// <param name="searchAlgorithm">Search algorithm to use.</param>
        public static void ForEachChild<T>(this GameObject gameObject, Action<T> action, bool recursive = true, View parent = null, SearchAlgorithm searchAlgorithm = SearchAlgorithm.DepthFirst) where T : View
        {
            switch (searchAlgorithm)
            {
                default:
                case SearchAlgorithm.DepthFirst:
                    foreach (Transform child in gameObject.transform)
                    {
                        bool skipChild = false;
                        if (parent != null)
                        {
                            var view = child.GetComponent<View>();
                            if (view.Parent != parent.gameObject)
                                skipChild = true;
                        }

                        if (!skipChild)
                        {
                            var component = child.GetComponent<T>();
                            if (component != null)
                            {
                                action(component);
                            }
                        }

                        if (recursive)
                        {
                            child.gameObject.ForEachChild(action, recursive);
                        }
                    }
                    break;

                case SearchAlgorithm.BreadthFirst:
                    Queue<GameObject> queue = new Queue<GameObject>();
                    foreach (Transform child in gameObject.transform)
                    {
                        bool skipChild = false;
                        if (parent != null)
                        {
                            var view = child.GetComponent<View>();
                            if (view.Parent != parent.gameObject)
                                skipChild = true;
                        }

                        if (!skipChild)
                        {
                            var component = child.GetComponent<T>();
                            if (component != null)
                            {
                                action(component);
                            }
                        }

                        if (recursive)
                        {
                            // add children to queue
                            queue.Enqueue(child.gameObject);
                        }
                    }

                    foreach (GameObject go in queue)
                    {
                        go.ForEachChild(action, recursive, parent, searchAlgorithm);
                    }
                    break;

                case SearchAlgorithm.ReverseDepthFirst:
                    foreach (Transform child in gameObject.transform)
                    {
                        if (recursive)
                        {
                            child.gameObject.ForEachChild(action, recursive);
                        }

                        if (parent != null)
                        {
                            var view = child.GetComponent<View>();
                            if (view.Parent != parent.gameObject)
                                continue;
                        }

                        var component = child.GetComponent<T>();
                        if (component != null)
                        {
                            action(component);
                        }
                    }
                    break;

                case SearchAlgorithm.ReverseBreadthFirst:
                    Stack<T> componentStack = new Stack<T>();
                    Stack<GameObject> childStack = new Stack<GameObject>();
                    foreach (Transform child in gameObject.transform)
                    {
                        if (recursive)
                        {
                            childStack.Push(child.gameObject);
                        }

                        if (parent != null)
                        {
                            var view = child.GetComponent<View>();
                            if (view.Parent != parent.gameObject)
                                continue;
                        }

                        var component = child.GetComponent<T>();
                        if (component != null)
                        {
                            componentStack.Push(component);
                        }
                    }

                    foreach (var childObject in childStack)
                    {
                        childObject.ForEachChild(action, recursive, parent, searchAlgorithm);
                    }

                    foreach (T component in componentStack)
                    {
                        action(component);
                    }

                    break;
            }
        }

        /// <summary>
        /// Performs an action on view children of a view.
        /// </summary>
        public static void ForEachChild<T>(this View view, Action<T> action, bool recursive = true, View parent = null, SearchAlgorithm searchAlgorithm = SearchAlgorithm.DepthFirst) where T : View
        {
            view.gameObject.ForEachChild<T>(action, recursive, parent, searchAlgorithm);
        }

        /// <summary>
        /// Performs an action on all parents of a game object.
        /// </summary>
        public static void ForEachParent<T>(this GameObject gameObject, Action<T> action, bool recursive = true) where T : UnityEngine.Component
        {
            var parent = gameObject.transform.parent;
            if (parent != null)
            {
                var component = parent.GetComponent<T>();
                if (component != null)
                {
                    action(component);
                }

                if (recursive)
                {
                    parent.gameObject.ForEachParent(action, recursive);
                }
            }
        }

        /// <summary>
        /// Gets layout root.
        /// </summary>
        public static GameObject GetLayoutRoot(this GameObject gameObject)
        {
            var viewComponent = gameObject.GetComponent<View>();
            if (viewComponent != null && viewComponent.IsLayoutRoot)
            {
                return gameObject;
            }
            
            var parent = gameObject.transform.parent;
            if (parent == null)
                return null;

            return parent.gameObject.GetLayoutRoot();
        }

        /// <summary>
        /// Gets layout root.
        /// </summary>
        public static View GetLayoutRoot(this View view)
        {
            var go = view.gameObject.GetLayoutRoot();
            return go != null ? go.GetComponent<View>() : null;
        }

        /// <summary>
        /// Finds view.
        /// </summary>
        public static View FindView(this View view, string id, bool recursive = true, View parent = null)
        {
            return view.FindView<View>(id, recursive, parent);
        }

        /// <summary>
        /// Finds view.
        /// </summary>
        public static T FindView<T>(this View view, string id, bool recursive = true, View parent = null) where T : View
        {
            var go = view.gameObject.FindView<T>(id, recursive, parent);
            return go != null ? go.GetComponent<T>() : null;
        }

        /// <summary>
        /// Finds view.
        /// </summary>
        public static GameObject FindView(this GameObject gameObject, string id, bool recursive = true, View parent = null)
        {
            return gameObject.FindView<View>(id, recursive, parent);
        }

        /// <summary>
        /// Finds view.
        /// </summary>
        public static GameObject FindView<T>(this GameObject gameObject, string id, bool recursive = true, View parent = null) where T : View
        {
            return gameObject.FindView<T>(id, true, recursive, parent);
        }

        /// <summary>
        /// Finds view.
        /// </summary>
        public static GameObject FindView<T>(this GameObject gameObject, bool recursive, View parent = null) where T : View
        {
            return gameObject.FindView<T>(null, false, recursive, parent);
        }

        /// <summary>
        /// Finds view.
        /// </summary>
        public static GameObject FindView<T>(this GameObject gameObject, string id, bool filterById, bool recursive, View parent) where T : View
        {
            foreach (Transform child in gameObject.transform)
            {
                var viewComponent = child.GetComponent<T>();
                if (viewComponent != null)
                {
                    if ((!filterById || String.Equals(id, viewComponent.Id, StringComparison.OrdinalIgnoreCase)) &&
                        (parent == null || viewComponent.Parent == parent.gameObject))
                    {
                        return child.gameObject;
                    }
                }

                if (recursive)
                {
                    var result = child.gameObject.FindView<T>(id, recursive, parent);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a flag enum has a flag value set.
        /// </summary>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            // check if from the same type
            if (variable.GetType() != value.GetType())
            {
                Debug.LogError("[MarkUX.347] The checked flag is not from the same type as the checked variable.");
                return false;
            }

            Convert.ToUInt64(value);
            ulong num = Convert.ToUInt64(value);
            ulong num2 = Convert.ToUInt64(variable);
            return (num2 & num) == num;
        }

        /// <summary>
        /// Resets a rect transform.
        /// </summary>
        public static void Reset(this RectTransform rectTransform)
        {
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.localPosition = new Vector3(0f, 0f, 0f);
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = new Vector2(0.0f, 0.0f);
            rectTransform.offsetMax = new Vector2(0.0f, 0.0f);
        }

        /// <summary>
        /// Clamps a value to specified range [min, max].
        /// </summary>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Converts RGB color to HSV.
        /// </summary>
        public static ColorHsv ToHsv(this Color color)
        {
            ColorHsv hsv = new ColorHsv();
            float r = color.r;
            float g = color.g;
            float b = color.b;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            // calculate value
            hsv.Value = max;
            if (hsv.Value == 0)
            {
                // color is black
                hsv.Hue = 0;
                hsv.Saturation = 0;
                return hsv;
            }

            // normalize value to 1
            r = r / hsv.Value;
            g = g / hsv.Value;
            b = b / hsv.Value;
            max = Math.Max(r, Math.Max(g, b));
            min = Math.Min(r, Math.Min(g, b));

            // calculate saturation
            hsv.Saturation = max - min;
            if (hsv.Saturation == 0)
            {
                hsv.Hue = 0;
                return hsv;
            }

            // normalize saturation
            r = (r - min) / (max - min);
            g = (g - min) / (max - min);
            b = (b - min) / (max - min);
            max = Math.Max(r, Math.Max(g, b));
            min = Math.Min(r, Math.Min(g, b));

            // calculate hue
            if (max == r)
            {
                hsv.Hue = 60f * (g - b);
                if (hsv.Hue < 0)
                {
                    hsv.Hue += 360f;
                }
            }
            else if (max == g)
            {
                hsv.Hue = 120f + 60f * (b - r);
            }
            else if (max == b)
            {
                hsv.Hue = 240f + 60f * (r - g);
            }

            return hsv;
        }

        /// <summary>
        /// Converts RGB color to HSV.
        /// </summary>
        public static Color ToRgb(this ColorHsv hsv)
        {
            int hi = (int)Math.Floor(hsv.Hue / 60.0) % 6;
            float f = (float)(hsv.Hue / 60.0f - Math.Floor(hsv.Hue / 60.0));

            float p = (hsv.Value * (1f - hsv.Saturation));
            float q = (hsv.Value * (1f - f * hsv.Saturation));
            float t = (hsv.Value * (1f - (1f - f) * hsv.Saturation));

            if (hi == 0)
                return new Color(hsv.Value, t, p);
            else if (hi == 1)
                return new Color(q, hsv.Value, p);
            else if (hi == 2)
                return new Color(p, hsv.Value, t);
            else if (hi == 3)
                return new Color(p, q, hsv.Value);
            else if (hi == 4)
                return new Color(t, p, hsv.Value);
            else
                return new Color(hsv.Value, p, q);
        }

        /// <summary>
        /// Removes all whitespace from a string.
        /// </summary>
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Calculates mouse screen position.
        /// </summary>
        public static Vector2 GetMouseScreenPosition(this UnityEngine.Canvas canvas, Vector3 mousePosition)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);
            return mouseScreenPosition;
        }

        /// <summary>
        /// Calculates mouse screen position.
        /// </summary>
        public static Vector2 GetMouseScreenPosition(this UnityEngine.Canvas canvas, Vector2 mousePosition)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);
            return mouseScreenPosition;
        }

        #endregion
    }
}
