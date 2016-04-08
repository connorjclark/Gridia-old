#region Using Statements
using MarkUX.Animation;
using MarkUX.ValueConverters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Shows a list of views based on a collection of data.
    /// </summary>
    [InternalView]
    public class List : Group
    {
        #region Fields

        public ViewAction SelectionChanged;
        public ViewAction ItemsChanged;

        public string SelectedIndex;
        public bool CanDeselect;
        public bool CanMultiSelect;
        public bool CanSelect;

        private bool _firstItems;

        [ChangeHandler("UpdateLayouts")]
        [GenericListValueConverter]
        public List<object> Items;
        public bool ItemsSet;        

        // list mask
        public Mask ListMask;
        public Sprite ListMaskImage;
        public UnityEngine.UI.Image.Type ListMaskImageType;
        public Color ListMaskColor;

        // default item width/height
        public ElementSize ItemWidth;
        public bool ItemWidthSet;
        public ElementSize ItemHeight;
        public bool ItemHeightSet;

        // item background
        [ChangeHandler("UpdateLayout")]
        public Color ItemColor;
        public bool ItemColorSet;

        [ChangeHandler("UpdateLayout")]
        public Sprite ItemImage;
        public bool ItemImageSet;

        [ChangeHandler("UpdateLayout")]
        public UnityEngine.UI.Image.Type ItemImageType;

        // selected background
        [ChangeHandler("UpdateLayout")]
        public Color SelectedItemColor;
        public bool SelectedItemColorSet;

        [ChangeHandler("UpdateLayout")]
        public Sprite SelectedItemImage;
        public bool SelectedItemImageSet;

        // highlighted background
        [ChangeHandler("UpdateLayout")]
        public Color HighlightedItemColor;
        public bool HighlightedItemColorSet;

        [ChangeHandler("UpdateLayout")]
        public Sprite HighlightedItemImage;
        public bool HighlightedItemImageSet;

        // list item header
        public Font ItemFont;
        public bool ItemFontSet;
        public Margin ItemTextMargin;
        public bool ItemTextMarginSet;
        public FontStyle ItemFontStyle;
        public bool ItemFontStyleSet;
        public int ItemFontSize;
        public bool ItemFontSizeSet;
        public Color ItemFontColor;
        public bool ItemFontColorSet;
        public Alignment ItemTextAlignment;
        public bool ItemTextAlignmentSet;
        public Color ItemShadowColor;
        public bool ItemShadowColorSet;
        public Vector2 ItemShadowDistance;
        public bool ItemShadowDistanceSet;
        public Color ItemOutlineColor;
        public bool ItemOutlineColorSet;
        public Vector2 ItemOutlineDistance;
        public bool ItemOutlineDistanceSet;

        private List<object> _generatedItems;
        private List<object> _selectedItems;
        private View _parent;
        private View _template;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public List()
        {
            ItemImage = null;
            SelectedItemImage = null;
            HighlightedItemImage = null;
            Spacing = new ElementSize();
            CanDeselect = false;
            CanMultiSelect = false;
            CanSelect = true;
            CanSelect = true;
            ItemTextAlignment = Alignment.Center;
            ItemTextMargin = new Margin();
            ItemColor = Color.clear;
            ItemImageType = UnityEngine.UI.Image.Type.Simple;
            SelectedItemColor = Color.clear;
            HighlightedItemColor = Color.clear;
            SelectedIndex = String.Empty;
            _firstItems = true;

            ListMaskColor = new Color(1,1,1,0);
            ListMaskImageType = UnityEngine.UI.Image.Type.Simple;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets list of currently selected items.
        /// </summary>
        public List<object> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            if (CanMultiSelect)
            {
                CanSelect = true;
            }

            bool hasItemsChanged = false;
            if (Application.isPlaying)
            {
                // do we have any new items to present?
                if (Items != null && Items.Count > 0)
                {
                    bool isEqual = _generatedItems != null ? _generatedItems.SequenceEqual<object>(Items) : false;
                    if (!isEqual)
                    {
                        // yes. generate new list items
                        _generatedItems = new List<object>(Items);
                        GenerateListItems();
                        hasItemsChanged = true;
                    }
                }
                else
                {
                    if (_generatedItems != null && _generatedItems.Count > 0)
                    {
                        hasItemsChanged = true;
                    }

                    // no. clear list of items
                    _generatedItems = new List<object>();
                    GenerateListItems(); // clear items
                }
            }

            // set list item values
            int index = 0;
            ListMask.ForEachChild<ListItem>(x =>
            {
                if (x.IsDestroyed)
                    return;

                x.SetValue(() => x.Index, index + 1);
                x.SetValue(() => x.ZeroBasedIndex, index);
                x.SetValue(() => x.CanToggleOff, false); // let list handle selection logic
                x.SetValue(() => x.CanToggleOn, false);

                // set item width/height
                if (ItemWidthSet && ItemWidth != null)
                {
                    x.SetValue(() => x.Width, ItemWidth);
                }

                if (ItemHeightSet && ItemHeight != null)
                {
                    x.SetValue(() => x.Height, ItemHeight);
                }

                // set item images
                if (ItemImageSet)
                {
                    x.SetValue(() => x.BackgroundImage, ItemImage);
                    x.SetValue(() => x.BackgroundImageType, ItemImageType);
                }

                if (ItemColorSet)
                {
                    x.SetValue(() => x.BackgroundColor, ItemColor);
                }

                if (SelectedItemImageSet)
                {
                    x.SetValue(() => x.PressedImage, SelectedItemImage);
                }

                if (SelectedItemColorSet)
                {
                    x.SetValue(() => x.PressedColor, SelectedItemColor);
                }

                if (HighlightedItemImageSet)
                {
                    x.SetValue(() => x.HighlightedImage, HighlightedItemImage);
                }

                if (HighlightedItemColorSet)
                {
                    x.SetValue(() => x.HighlightedColor, HighlightedItemColor);
                }

                // set header font style
                if (ItemFontSet)
                {
                    x.SetValue(() => x.Font, ItemFont);
                }

                if (ItemTextMarginSet)
                {
                    x.SetValue(() => x.TextMargin, ItemTextMargin);
                }

                if (ItemFontStyleSet)
                {
                    x.SetValue(() => x.FontStyle, ItemFontStyle);
                }

                if (ItemFontSizeSet)
                {
                    x.SetValue(() => x.FontSize, ItemFontSize);
                }

                if (ItemFontColorSet)
                {
                    x.SetValue(() => x.FontColor, ItemFontColor);
                }

                if (ItemTextAlignmentSet)
                {
                    x.SetValue(() => x.TextAlignment, ItemTextAlignment);
                }

                if (ItemShadowColorSet)
                {
                    x.SetValue(() => x.ShadowColor, ItemShadowColor);
                }

                if (ItemShadowDistanceSet)
                {
                    x.SetValue(() => x.ShadowDistance, ItemShadowDistance);
                }

                if (ItemOutlineColorSet)
                {
                    x.SetValue(() => x.OutlineColor, ItemOutlineColor);
                }

                if (ItemOutlineDistanceSet)
                {
                    x.SetValue(() => x.OutlineDistance, ItemOutlineDistance);
                }

                // skip the rest of initialization for templates
                if (x == _template)
                    return;

                if (!x.IsInitialized)
                {
                    // add view action entry for click events
                    x.ViewActions.First(y => y.Name == "Click").Entries.Add(
                            new ViewActionEntry("ListItemClick", gameObject)
                        );

                    x.IsInitialized = true;
                }

                ++index;
            }, false);

            // set selected items
            var previouslySelectedItems = new List<object>(_selectedItems);
            _selectedItems.Clear();
            if (index > 0)
            {
                // first time populating list?
                if (_firstItems)
                {
                    _firstItems = false;

                    // yes. get default selected index
                    if (!String.IsNullOrEmpty(SelectedIndex))
                    {
                        // get selected indexes
                        int[] selectedIndexArray;
                        try
                        {
                            selectedIndexArray = SelectedIndex.Split(',').Select(x => System.Convert.ToInt32(x, CultureInfo.InvariantCulture)).ToArray();
                        }
                        catch
                        {
                            Debug.LogError(String.Format("[MarkUX.326] {0}: Unable to parse SelectedIndex=\"{1}\". Improperly formatted string. Expected a comma-separated list of integers.", Name, SelectedIndex));
                            selectedIndexArray = new int[0];
                        }

                        int itemIndex = 0;
                        ListMask.ForEachChild<ListItem>(x =>
                        {
                            if (x == _template || x.IsDestroyed)
                                return;

                            // select item
                            if (selectedIndexArray.Contains(x.ZeroBasedIndex))
                            {
                                x.ToggleValue = false; // make sure selection gets re-triggered when list is initialized
                                SelectItem(x);
                            }

                            ++itemIndex;
                        }, false);
                    }
                }
                else
                {
                    // select previously selected items
                    ListMask.ForEachChild<ListItem>(x =>
                    {
                        if (x == _template || x.IsDestroyed)
                            return;

                        if (x.ToggleValue == false && previouslySelectedItems.Contains(x.IsDynamic ? x.Item : x))
                        {
                            SelectItem(x);
                        }
                    }, false);
                }
            }

            // if items have changed trigger change action
            if (hasItemsChanged)
            {
                ItemsChanged.Trigger();
            }

            base.UpdateLayout();
        }

        /// <summary>
        /// Called when a list item is clicked.
        /// </summary>
        public void ListItemClick(ListItem source)
        {
            SelectItem(source, true);
        }

        /// <summary>
        /// Selects item in list.
        /// </summary>
        public void SelectItem(object objSource, bool userTriggered = false)
        {
            if (userTriggered && !CanSelect)
                return;

            // find item to select
            ListItem source = null;
            if (objSource is ListItem)
            {
                source = objSource as ListItem;
            }
            else
            {
                ListMask.ForEachChild<ListItem>(x =>
                {
                    if (x.IsDynamic && x.Item == objSource)
                    {
                        source = x;
                    }
                }, false);
            }
            
            // is the item already selected?
            if (source.ToggleValue == true)
            {
                // yes. can it be deselected?
                if (userTriggered && !CanDeselect)
                {
                    return; // no. do nothing
                }
                else
                {
                    // deselect and trigger event
                    source.SetValue(() => source.ToggleValue, false);
                    _selectedItems.Remove(source.IsDynamic ? source.Item : source);
                    SelectionChanged.Trigger(new ListSelectionActionData()); 
                }
            }
            else
            {
                // check if any other item is selected and unselect it
                if (!CanMultiSelect)
                {
                    ListMask.ForEachChild<ListItem>(x =>
                    {
                        if (x == _template || x == source || x.IsDestroyed)
                        {
                            return;
                        }

                        if (x.ToggleValue)
                        {
                            x.SetValue(() => x.ToggleValue, false);
                            _selectedItems.Remove(x.IsDynamic ? x.Item : x);
                        }
                    }, false);
                }

                // select and trigger event
                source.SetValue(() => source.ToggleValue, true);
                SelectionChanged.Trigger(new ListSelectionActionData { ListItem = source });
                _selectedItems.Add(source.IsDynamic ? source.Item : source); 
            }
        }

        /// <summary>
        /// Generate list items.
        /// </summary>
        private void GenerateListItems()
        {
            // clear all items except template and static items
            ListMask.ForEachChild<ListItem>(x =>
            {
                if (x != _template && x.IsDynamic && !x.IsDestroyed)
                {
                    x.Deactivate();
                    x.IsDestroyed = true;
                    GameObject.Destroy(x.gameObject);
                }
            }, false);

            // do we have a template?
            if (_generatedItems.Count > 0 && _template == null)
            {
                Debug.LogError(String.Format("[MarkUX.327] {0}: Unable to generate list from items. Template missing. Add a template by adding a <ListItem Id=\"Template\" /> to the list.", Name));
                return;
            }

            // create new items
            for (int i = 0; i < _generatedItems.Count; ++i)
            {
                ListItem listItem = null;
                if (_template is ListItem)
                {
                    listItem = CreateViewFromTemplate(_template.gameObject, ListMask.gameObject, _parent) as ListItem;
                    listItem.Activate();
                }
                else
                {
                    Debug.LogError(String.Format("[MarkUX.374] {0}: Unable to generate list items. Template must be of type ListItem.", Name));
                    return;
                }

                listItem.Id = String.Empty;
                listItem.Name = String.Format("ListItem ({0})", _generatedItems[i] != null ? _generatedItems[i].GetType().Name : "null");
                listItem.gameObject.name = listItem.Name;

                // handle special case when item text is bound to item data
                var itemDataTextFieldBinding = listItem.FieldBindings.FirstOrDefault(x => x.SourceFieldPathString == "Text" && 
                    x.TargetFieldPathString.StartsWith("Item."));                
                if (itemDataTextFieldBinding != null)
                {
                    var textFieldBinding = listItem.FieldBindings.FirstOrDefault(x => x.SourceFieldPathString == "Text" && x.TargetFieldPathString == "Text");

                    // handle special case when binding refers to item index
                    if (itemDataTextFieldBinding.TargetFieldPathString.Equals("Item.Index", StringComparison.OrdinalIgnoreCase) ||
                        itemDataTextFieldBinding.TargetFieldPathString.Equals("Item.ZeroBasedIndex", StringComparison.OrdinalIgnoreCase))
                    {
                        itemDataTextFieldBinding.TargetFieldPathString = itemDataTextFieldBinding.TargetFieldPathString.Substring(5);
                    }

                    // update text field binding to reference item
                    textFieldBinding.SourceFieldPathString = itemDataTextFieldBinding.TargetFieldPathString;
                    textFieldBinding.FormatString = itemDataTextFieldBinding.FormatString;
                    textFieldBinding.Update = true;

                    // remove this field binding
                    listItem.FieldBindings.Remove(itemDataTextFieldBinding);
                }

                // update field binding of children
                listItem.ForEachChild<View>(x =>
                {
                    UpdateChildFieldBindings(x, listItem);
                }, true, _parent);

                listItem.Item = _generatedItems[i];
                listItem.InitializeViews();
                listItem.UpdateViews();
            }
        }

        /// <summary>
        /// Updates item field bindings.
        /// </summary>
        private void UpdateChildFieldBindings(View x, View listItem)
        {
            foreach (var fieldBinding in x.FieldBindings)
            {
                // source need to be this parent
                if (fieldBinding.Target != _parent.gameObject)
                    continue;

                // check if any bindings references "Item"
                if (fieldBinding.TargetFieldPathString.StartsWith("Item.", StringComparison.OrdinalIgnoreCase))
                {
                    // binding references item
                    fieldBinding.Target = listItem.gameObject;

                    // handle special case when binding refers to item index
                    if (fieldBinding.TargetFieldPathString.Equals("Item.Index", StringComparison.OrdinalIgnoreCase) ||
                        fieldBinding.TargetFieldPathString.Equals("Item.ZeroBasedIndex", StringComparison.OrdinalIgnoreCase))
                    {
                        fieldBinding.TargetFieldPathString = fieldBinding.TargetFieldPathString.Substring(5);
                    }

                    // add parent binding to list item
                    var newBinding = new FieldBinding(fieldBinding.TargetFieldPathString, fieldBinding.SourceFieldPathString, listItem.gameObject, x.gameObject, fieldBinding.FormatString, true);
                    newBinding.Update = true;

                    // remove format string from source
                    fieldBinding.FormatString = String.Empty;
                    listItem.FieldBindings.Add(newBinding);
                }
            }
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            GroupContentContainer = ListMask;
            Items = new List<object>();
            _generatedItems = new List<object>();
            _selectedItems = new List<object>();
            _firstItems = true;

            // set template and parent view
            if (_template == null)
            {
                _template = ListMask.FindView<View>("Template", false);
            }
           
            _parent = _template != null && _template.Parent != null ? _template.Parent.GetComponent<View>() : 
                (Parent != null ? Parent.GetComponent<View>() : null);
            if (_template != null)
            {
                _template.Deactivate();
            }
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<List Spacing=""0"" Orientation=""Vertical"" CanDeselect=""False"">
                        <Mask Id=""ListMask"" BackgroundImage=""{ListMaskImage}"" 
                              BackgroundImageType=""{ListMaskImageType}"" BackgroundColor=""{ListMaskColor}"">
                            <ContentContainer IncludeContainerInContent=""False"" />
                        </Mask>
                    </List>";
        }

        #endregion
    }
}
