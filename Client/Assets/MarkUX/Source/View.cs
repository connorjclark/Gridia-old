#region Using Statements
using MarkUX.Animation;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Base class for view-models.
    /// </summary>
    public class View : MonoBehaviour
    {
        #region Fields

        public ViewAction EnabledChanged;

        [ChangeHandler("UpdateLayouts")]
        public ElementSize Width;
        public bool WidthSet;

        [ChangeHandler("UpdateLayouts")]
        public ElementSize Height;
        public bool HeightSet;

        [ChangeHandler("UpdateLayout")]
        public Alignment Alignment;
        public bool AlignmentSet;

        [ChangeHandler("UpdateLayout")]
        public Margin Margin;

        [ChangeHandler("UpdateLayout")]
        public Margin Offset;

        [ChangeHandler("UpdateLayout")]
        public Margin OffsetFromParent;

        [ChangeHandler("UpdateLayout")]
        public bool UpdateRectTransform;

        [ChangeHandler("UpdateLayout")]
        public bool UpdateBackground;

        [ChangeHandler("UpdateBehavior")]
        public Color BackgroundColor;
        public bool BackgroundColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Sprite BackgroundImage = null;

        [ChangeHandler("UpdateBehavior")]
        public UnityEngine.UI.Image.Type BackgroundImageType;

        [ChangeHandler("UpdateBehavior")]
        public Material BackgroundMaterial = null;

        [ChangeHandler("UpdateBehavior")]
        public bool PreserveAspect;

        [ChangeHandler("UpdateBehavior")]
        public bool Enabled;

        [ChangeHandler("UpdateBehavior")]
        public string Id;

        [ChangeHandler("UpdateBehavior")]
        public string Style;

        [ChangeHandler("UpdateBehavior")]
        public float Alpha;
        public bool AlphaSet;

        [ChangeHandler("UpdateBehavior")]
        public Vector3 Rotation;
        public bool RotationSet;

        [ChangeHandler("UpdateBehavior")]
        public Vector3 Scale;
        public bool ScaleSet;

        [ChangeHandler("UpdateBehavior")]
        public Vector2 Pivot;

        [ChangeHandler("UpdateBehavior")]
        public HideFlags HideFlags;

        [ChangeHandler("UpdateBehavior")]
        public bool AlwaysBlockRaycast;

        public int SortIndex;

        [NotSetFromXml]
        public bool ChildLayoutUpdated;

        [NotSetFromXml]
        public string Name;

        [NotSetFromXml]
        public GameObject Parent;

        [NotSetFromXml]
        public View ParentView;

        [NotSetFromXml]
        public GameObject LayoutParent;

        [NotSetFromXml]
        public View LayoutParentView;

        [NotSetFromXml]
        public bool IsLayoutRoot;

        [NotSetFromXml]
        public bool IsDestroyed;

        [NotSetFromXml]
        public List<ViewAction> ViewActions;

        [NotSetFromXml]
        public List<FieldBinding> FieldBindings;

        [NotSetFromXml]
        public List<FieldChangeHandler> FieldChangeHandlers;

        private Dictionary<string, ViewFieldData> _viewFieldData;
        private Dictionary<string, Dictionary<string, MethodInfo>> _changeHandlers; // using string and dictionaries as keys because windows phone 8 can't hash FieldInfo (and possibly MethodInfo) correctly
        private Dictionary<string, MethodInfo> _triggeredChangeHandlers;
        private HashSet<string> _changedFields;

        [NotSetFromXml]
        public bool IsDynamic;

        [NotSetFromXml]
        public GameObject RootCanvas;

        [NotSetFromXml]
        public ViewPresenter ParentViewPresenter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public View()
        {
            Alignment = Alignment.Center;
            Width = new ElementSize(1.0f, ElementSizeUnit.Percents);
            Height = new ElementSize(1.0f, ElementSizeUnit.Percents);
            Margin = new Margin();
            Offset = new Margin();
            BackgroundColor = Color.clear;
            BackgroundImageType = UnityEngine.UI.Image.Type.Simple;
            Enabled = true;
            ViewActions = new List<ViewAction>();
            OffsetFromParent = new Margin();
            UpdateRectTransform = true;
            UpdateBackground = true;
            FieldBindings = new List<FieldBinding>();
            FieldChangeHandlers = new List<FieldChangeHandler>();
            HideFlags = HideFlags.None;
            Pivot = new Vector2(0.5f, 0.5f);
            BackgroundImage = null;
            BackgroundMaterial = null;
            SortIndex = 0;
            AlwaysBlockRaycast = false;

            _changeHandlers = new Dictionary<string, Dictionary<string, MethodInfo>>();
            _triggeredChangeHandlers = new Dictionary<string, MethodInfo>();
            _changedFields = new HashSet<string>();
            _viewFieldData = new Dictionary<string, ViewFieldData>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cleans up.
        /// </summary>
        public void CleanUp()
        {
            if (IsDynamic)
            {
                // dynamic game objects removes all references
                FieldBindings.Clear();
                FieldChangeHandlers.Clear();
                DestroyImmediate(gameObject);
            }
            else
            {
                // remove references to dynamic field bindings
                FieldBindings.RemoveAll(x => x.IsDynamic);
            }
        }

        /// <summary>
        /// Sets the value of a field utilizing the binding and change tracking system.
        /// </summary>
        public void SetValue<TField>(Expression<Func<TField>> field, object value)
        {
            // get field data from expression string
            string expressionString = field.ToString();
            if (!_viewFieldData.ContainsKey(expressionString))
            {
                var viewFieldPath = expressionString.Substring(expressionString.IndexOf(").") + 2);
                var newViewFieldData = TypeHelper.GetViewFieldData(this, viewFieldPath);

                if (newViewFieldData == null)
                {
                    Debug.LogError(String.Format("[MarkUX.377] {0}: Unable to parse SetValue lambda view field path \"{1}\".", Name, viewFieldPath));
                    return;
                }

                _viewFieldData.Add(expressionString, newViewFieldData);
            }

            var viewFieldData = _viewFieldData[expressionString];
            
            FieldInfo classFieldInfo = viewFieldData.ClassFieldInfo;
            FieldInfo objectFieldInfo = viewFieldData.ObjectFieldInfo;
            object obj = viewFieldData.Object;

            // if object is view call SetValue on it
            if (obj is View)
            {
                (obj as View).SetValue(classFieldInfo, objectFieldInfo, obj, value, null, false, false, false);
            }
            else
            {
                SetValue(classFieldInfo, objectFieldInfo, obj, value, null, false, false, false);
            }
        }

        /// <summary>
        /// Sets the value of a field utilizing the binding and change tracking system.
        /// </summary>
        public void SetValue(FieldInfo classFieldInfo, FieldInfo objectFieldInfo, object obj, object inValue, List<KeyValuePair<View, FieldInfo>> callstack, bool onlySetChanged, bool forceSetValue, bool triggerChangeHandlersImmediately)
        {
            if (obj == null)
                return;

            object value = inValue;
            if (value != null)
            {
                var valueType = value.GetType();
                var fieldType = objectFieldInfo.FieldType;

                // are the types different?
                if (valueType != fieldType && !valueType.IsSubclassOf(fieldType))
                {
                    // yes. check if there are any value converters for the field
                    var valueConverter = objectFieldInfo.GetCustomAttributes(typeof(ValueConverter), true).FirstOrDefault() as ValueConverter;
                    if (valueConverter == null)
                    {
                        // check if there is a default value converter for type
                        valueConverter = ViewData.GetValueConverter(fieldType);
                    }

                    if (valueConverter == null)
                    {
                        Debug.LogError(String.Format("[MarkUX.306] Unable to assign value \"{0}\" to field \"{1}.{2}\". There is no converter to convert from {3} to {4}.", value, obj.GetType().Name, objectFieldInfo.Name, valueType.Name, fieldType.Name));
                        return;
                    }
                    else
                    {
                        // attempt to convert value to field type
                        var result = valueConverter.Convert(inValue, ValueConverterContext.Empty);
                        if (!result.Success)
                        {
                            Debug.LogError(String.Format("[MarkUX.307] Unable to assign value \"{0}\" to field \"{1}.{2}\". Value converion failed. {3}", value, obj.GetType().Name, objectFieldInfo.Name, result.ErrorMessage));
                            return;
                        }

                        value = result.ConvertedObject;
                    }
                }
            }

            // is the value different?
            object objectFieldCurrentValue = objectFieldInfo.GetValue(obj);
            bool valuesEqual = value != null ? value.Equals(objectFieldCurrentValue) : objectFieldCurrentValue == null;
            if (!forceSetValue && !onlySetChanged && valuesEqual)
            {
                // no. set the value set indicator and return
                SetValueSetIndicator(classFieldInfo);
                return;
            }

            // set field value
            if (!onlySetChanged)
            {
                try
                {
                    objectFieldInfo.SetValue(obj, value);
                }
                catch (Exception e)
                {
                    Debug.LogError(String.Format("[MarkUX.308] Unable to assign value \"{0}\" to field \"{1}.{2}\". Exception thrown: {3}", value, obj.GetType().Name, objectFieldInfo.Name, e.Message));
                    return;
                }

                // set the value-set indicator
                SetValueSetIndicator(classFieldInfo);
            }
            else
            {
                value = objectFieldCurrentValue;
            }

            // track changed fields
            if (!_changedFields.Contains(classFieldInfo.Name))
            {
                _changedFields.Add(classFieldInfo.Name);

                // does this field have change handlers?
                if (_changeHandlers.ContainsKey(classFieldInfo.Name))
                {
                    // yes. add them to set of triggered change handlers
                    foreach (var x in _changeHandlers[classFieldInfo.Name])
                    {
                        if (!_triggeredChangeHandlers.ContainsKey(x.Key))
                        {
                            _triggeredChangeHandlers.Add(x.Key, x.Value);
                        }
                    }
                }
            }

            // init callstack
            if (callstack == null)
            {
                callstack = new List<KeyValuePair<View, FieldInfo>>();
            }

            // get bounded fields
            var fieldBindings = FieldBindings.Where(x => x.SourceViewField == classFieldInfo &&
                x.SourceObject == obj && !callstack.Any(c => c.Key == x.TargetView && c.Value == x.TargetViewField));
            if (fieldBindings.Any())
            {
                // add this field to the callstack
                callstack.Add(new KeyValuePair<View, FieldInfo>(this, classFieldInfo));

                // set value of bounded fields
                foreach (var fieldBinding in fieldBindings)
                {
                    fieldBinding.PropagateValue(callstack);
                }
            }

            if (triggerChangeHandlersImmediately)
            {
                TriggerChangeHandlers();
            }
        }

        /// <summary>
        /// Sets indicator indicating if value has been set.
        /// </summary>
        private void SetValueSetIndicator(FieldInfo classFieldInfo)
        {
            FieldInfo setFieldInfo = GetType().GetField(classFieldInfo.Name + "Set");
            if (setFieldInfo != null)
            {
                setFieldInfo.SetValue(this, true);
            }
        }

        /// <summary>
        /// Gets boolean indicating if a field value has changed since last frame.
        /// </summary>
        public bool HasChanged<TField>(Expression<Func<TField>> field)
        {
            // get field data from expression string
            string expressionString = field.ToString();
            if (!_viewFieldData.ContainsKey(expressionString))
            {
                var viewFieldPath = expressionString.Substring(expressionString.IndexOf(").") + 2);
                var newViewFieldData = TypeHelper.GetViewFieldData(this, viewFieldPath);

                if (newViewFieldData == null)
                {
                    Debug.LogError(String.Format("[MarkUX.377] {0}: Unable to parse SetValue lambda view field path \"{1}\".", Name, viewFieldPath));
                    return false;
                }

                _viewFieldData.Add(expressionString, newViewFieldData);
            }

            var viewFieldData = _viewFieldData[expressionString];

            FieldInfo classFieldInfo = viewFieldData.ClassFieldInfo;
            FieldInfo objectFieldInfo = viewFieldData.ObjectFieldInfo;
            object obj = viewFieldData.Object;

            // if object is view, call HasChanged on it
            if (obj is View)
            {
                return (obj as View).HasChanged(objectFieldInfo.Name);
            }

            return HasChanged(classFieldInfo.Name);
        }

        /// <summary>
        /// Gets boolean indicating if a field value has changed since last frame.
        /// </summary>
        private bool HasChanged(string fieldName)
        {
            return _changedFields.Contains(fieldName);
        }

        /// <summary>
        /// Sets boolean indicating that a field value has changed since last frame.
        /// </summary>
        public void SetChanged<TField>(Expression<Func<TField>> field)
        {
            // get field data from expression string
            string expressionString = field.ToString();
            if (!_viewFieldData.ContainsKey(expressionString))
            {
                var viewFieldPath = expressionString.Substring(expressionString.IndexOf(").") + 2);
                var newViewFieldData = TypeHelper.GetViewFieldData(this, viewFieldPath);

                if (newViewFieldData == null)
                {
                    Debug.LogError(String.Format("[MarkUX.377] {0}: Unable to parse SetValue lambda view field path \"{1}\".", Name, viewFieldPath));
                    return;
                }

                _viewFieldData.Add(expressionString, newViewFieldData);
            }

            var viewFieldData = _viewFieldData[expressionString];

            FieldInfo classFieldInfo = viewFieldData.ClassFieldInfo;
            FieldInfo objectFieldInfo = viewFieldData.ObjectFieldInfo;
            object obj = viewFieldData.Object;

            // if object is view, call SetChanged on it
            if (obj is View)
            {
                (obj as View).SetValue(classFieldInfo, objectFieldInfo, obj, null, null, true, false, false);
            }
            else
            {
                SetValue(classFieldInfo, objectFieldInfo, obj, null, null, true, false, false);
            }
        }

        /// <summary>
        /// Updates layout and behavior of the view.
        /// </summary>
        public void UpdateView()
        {
            UpdateBehavior();
            UpdateLayout();
            UpdateBindings();
            TriggerChangeHandlers();
        }

        /// <summary>
        /// Updates the layout of the view and notifies parents of update.
        /// </summary>
        public void UpdateLayouts()
        {
            UpdateLayout();

            // inform parents of update
            gameObject.ForEachParent<View>(x => x.SetValue(() => x.ChildLayoutUpdated, true));
        }

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public virtual void UpdateLayout()
        {
            //Debug.Log(String.Format("{0}.UpdateLayout called", Name));

            if (UpdateRectTransform)
            {
                var rectTransform = GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // update rectTransform
                    // horizontal alignment and positioning
                    float xMin = 0f;
                    float xMax = 0f;
                    float offsetMinX = 0f;
                    float offsetMaxX = 0f;

                    if (Alignment.HasFlag(Alignment.Left))
                    {
                        xMin = 0f;
                        xMax = Width.Percent;
                        offsetMinX = 0f;
                        offsetMaxX = Width.Pixels;
                    }
                    else if (Alignment.HasFlag(Alignment.Right))
                    {
                        xMin = 1f - Width.Percent;
                        xMax = 1f;
                        offsetMinX = -Width.Pixels;
                        offsetMaxX = 0f;
                    }
                    else
                    {
                        xMin = 0.5f - Width.Percent / 2f;
                        xMax = 0.5f + Width.Percent / 2f;
                        offsetMinX = -Width.Pixels / 2f;
                        offsetMaxX = Width.Pixels / 2f;
                    }

                    // vertical alignment
                    float yMin = 0f;
                    float yMax = 0f;
                    float offsetMinY = 0f;
                    float offsetMaxY = 0f;

                    if (Alignment.HasFlag(Alignment.Top))
                    {
                        yMin = 1f - Height.Percent;
                        yMax = 1f;
                        offsetMinY = -Height.Pixels;
                        offsetMaxY = 0f;
                    }
                    else if (Alignment.HasFlag(Alignment.Bottom))
                    {
                        yMin = 0f;
                        yMax = Height.Percent;
                        offsetMinY = 0f;
                        offsetMaxY = Height.Pixels;
                    }
                    else
                    {
                        yMin = 0.5f - Height.Percent / 2f;
                        yMax = 0.5f + Height.Percent / 2f;
                        offsetMinY = -Height.Pixels / 2f;
                        offsetMaxY = Height.Pixels / 2f;
                    }

                    rectTransform.anchorMin = new Vector2(xMin, yMin);
                    rectTransform.anchorMax = new Vector2(xMax, yMax);

                    // positioning and margins
                    rectTransform.offsetMin = new Vector2(
                        offsetMinX + Margin.Left.Pixels + Offset.Left.Pixels - Offset.Right.Pixels + OffsetFromParent.Left.Pixels - OffsetFromParent.Right.Pixels,
                        offsetMinY + Margin.Bottom.Pixels - Offset.Top.Pixels + Offset.Bottom.Pixels - OffsetFromParent.Top.Pixels + OffsetFromParent.Bottom.Pixels);
                    rectTransform.offsetMax = new Vector2(
                        offsetMaxX - Margin.Right.Pixels + Offset.Left.Pixels - Offset.Right.Pixels + OffsetFromParent.Left.Pixels - OffsetFromParent.Right.Pixels,
                        offsetMaxY - Margin.Top.Pixels - Offset.Top.Pixels + Offset.Bottom.Pixels - OffsetFromParent.Top.Pixels + OffsetFromParent.Bottom.Pixels);

                    rectTransform.anchoredPosition = new Vector2(
                        rectTransform.offsetMin.x / 2.0f + rectTransform.offsetMax.x / 2.0f,
                        rectTransform.offsetMin.y / 2.0f + rectTransform.offsetMax.y / 2.0f);

                    // pivot
                    rectTransform.pivot = Pivot;
                }
            }

            // reset child-update flag
            ChildLayoutUpdated = false;
        }

        /// <summary>
        /// Updates the behavior and visual appearance of the view.
        /// </summary>
        public virtual void UpdateBehavior()
        {
            //Debug.Log(String.Format("{0}.UpdateBehavior called", Name));

            gameObject.hideFlags = HideFlags;

            // background image and color
            if (UpdateBackground)
            {
                SetBackground();
            }

            // set gameObject name to Id if set
            if (!String.IsNullOrEmpty(Id))
            {
                name = Id;
                Name = Id;
            }

            // set alpha
            var canvasGroup = GetComponent<CanvasGroup>();
            if (Enabled && canvasGroup != null)
            {
                if (canvasGroup.blocksRaycasts == false)
                {
                    // enabled has changed, trigger action
                    EnabledChanged.Trigger(new EnabledChangedActionData { Enabled = true });
                }

                canvasGroup.alpha = AlphaSet ? Alpha : 1;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }

            if (!Enabled)
            {
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                else if (canvasGroup.blocksRaycasts == true)
                {
                    // enabled has changed, trigger action
                    EnabledChanged.Trigger(new EnabledChangedActionData { Enabled = false });
                }

                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            if (AlphaSet)
            {                
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                                
                if (Enabled)
                {
                    canvasGroup.alpha = Alpha;
                    canvasGroup.blocksRaycasts = Alpha > 0;
                    canvasGroup.interactable = Alpha > 0;
                }
            }

            // update scale and rotation
            if (UpdateRectTransform)
            {
                var rectTransform = GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (ScaleSet)
                    {
                        rectTransform.localScale = Scale;
                    }

                    if (RotationSet)
                    {
                        rectTransform.rotation = Quaternion.Euler(Rotation);
                    }
                }
            }
        }

        /// <summary>
        /// Sets background image and color.
        /// </summary>
        protected void SetBackground()
        {
            SetBackground(null, null, Color.clear, false);
        }

        /// <summary>
        /// Sets background image and color.
        /// </summary>
        protected void SetBackground(Sprite inBackgroundImage, Material inBackgroundMaterial, Color inBackgroundColor, bool inBackgroundColorSet)
        {
            var image = GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                var backgroundImage = inBackgroundImage != null ? inBackgroundImage : BackgroundImage;
                var backgroundMaterial = inBackgroundMaterial != null ? inBackgroundMaterial : BackgroundMaterial;
                var backgroundColor = inBackgroundColorSet ? inBackgroundColor : BackgroundColor;
                var backgroundColorSet = inBackgroundColorSet ? true : BackgroundColorSet;

                if (backgroundImage != null)
                {
                    image.sprite = backgroundImage;
                    image.type = BackgroundImageType;
                    image.color = backgroundColorSet ? backgroundColor : Color.white;
                }
                else
                {
                    image.color = backgroundColorSet ? backgroundColor : Color.clear;
                }

                if (backgroundMaterial != null)
                {
                    image.material = backgroundMaterial;
                }

                // if image color is clear disable image component
                image.enabled = AlwaysBlockRaycast ? true : image.color.a > 0;
                image.preserveAspect = PreserveAspect;
            }
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public virtual string GetEmbeddedXml()
        {
            return String.Empty;
        }

        /// <summary>
        /// Updates the view. Called once per frame in reverse breadth-first order.
        /// </summary>
        public virtual void LateUpdate()
        {
            TriggerChangeHandlers();
        }

        /// <summary>
        /// Called once per frame or when a view gets deactivated (because LateUpdate() isn't called on inactive game objects).
        /// </summary>
        public void TriggerChangeHandlers()
        {
            if (_triggeredChangeHandlers.Count > 0)
            {
                // call changed field handlers
                foreach (var triggeredHandler in _triggeredChangeHandlers.Values)
                {
                    //Debug.Log(String.Format("Triggered change handler \"{0}.{1}\".", Name, triggeredHandler.Name));
                    triggeredHandler.Invoke(this, null);
                }

                _triggeredChangeHandlers.Clear();
            }

            if (_changedFields.Count > 0)
            {
                _changedFields.Clear();
            }
        }

        /// <summary>
        /// Initializes the view and its children.
        /// </summary>
        public void InitializeViews()
        {
            InitializeInternal();
            Initialize();
            this.ForEachChild<View>(x => { x.InitializeInternal(); x.Initialize(); });
        }

        /// <summary>
        /// Updates the view and its children (reverse breadth first).
        /// </summary>
        public void UpdateViews()
        {
            this.ForEachChild<View>(x => x.UpdateView(), true, null, SearchAlgorithm.ReverseBreadthFirst);
            UpdateView();
        }

        /// <summary>
        /// Initializes the view. Called automatically by the view engine Awake() unless the view is created dynamically.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Initializes the view. Called automatically by the view engine Awake() unless the view is created dynamically.
        /// </summary>
        internal void InitializeInternal()
        {
            // hook up event system triggers
            var eventTrigger = GetComponent<EventTrigger>();
            if (eventTrigger != null)
            {
#if UNITY_4_6 || UNITY_5_0
                var triggers = eventTrigger.delegates;
#else
                var triggers = eventTrigger.triggers;
#endif                

                if (triggers != null)
                {
                    triggers.Clear();
                    foreach (var viewAction in ViewActions.Where(a => a.TriggeredByEventSystem))
                    {
                        var entry = new EventTrigger.Entry();
                        entry.eventID = viewAction.EventTriggerType;
                        entry.callback = new EventTrigger.TriggerEvent();

                        var eventViewAction = viewAction;
                        var action = new UnityAction<BaseEventData>(eventData => eventViewAction.Trigger(eventData));
                        entry.callback.AddListener(action);

                        triggers.Add(entry);
                    }
                }
            }

            // init field change handlers
            InitFieldChangeHandlers();
        }

        /// <summary>
        /// Creates a view of specified type.
        /// </summary>
        public static T CreateView<T>(GameObject layoutParent, View parent, string style = null) where T : View
        {
            // find view template
            Type viewType = typeof(T);
            var viewTemplate = parent.ParentViewPresenter.GetViewTemplate(viewType, style);

            if (viewTemplate == null)
            {
                Debug.LogError(String.Format("[MarkUX.309] CreateView<{0}>() called for view type that has no template. Are you a missing a [CreatesView(typeof({0}))] attribute on the view?", viewType.Name));
                return null;
            }

            return CreateViewFromTemplate(viewTemplate, layoutParent, parent) as T;
        }

        /// <summary>
        /// Creates a view from specified template.
        /// </summary>
        public static View CreateViewFromTemplate(GameObject template, GameObject layoutParent, View parent)        
        {
            // instantiate template
            var go = Instantiate(template) as GameObject;

            // make the item temporary
            go.hideFlags = HideFlags.DontSave;

            // set layout parent
            go.transform.SetParent(layoutParent.transform, false);
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.Reset();

            // set view parent
            var view = go.GetComponent<View>();
            view.Parent = parent != null ? parent.gameObject : null;
            view.IsDynamic = true;
            go.name = view.Name;

            return view;
        }

        /// <summary>
        /// Creates view from prefab.
        /// </summary>
        /// <param name="viewPrefab">View prefab.</param>
        /// <param name="layoutParent">Parent in layout.</param>
        /// <param name="parent">View parent.</param>
        private static GameObject CreateView(Transform viewPrefab, GameObject layoutParent, View parent)
        {
            // instantiate the template
            var transform = Instantiate(viewPrefab) as Transform;
            var go = transform.gameObject;

            // make the item temporary
            go.hideFlags = HideFlags.DontSave;

            // set layout parent
            go.transform.SetParent(layoutParent.transform, false);
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.Reset();

            // set view parent
            var view = go.GetComponent<View>();
            view.Parent = parent != null ? parent.gameObject : null;
            view.IsDynamic = true;
            go.name = view.Name;

            // initialize view
            view.InitializeViews();
            view.UpdateViews();

            return go;
        }

        /// <summary>
        /// Initializes field change handlers.
        /// </summary>
        private void InitFieldChangeHandlers()
        {
            _changeHandlers.Clear();
            foreach (var fieldChangeHandler in FieldChangeHandlers)
            {
                if (fieldChangeHandler.FieldInfo == null)
                {
                    Debug.LogError(String.Format("[MarkUX.310] {0}: Field \"{1}\" missing for field change handler \"{2}\".", Name, fieldChangeHandler.FieldName, fieldChangeHandler.ChangeHandlerName));
                    continue;
                }

                if (!_changeHandlers.ContainsKey(fieldChangeHandler.FieldInfo.Name))
                {
                    _changeHandlers.Add(fieldChangeHandler.FieldInfo.Name, new Dictionary<string, MethodInfo>());
                }

                if (fieldChangeHandler.ChangeHandler != null)
                {
                    if (!_changeHandlers[fieldChangeHandler.FieldInfo.Name].ContainsKey(fieldChangeHandler.ChangeHandlerName))
                    {
                        _changeHandlers[fieldChangeHandler.FieldInfo.Name].Add(fieldChangeHandler.ChangeHandlerName, fieldChangeHandler.ChangeHandler);
                    }
                }
            }
        }

        /// <summary>
        /// Activates view.
        /// </summary>
        public void Activate(bool immediate = false)
        {
            if (immediate)
            {
                // set alpha
                var canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = AlphaSet ? Alpha : 1;
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                }

                EnabledChanged.Trigger(new EnabledChangedActionData { Enabled = true });
            }

            SetValue(() => Enabled, true);
        }

        /// <summary>
        /// Deactivates view. Turns off the renderer.
        /// </summary>
        public void Deactivate(bool immediate = false)
        {
            if (immediate)
            {
                // set alpha
                var canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;

                EnabledChanged.Trigger(new EnabledChangedActionData { Enabled = false });
            }

            SetValue(() => Enabled, false);
        }

        /// <summary>
        /// Update bindings.
        /// </summary>
        internal void UpdateBindings()
        {
            foreach (var fieldBinding in FieldBindings)
            {
                fieldBinding.Initialize();

                if (fieldBinding.Update)
                {
                    // propagate value from source to target
                    fieldBinding.PropagateValue();
                }
            }
        }
             
        #endregion

        #region Properties

        /// <summary>
        /// Gets actual width of view in pixels. Useful when Width may be specified as percentage and you want actual pixel width.
        /// </summary>
        public float ActualWidth
        {
            get
            {
                var rectTransform = GetComponent<RectTransform>();
                return Mathf.Abs(rectTransform.rect.width);
            }
        }

        /// <summary>
        /// Gets actual height of view in pixels. Useful when Height may be specified as percentage and you want actual pixel height.
        /// </summary>
        public float ActualHeight
        {
            get
            {
                var rectTransform = GetComponent<RectTransform>();
                return Mathf.Abs(rectTransform.rect.height);
            }
        }

        #endregion
    }
}
