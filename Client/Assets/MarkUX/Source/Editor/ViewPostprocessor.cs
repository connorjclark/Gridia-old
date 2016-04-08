#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Xml.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MarkUX.ValueConverters;
using System.Text.RegularExpressions;
using MarkUX.Views;
#endregion

namespace MarkUX.Editor
{
    /// <summary>
    /// Processes view XML assets that are added, deleted, updated, etc. and generates the UI objects.
    /// </summary>
    internal class ViewPostprocessor : AssetPostprocessor
    {
        #region Fields
        #endregion

        #region Methods

        /// <summary>
        /// Processes the view XML assets that are added, deleted, updated, etc. and generates the UI objects.
        /// </summary>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var configuration = Configuration.Instance;

            // check if any view XML is added, moved, updated or deleted
            bool viewAssetsUpdated = false;
            foreach (var path in importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedFromAssetPaths))
            {
                if (configuration.ViewPaths.Any(x => path.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    path.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    viewAssetsUpdated = true;
                    break;
                }
            }

            // any views updated? 
            if (!viewAssetsUpdated)
            {
                return; // no.
            }

            // process view assets
            ProcessViewAssets();
        }

        /// <summary>
        /// Processes view assets in the project.
        /// </summary>
        public static void ProcessViewAssets()
        {
            // don't process assets while playing
            if (Application.isPlaying)
            {
                Debug.LogWarning("[MarkUX.351] View assets are not processed while application is playing. Reload views by stopping the application and pressing \"Reload Views\" on the View Presenter or by making changes to a view and saving it.");
                return;
            }

            // get all view XML assets and view presenters
            var viewAssets = GetAllViewAssets();
            var viewPresenters = UnityEngine.Object.FindObjectsOfType(typeof(ViewPresenter)).Cast<ViewPresenter>().ToList();

            // do we have any view presenters in the scene?
            if (!viewPresenters.Any())
            {
                return; // no.
            }

            // find view types and value converters
            var viewTypes = TypeHelper.FindDerivedTypes(typeof(View));
            var valueConverters = new List<ValueConverter>();
            foreach (var valueConverterType in TypeHelper.FindDerivedTypes(typeof(ValueConverter)))
            {
                valueConverters.Add(TypeHelper.CreateInstance(valueConverterType) as ValueConverter);
            }

            // find field change handlers for each viewtype
            var viewFieldChangeHandlers = new Dictionary<Type, List<FieldChangeHandler>>();
            foreach (var viewType in viewTypes)
            {
                // look for methods ending in "HasChanged"
                var fieldChangeHandlers = new List<FieldChangeHandler>();
                var methods = viewType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var method in methods.Where(x => x.Name.EndsWith("HasChanged") && x.Name.Length > 10))
                {
                    var fieldName = method.Name.Substring(0, method.Name.Length - 10);
                    fieldChangeHandlers.Add(new FieldChangeHandler(fieldName, method.Name));
                }

                // check for change handlers set by field attributes
                var fields = viewType.GetFields();
                foreach (var field in fields)
                {
                    var changeHandlers = field.GetCustomAttributes(typeof(ChangeHandler), true);
                    foreach (ChangeHandler changeHandler in changeHandlers)
                    {
                        fieldChangeHandlers.Add(new FieldChangeHandler(field.Name, changeHandler.ChangeHandlerName));
                    }
                }

                viewFieldChangeHandlers.Add(viewType, fieldChangeHandlers);
            }

            ViewData.Initialize(viewPresenters[0].ElementSize);

            // parse the view XML assets and create view and theme elements
            var viewElements = new List<ViewElement>();
            var themeElements = new Dictionary<string, List<ThemeElement>>();
            var themes = new List<string>();
            foreach (var viewAsset in viewAssets)
            {
                XElement element = null;
                try
                {
                    element = XElement.Parse(viewAsset.text);
                }
                catch (Exception e)
                {
                    Debug.LogError(String.Format("[MarkUX.329] {0}: Error parsing view asset. Exception thrown: {1}.", viewAsset.name, e.Message));
                    return;
                }

                if (String.Equals(element.Name.LocalName, "Theme", StringComparison.OrdinalIgnoreCase))
                {
                    var themeNameAttr = element.Attribute("Name");
                    if (themeNameAttr != null)
                    {
                        var themeName = themeNameAttr.Value;
                        if (!themes.Contains(themeName))
                        {
                            themes.Add(themeName);
                        }

                        if (!themeElements.ContainsKey(themeName))
                        {
                            themeElements.Add(themeName, new List<ThemeElement>());
                        }

                        themeElements[themeName].AddRange(CreateThemeElementsFromXml(viewAsset.name, element, viewTypes));
                    }
                }
                else
                {
                    var viewElement = CreateViewElementFromXml(viewAsset.name, element, viewTypes);
                    viewElements.Add(viewElement);
                }
            }

            // create temporary game object that view components can be added to
            var temporaryGo = new GameObject("MarkUX_Temporary");
            temporaryGo.hideFlags = HideFlags.HideAndDontSave;

            // parse embedded view xml and create view elements
            foreach (var viewType in viewTypes)
            {
                // ignore types that already has view elements
                if (viewElements.Any(x => String.Equals(x.Name, viewType.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var temporaryViewComponent = temporaryGo.AddComponent(viewType) as View;

                //var v = TypeHelper.CreateInstance(viewType) as View;
                var embeddedXml = temporaryViewComponent.GetEmbeddedXml();
                if (!String.IsNullOrEmpty(embeddedXml))
                {
                    XElement embeddedElement = null;
                    try
                    {
                        embeddedElement = XElement.Parse(embeddedXml);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(String.Format("[MarkUX.330] Error parsing embedded XML in view \"{0}\". Exception thrown: {1}", viewType.Name, e.Message));
                        GameObject.DestroyImmediate(temporaryViewComponent);
                        return;
                    }

                    viewElements.Add(CreateViewElementFromXml(viewType.Name, embeddedElement, viewTypes));
                }

                GameObject.DestroyImmediate(temporaryViewComponent);
            }

            // remove temporary game object
            GameObject.DestroyImmediate(temporaryGo);

            // set view element dependencies
            foreach (var viewElement in viewElements)
            {
                foreach (var dependencyName in viewElement.DependencyNames)
                {
                    try
                    {
                        viewElement.Dependencies.Add(viewElements.Where(x => String.Equals(x.Name, dependencyName, StringComparison.OrdinalIgnoreCase)).First());
                    }
                    catch
                    {
                        Debug.LogError(String.Format("[MarkUX.331] {0}: The view \"{1}\" could not be found.", viewElement.AssetName, dependencyName));
                        return;
                    }
                }
            }

            // sort view elements by dependencies
            try
            {
                viewElements = SortByDependency(viewElements);
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("[MarkUX.332] Unable to parse views. {0}", e.Message));
                return;
            }

            // process views for each view presenter
            foreach (var viewPresenter in viewPresenters)
            {
                ProcessViewAssetsForViewPresenter(viewPresenter, viewTypes, valueConverters, viewFieldChangeHandlers,
                    viewElements, themes, themeElements);
            }

            Debug.Log("[MarkUX] Views processed. " + DateTime.Now.ToString());
        }
               
        /// <summary>
        /// Processes view assets for a view presenter.
        /// </summary>
        private static void ProcessViewAssetsForViewPresenter(ViewPresenter viewPresenter, IEnumerable<Type> viewTypes, List<ValueConverter> valueConverters, Dictionary<Type, List<FieldChangeHandler>> viewFieldChangeHandlers, List<ViewElement> viewElements, List<string> themes, Dictionary<string, List<ThemeElement>> themeElements)
        {
            // Parses the views in the following order
            // ParseView(view)
            //   Foreach child
            //     ParseView(child)
            //     SetViewValues(child)
            //   Foreach contentView
            //      ParseView(contentView)
            //      SetViewValues(contentView)
            //   SetViewValues(view)

            // set list of views, themes and prefabs in view presenter
            viewPresenter.Views.Clear();
            viewPresenter.Themes.Clear();

            viewPresenter.Views.AddRange(viewElements.Where(y => !y.IsInternal).Select(x => x.Name).OrderBy(x => x));            
            viewPresenter.Themes.AddRange(themes.OrderBy(x => x));
             
            var oldLayoutRoot = viewPresenter.transform.Find("LayoutRoot");
            if (oldLayoutRoot != null)
            {
                // destroy old layout-root and add to undo manager
                if (!viewPresenter.DisableUndo)
                {
                    Undo.DestroyObjectImmediate(oldLayoutRoot.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(oldLayoutRoot.gameObject);
                }
            }

            // create layout root
            var layoutRootGo = CreateLayoutRoot(viewPresenter);
            viewPresenter.LayoutRoot = layoutRootGo;

            // add to undo manager
            if (!viewPresenter.DisableUndo)
            {
                Undo.RegisterCreatedObjectUndo(layoutRootGo, "Generated LayoutRoot");
            }

            // create template root
            var templateRootGo = CreateTemplateRoot(layoutRootGo);
            viewPresenter.TemplateRoot = templateRootGo;            

            // create the main view game objects
            var mainViewElement = viewElements.FirstOrDefault(x => String.Equals(x.Name, viewPresenter.MainView, StringComparison.OrdinalIgnoreCase));
            View mainView = null;
            if (mainViewElement != null)
            {
                var selectedThemeElements = themeElements.ContainsKey(viewPresenter.Theme) ? themeElements[viewPresenter.Theme] : new List<ThemeElement>();
                mainView = CreateViewGameObject(viewPresenter, mainViewElement, viewElements, selectedThemeElements, layoutRootGo, valueConverters, viewFieldChangeHandlers, null, null, String.Empty, String.Empty);
            }
            else
            {
                if (String.IsNullOrEmpty(viewPresenter.MainView))
                {
                    Debug.LogWarning(String.Format("[MarkUX.352] Main view not selected. No view is rendered by the view presenter."));
                    return;
                }
                else
                {
                    Debug.LogError(String.Format("[MarkUX.335] Unable to find main view \"{0}\".", viewPresenter.MainView));
                    return;
                }
            }

            // initialize and update layout
            try
            {
                mainView.InitializeViews();
                mainView.UpdateViews();
                mainView.UpdateViews(); // update twice
                mainView.CleanUp();
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("[MarkUX.336] Unable to initialize views. Exception thrown: \"{0}\". {1}", e.Message, e.StackTrace));
                return;
            }

            // create layout grid 
            if (viewPresenter.DrawGrid)
            {
                CreateLayoutGrid(viewPresenter, layoutRootGo);
            }
        }

        /// <summary>
        /// Creates layout root.
        /// </summary>
        private static GameObject CreateLayoutRoot(ViewPresenter viewPresenter)
        {
            var layoutRootGo = new GameObject("LayoutRoot");
            layoutRootGo.layer = LayerMask.NameToLayer(Configuration.Instance.UILayer);

            var rectTransform = layoutRootGo.AddComponent<RectTransform>();
            layoutRootGo.transform.SetParent(viewPresenter.transform, false);
            rectTransform.Reset();

            var layoutRootView = layoutRootGo.AddComponent<View>();
            layoutRootView.IsLayoutRoot = true;
            return layoutRootGo;
        }

        /// <summary>
        /// Creates template root.
        /// </summary>
        private static GameObject CreateTemplateRoot(GameObject layoutRoot)
        {
            var templateRootGo = new GameObject("TemplateRoot");
            templateRootGo.layer = LayerMask.NameToLayer(Configuration.Instance.UILayer);

            var rectTransform = templateRootGo.AddComponent<RectTransform>();
            templateRootGo.transform.SetParent(layoutRoot.transform, false);
            rectTransform.Reset();
            templateRootGo.SetActive(false);
            templateRootGo.hideFlags = HideFlags.HideInHierarchy;

            var templateRootView = templateRootGo.AddComponent<View>();
            templateRootView.IsLayoutRoot = true;
            templateRootView.HideFlags = HideFlags.HideInHierarchy;
            templateRootView.Enabled = false;
            return templateRootGo;
        }

        /// <summary>
        /// Creates a game object for a view.
        /// </summary>
        private static View CreateViewGameObject(ViewPresenter viewPresenter, ViewElement viewElement, List<ViewElement> viewElements, List<ThemeElement> themeElements, GameObject layoutParent, List<ValueConverter> valueConverters, Dictionary<Type, List<FieldChangeHandler>> viewFieldChangeHandlers, IEnumerable<XElement> content, GameObject contentParent, string viewId, string viewStyle)
        {
            // Parses the views in the following order:
            // ParseView(view)
            //   Foreach child
            //     ParseView(child)
            //     SetViewValues(child)
            //   Foreach contentView
            //      ParseView(contentView)
            //      SetViewValues(contentView)
            //   SetViewValues(view)
            //   Set values depending on view values being set

            // create view game object with required components
            var go = new GameObject(viewElement.Name);
            var rectTransform = go.AddComponent<RectTransform>();
            go.transform.SetParent(layoutParent.transform, false);
            rectTransform.Reset();

            go.layer = LayerMask.NameToLayer(Configuration.Instance.UILayer);

            // add image component
            go.AddComponent<UnityEngine.UI.Image>();

            // remove components based on attributes set
            foreach (RemoveComponent removeComponentAttribute in viewElement.ViewType.GetCustomAttributes(typeof(RemoveComponent), true))
            {
                var component = go.GetComponent(removeComponentAttribute.ComponentType);
                if (component != null)
                {
                    GameObject.DestroyImmediate(go.GetComponent(removeComponentAttribute.ComponentType));
                }
            }

            // add components based on attributes set
            foreach (AddComponent addComponentAttribute in viewElement.ViewType.GetCustomAttributes(typeof(AddComponent), true))
            {
                var component = go.GetComponent(addComponentAttribute.ComponentType);
                if (component == null)
                {
                    go.AddComponent(addComponentAttribute.ComponentType);
                }
            }

            // create view behavior
            var view = go.AddComponent(viewElement.ViewType) as View;
            view.Name = viewElement.Name;            

            if (contentParent != null)
            {
                view.Parent = contentParent;
                view.ParentView = contentParent.GetComponent<View>();
            }

            view.RootCanvas = viewPresenter.gameObject;
            view.LayoutParent = layoutParent;
            view.LayoutParentView = layoutParent.GetComponent<View>();
            view.ParentViewPresenter = viewPresenter;

            // does this view create other views?            
            foreach (CreatesView createsView in viewElement.ViewType.GetCustomAttributes(typeof(CreatesView), true))
            {
                // see if a template exists
                var template = viewPresenter.GetViewTemplate(createsView.Type, createsView.Style);                
                if (template != null)
                    continue;

                // .. create new template
                var templateViewElement = viewElements.FirstOrDefault(e => e.ViewType == createsView.Type);
                if (templateViewElement == null)
                {
                    Debug.LogError(String.Format("[MarkUX.333] CreateView attribute on view \"{0}\" references a view template of type \"{1}\" that could not be found.", templateViewElement.ViewType.Name, createsView.Type.Name));
                    continue;
                }

                var templateView = CreateViewGameObject(viewPresenter, templateViewElement, viewElements, themeElements, viewPresenter.TemplateRoot, valueConverters, viewFieldChangeHandlers, null, null, String.Empty, createsView.Style);

                // set style 
                templateView.Style = createsView.Style;
            }         

            // initiate view actions
            EventTrigger eventTrigger = null;
            foreach (var viewActionField in viewElement.ViewActionFields)
            {
                // instantiate view action
                var field = viewElement.ViewType.GetField(viewActionField.Name);
                var viewAction = new ViewAction(go, viewActionField.Name);
                field.SetValue(view, viewAction);
                view.ViewActions.Add(viewAction);

                // check if view action is triggered by EventSystem events
                if (!ViewAction.EventTriggerTypes.ContainsKey(viewActionField.Name))
                    continue;

                viewAction.TriggeredByEventSystem = true;
                viewAction.EventTriggerType = ViewAction.EventTriggerTypes[viewActionField.Name];
                if (eventTrigger == null)
                {
                    eventTrigger = go.AddComponent<EventTrigger>();
                }
            }

            // initiate view change handlers
            foreach (var changeHandler in viewFieldChangeHandlers[viewElement.ViewType])
            {
                view.FieldChangeHandlers.Add(new FieldChangeHandler(changeHandler.FieldName, changeHandler.ChangeHandlerName, go));
            }

            // parse and create objects for elements in view
            foreach (var childElement in viewElement.Element.Elements())
            {
                var childViewElement = viewElements.First(x => String.Equals(childElement.Name.LocalName, x.Name, StringComparison.OrdinalIgnoreCase));

                var childViewIdAttr = childElement.Attribute("Id");
                var childViewStyleAttr = childElement.Attribute("Style");

                // create object for child
                var childView = CreateViewGameObject(viewPresenter, childViewElement, viewElements, themeElements, go, valueConverters, viewFieldChangeHandlers, childElement.Elements(), go,
                    childViewIdAttr != null ? childViewIdAttr.Value : String.Empty, childViewStyleAttr != null ? childViewStyleAttr.Value : String.Empty);

                // set child view values
                foreach (var attribute in childElement.Attributes())
                {
                    SetViewValue(childView, attribute.Name.LocalName, attribute.Value, valueConverters, childViewElement, go, null);
                }
            }

            // do we have content?            
            if (content != null && content.Count() > 0)
            {
                // is this a content-view?
                if (view is ContentView)
                {
                    // yes. is this a template-view?
                    GameObject contentLayoutParent = null;
                    if (view is TemplateView)
                    {
                        var templateView = view as TemplateView;

                        // add template object
                        var templateGoView = CreateViewGameObject(viewPresenter, viewElements.Where(x => x.ViewType == templateView.GetTemplateViewType()).FirstOrDefault(),
                            viewElements, themeElements, go, valueConverters, viewFieldChangeHandlers, null, go, String.Empty, String.Empty);
                        var templateGo = templateGoView.gameObject;

                        templateGoView.Enabled = false;
                        templateGo.name = "Template";

                        templateView.Template = templateGo;
                        contentLayoutParent = templateGo;
                    }
                    else
                    {
                        contentLayoutParent = (view as ContentView).ContentContainer;
                        if (contentLayoutParent == null)
                        {
                            contentLayoutParent = go;
                        }
                        else
                        {
                            // should the content container itself should be included in content?
                            var contentContainer = contentLayoutParent.GetComponent<ContentContainer>();
                            if (contentContainer != null && !contentContainer.IncludeContainerInContent)
                            {
                                // no. set the layout parent to the content container's parent and destroy the container
                                contentLayoutParent = contentContainer.transform.parent.gameObject;
                                GameObject.DestroyImmediate(contentContainer.gameObject);
                            }
                        }
                    }

                    // create object(s) for content
                    foreach (var contentElement in content)
                    {
                        var contentViewElement = viewElements.First(x => String.Equals(contentElement.Name.LocalName, x.Name, StringComparison.OrdinalIgnoreCase));

                        var contentElementIdAttr = contentElement.Attribute("Id");
                        var contentElementStyleAttr = contentElement.Attribute("Style");

                        // create object for content
                        var contentView = CreateViewGameObject(viewPresenter, contentViewElement, viewElements, themeElements, contentLayoutParent, valueConverters, viewFieldChangeHandlers, contentElement.Elements(), contentParent,
                            contentElementIdAttr != null ? contentElementIdAttr.Value : String.Empty, contentElementStyleAttr != null ? contentElementStyleAttr.Value : String.Empty);

                        // set content view values
                        foreach (var attribute in contentElement.Attributes())
                        {
                            SetViewValue(contentView, attribute.Name.LocalName, attribute.Value, valueConverters, contentViewElement, contentParent, null);
                        }
                    }
                }
                else
                {
                    Debug.LogError(String.Format("[MarkUX.337] {0}: Content assigned to view that isn't a content view (does not inherit from ContentView).", viewElement.AssetName));
                }
            }

            // set view values
            // NOTE: these are the values that appear in the file itself, e.g. values internal to the view />
            foreach (var attribute in viewElement.Element.Attributes())
            {
                SetViewValue(view, attribute.Name.LocalName, attribute.Value, valueConverters, viewElement, go, null);
            }

            // set theme values
            foreach (var themeElement in themeElements.Where(x => x.ViewType == viewElement.ViewType))
            {
                // filter by Id
                if (!String.IsNullOrEmpty(themeElement.Id) && !String.Equals(themeElement.Id, viewId, StringComparison.OrdinalIgnoreCase))
                    continue;

                // filter by style
                if (!String.IsNullOrEmpty(themeElement.Style) && !String.Equals(themeElement.Style, viewStyle, StringComparison.OrdinalIgnoreCase))
                    continue;

                // filter by parent view type
                if (themeElement.ParentViewType != null && contentParent != null && contentParent.GetComponent<View>().GetType() != themeElement.ParentViewType)
                    continue;

                // we have a match - set theme values
                foreach (var kv in themeElement.Values)
                {
                    SetViewValue(view, kv.Key, kv.Value, valueConverters, viewElement, go, themeElement.BaseDirectory);
                }
            }

            // set references to other views/gameobjects
            foreach (var field in viewElement.ViewType.GetFields())
            {
                // is this a reference to a view?
                if (field.FieldType.IsSubclassOf(typeof(View)) || field.FieldType == typeof(View))
                {
                    // yes. find view in the gameObject hierarchy
                    go.ForEachChild<View>(x =>
                    {
                        if (x.Parent == go && String.Equals(x.Id, field.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            view.SetValue(field, field, view, x, null, false, true, false);
                        }
                    });
                }

                // is this a reference to a gameobject?
                else if (field.FieldType == typeof(GameObject))
                {
                    // yes. find gameobject in hierarchy
                    go.ForEachChild<View>(x =>
                    {
                        if (x.Parent == go && String.Equals(x.Id, field.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            view.SetValue(field, field, view, x.gameObject, null, false, true, false);
                        }
                    });
                }
            }

            return view;
        }

        /// <summary>
        /// Parses XML and creates a view element.
        /// </summary>
        private static ViewElement CreateViewElementFromXml(string assetName, XElement element, IEnumerable<Type> viewTypes)
        {
            var viewElement = new ViewElement();
            viewElement.Element = element;
            viewElement.Name = element.Name.LocalName;
            viewElement.AssetName = assetName;

            // set dependency names
            foreach (var descendant in element.Descendants())
            {
                if (!viewElement.DependencyNames.Contains(descendant.Name.LocalName, StringComparer.OrdinalIgnoreCase))
                {
                    viewElement.DependencyNames.Add(descendant.Name.LocalName);
                }
            }

            // set view type
            var type = viewTypes.FirstOrDefault(t => String.Equals(t.Name, viewElement.Name, StringComparison.OrdinalIgnoreCase));
            if (type == null)
            {
                type = typeof(View);
            }
            viewElement.ViewType = type;

            // set view action types
            var fields = type.GetFields();
            foreach (var field in fields.Where(x => x.FieldType == typeof(ViewAction)))
            {
                viewElement.ViewActionFields.Add(field);
            }

            // set if view is internal
            if (type.GetCustomAttributes(typeof(InternalView), false).Any())
            {
                viewElement.IsInternal = true;
            }

            return viewElement;
        }

        /// <summary>
        /// Parses XML and creates theme elements.
        /// </summary>
        private static IEnumerable<ThemeElement> CreateThemeElementsFromXml(string assetName, XElement element, IEnumerable<Type> viewTypes)
        {
            var themeElements = new List<ThemeElement>();

            var baseDirectoryAttr = element.Attribute("BaseDirectory");
            var baseDirectory = baseDirectoryAttr != null ? baseDirectoryAttr.Value : String.Empty;

            // parse and create objects for elements in theme
            bool isInvalidThemeElement = false;
            foreach (var childElement in element.Elements())
            {
                // create theme element                
                var themeElement = new ThemeElement();
                var type = viewTypes.FirstOrDefault(t => String.Equals(t.Name, childElement.Name.LocalName, StringComparison.OrdinalIgnoreCase));
                if (type == null)
                {
                    Debug.LogError(String.Format("[MarkUX.338] {0}: unable to find view type \"{1}\".", assetName, childElement.Name.LocalName));
                    continue;
                }
                themeElement.ViewType = type;
                themeElement.BaseDirectory = baseDirectory;

                // set values
                foreach (var attribute in childElement.Attributes())
                {
                    var attributeName = attribute.Name.LocalName;
                    if (String.Equals(attributeName, "Id", StringComparison.OrdinalIgnoreCase))
                    {
                        themeElement.Id = attribute.Value;
                    }
                    else if (String.Equals(attributeName, "Style", StringComparison.OrdinalIgnoreCase))
                    {
                        themeElement.Style = attribute.Value;
                    }
                    else if (String.Equals(attributeName, "ParentViewType", StringComparison.OrdinalIgnoreCase))
                    {
                        var parentViewType = viewTypes.FirstOrDefault(t => String.Equals(t.Name, attribute.Value, StringComparison.OrdinalIgnoreCase));
                        if (parentViewType == null)
                        {
                            Debug.LogError(String.Format("[MarkUX.339] {0}: {1}: unable to find parent view type \"{2}\".", assetName, childElement.Name.LocalName, attribute.Value));
                            isInvalidThemeElement = true;
                            break;
                        }

                        themeElement.ParentViewType = parentViewType;
                    }
                    else
                    {
                        themeElement.Values.Add(attributeName, attribute.Value);
                    }
                }

                if (isInvalidThemeElement)
                {
                    isInvalidThemeElement = false;
                    continue;
                }

                themeElements.Add(themeElement);
            }

            return themeElements;
        }

        /// <summary>
        /// Sets view field.
        /// </summary>        
        private static void SetViewValue(View view, string attributeName, string attributeValue, List<ValueConverter> valueConverters, ViewElement viewElement, GameObject contentParent, string baseDirectory)
        {
            // check and parse if attribute is a binding
            if (ParseAttributeBinding(view, attributeName, attributeValue, contentParent))
                return; // yes. work done

            // check if attribute refers to a view action
            var actionField = viewElement.ViewActionFields.FirstOrDefault(x => String.Equals(attributeName, x.Name, StringComparison.OrdinalIgnoreCase));
            if (actionField != null)
            {
                // add view action entry to view action
                var viewAction = actionField.GetValue(view) as ViewAction;
                viewAction.Entries.Add(new ViewActionEntry(attributeValue, contentParent));
                return;
            }

            // try assign value to field
            FieldInfo fieldInfo = view.GetType().GetField(attributeName);
            if (fieldInfo == null)
            {
                Debug.LogError(String.Format("[MarkUX.340] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Field missing.", view.Name, attributeValue, view.GetType().Name, attributeName));
                return;
            }

            // check if the field is allowed to be be set from xml
            bool notAllowed = fieldInfo.GetCustomAttributes(typeof(NotSetFromXml), true).Any();
            if (notAllowed)
            {
                Debug.LogError(String.Format("[MarkUX.341] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Field not allowed to be set from xml.", view.Name, attributeValue, view.GetType().Name, attributeName));
                return;
            }

            // check if there are any value-converters for the field
            var valueConverter = fieldInfo.GetCustomAttributes(typeof(ValueConverter), true).FirstOrDefault() as ValueConverter;
            if (valueConverter == null)
            {
                // check if there is a default value converter for type
                valueConverter = valueConverters.FirstOrDefault(x => x.Type == fieldInfo.FieldType);
            }

            // convert and set value
            if (valueConverter != null)
            {
                object value = null;

                // handle special case when an asset is to be loaded
                if (valueConverter is AssetValueConverter)
                {
                    try
                    {
                        var trimmedValue = attributeValue.Trim();
                        string assetPath = null;
                        UnityEngine.Object asset = null;

                        assetPath = trimmedValue;

                        // does the path refer to a sprite in a sprite atlas?
                        if (assetPath.Contains(":"))
                        {
                            // yes. load sprite from atlas
                            string[] parts = assetPath.Split(':');
                            assetPath = parts[0];
                            if (!String.IsNullOrEmpty(baseDirectory))
                            {
                                assetPath = Path.Combine(baseDirectory, assetPath);
                            }

                            UnityEngine.Object[] subSprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                            foreach (var s in subSprites)
                            {
                                if (s.name == parts[1])
                                {
                                    asset = s;
                                    break;
                                }
                            }

                            if (asset == null)
                            {
                                Debug.LogError(String.Format("[MarkUX.342] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Value conversion failed. Asset not found at path.", view.Name, attributeValue, view.GetType().Name, attributeName));
                                return;
                            }

                            value = asset;
                        }
                        else
                        {
                            // load sprite
                            if (!String.IsNullOrEmpty(baseDirectory))
                            {
                                assetPath = Path.Combine(baseDirectory, assetPath);
                            }

                            asset = AssetDatabase.LoadAssetAtPath(assetPath, valueConverter.Type);
                            if (asset == null)
                            {
                                Debug.LogError(String.Format("[MarkUX.342] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Value conversion failed. Asset not found at path.", view.Name, attributeValue, view.GetType().Name, attributeName));
                                return;
                            }

                            value = asset;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(String.Format("[MarkUX.343] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Value conversion failed. Exception thrown: {4}", view.Name, attributeValue, view.GetType().Name, attributeName, e.Message));
                        return;
                    }
                }
                else
                {
                    ConversionResult result = valueConverter.Convert(attributeValue, new ValueConverterContext { BaseDirectory = baseDirectory });
                    if (!result.Success)
                    {
                        Debug.LogError(String.Format("[MarkUX.344] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Value conversion failed. {4}", view.Name, attributeValue, view.GetType().Name, attributeName, result.ErrorMessage));
                        return;
                    }
                    value = result.ConvertedObject;
                }

                // set the value!
                view.SetValue(fieldInfo, fieldInfo, view, value, null, false, true, false);
                return;
            }
            else
            {
                Debug.LogError(String.Format("[MarkUX.345] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Value converter missing.", view.Name, attributeValue, view.GetType().Name, attributeName));
            }
        }

        private static Regex _bindingRegex = new Regex(@"{(?<field>[A-Za-z0-9\.\[\]]+)(?<format>:[^}]+)?}");

        /// <summary>
        /// Parses attribute binding.
        /// </summary>
        private static bool ParseAttributeBinding(View view, string attributeName, string attributeValue, GameObject contentParent)
        {
            var value = attributeValue.Trim();

            // check if value contains bindings
            string formatString = String.Empty;
            List<Match> matches = new List<Match>();
            foreach (Match match in _bindingRegex.Matches(value))
            {
                matches.Add(match);
            }

            if (matches.Count <= 0)
                return false;

            if (matches.Count == 1 && (matches[0].Value.Length == value.Length) && String.IsNullOrEmpty(matches[0].Groups["format"].Value))
            {
                // no format string
            }
            else if (matches.Count > 1)
            {
                // multiple bindings found
                Debug.LogError(String.Format("[MarkUX.346] {0}: Unable to assign value \"{1}\" to view field \"{2}.{3}\". Multibindings are currently not supported.", view.Name, attributeValue, view.GetType().Name, attributeName));
                return false;
            }
            else
            {
                // single formatted binding found
                int matchCount = 0;
                formatString = _bindingRegex.Replace(value, x =>
                {
                    string matchCountString = matchCount.ToString();
                    ++matchCount;
                    return String.Format("{{{0}{1}}}", matchCountString, x.Groups["format"]);
                });
                //Debug.Log(String.Format("Found binding with format string: {0} | {1}", value, formatString));
            }

            var sourceFieldName = attributeName;
            var targetFieldName = matches[0].Groups["field"].Value.Trim();

            // if field refers to special reserved word "Item" don't create parent bindings
            bool isTemplateBinding = targetFieldName.StartsWith("Item.", StringComparison.OrdinalIgnoreCase);

            // create two bindings one for each binding direction unless we have a format string then it's a one-way binding
            if (String.IsNullOrEmpty(formatString) || isTemplateBinding)
            {
                view.FieldBindings.Add(new FieldBinding(sourceFieldName, targetFieldName, view.gameObject, contentParent, isTemplateBinding ? formatString : String.Empty, false));
            }

            if (!isTemplateBinding)
            {
                var parentBinding = new FieldBinding(targetFieldName, sourceFieldName, contentParent, view.gameObject, formatString, false);
                parentBinding.Update = true; // make sure parent value gets propagated to child on update
                contentParent.GetComponent<View>().FieldBindings.Add(parentBinding);
            }
            return true;
        }

        /// <summary>
        /// Sorts the view elements by their dependencies so they can be processed in the right order.
        /// </summary>
        private static List<ViewElement> SortByDependency(List<ViewElement> viewElements)
        {
            var sorted = new List<ViewElement>();
            while (viewElements.Any(x => !x.PermanentMark))
            {
                var viewElement = viewElements.First(x => !x.PermanentMark);
                Visit(viewElement, sorted, String.Empty);
            }

            return sorted;
        }

        /// <summary>
        /// Used by dependency sort algorithm.
        /// </summary>
        private static void Visit(ViewElement viewElement, List<ViewElement> sorted, string dependencyChain)
        {
            if (viewElement.TemporaryMark)
            {
                // cyclical dependency detected
                throw new Exception(String.Format("Cyclical dependency {0}{1} detected.", dependencyChain, viewElement.Name));
            }
            else if (!viewElement.PermanentMark)
            {
                viewElement.TemporaryMark = true;
                foreach (var dependency in viewElement.Dependencies)
                {
                    Visit(dependency, sorted, String.Format("{0}{1}->", dependencyChain, viewElement.Name));
                }
                viewElement.TemporaryMark = false;
                viewElement.PermanentMark = true;

                // add element to list
                sorted.Add(viewElement);
            }
        }

        /// <summary>
        /// Creates layout grid.
        /// </summary>
        private static void CreateLayoutGrid(ViewPresenter viewPresenter, GameObject layoutRoot)
        {
            int textureWidth = (int)viewPresenter.ElementSize;
            int textureHeight = (int)viewPresenter.ElementSize;

            // draw layout grid
            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);

            // set the pixel values
            for (int y = 0; y < textureHeight; ++y)
            {
                for (int x = 0; x < textureWidth; ++x)
                {
                    if (x == 0 || y == 0)
                    {
                        texture.SetPixel(x, y, new Color(0.73f, 0f, 0.67f));
                    }
                    else if (x == textureWidth / 2 || y == textureHeight / 2)
                    {
                        texture.SetPixel(x, y, new Color(0.43f, 0.43f, 0.43f));
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            // apply SetPixel calls
            texture.Apply();

            // create sprite
            Sprite sprite = new Sprite();
            sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.0f, 0.0f));

            // create grid object
            var go = new GameObject("LayoutGrid");
            go.transform.SetParent(layoutRoot.transform, false);
            go.layer = LayerMask.NameToLayer("UI");

            // create rect transform and set default values
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.Reset();
            go.transform.localScale = new Vector3(1, -1, 1); // align grid top-left

            // add image component
            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            image.type = UnityEngine.UI.Image.Type.Tiled;
            image.color = new Color(1f, 1f, 1f, 0.47f);

            // disable capturing of input
            var canvasGroup = go.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Gets all view assets in project.
        /// </summary>
        private static HashSet<TextAsset> GetAllViewAssets()
        {
            HashSet<TextAsset> viewAssets = new HashSet<TextAsset>();
            
            foreach (var path in Configuration.Instance.ViewPaths)
            {
                string localPath = path.StartsWith("Assets/") ? path.Substring(7) : path;
                foreach (var asset in GetAssetsAtPath<TextAsset>(localPath))
                {
                    viewAssets.Add(asset);
                }
            }

            return viewAssets;
        }

        /// <summary>
        /// Gets all assets of a certain type at a path.
        /// </summary>
        private static T[] GetAssetsAtPath<T>(string path)
        {            
            ArrayList al = new ArrayList();
            string searchPath = Application.dataPath + "/" + path;

            if (Directory.Exists(searchPath))
            {
                string[] fileEntries = Directory.GetFiles(searchPath, "*.xml", SearchOption.AllDirectories);
                foreach (string fileName in fileEntries)
                {
                    string localPathFix = "Assets/" + path + fileName.Substring(searchPath.Length);
                    UnityEngine.Object t = AssetDatabase.LoadAssetAtPath(localPathFix, typeof(T));

                    if (t != null)
                        al.Add(t);
                }
            }

            T[] result = new T[al.Count];
            for (int i = 0; i < al.Count; i++)
                result[i] = (T)al[i];

            return result;
        }

        #endregion
    }
}

