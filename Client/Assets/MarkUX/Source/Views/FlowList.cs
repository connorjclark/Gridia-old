#region Using Statements
using MarkUX.Animation;
using MarkUX.ValueConverters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Shows a list of views flowing horizontally or vertically based on a collection of data.
    /// </summary>
    [InternalView]
    public class FlowList : ContentView
    {
        #region Fields

        public ViewAction SelectionChanged;
        public ViewAction ItemsChanged;

        [ChangeHandler("UpdateLayout")]
        public string SelectedIndex;
        public bool CanDeselect;
        public bool CanMultiSelect;
        public bool CanSelect;

        [NotSetFromXml]
        private bool _firstItems;

        [ChangeHandler("UpdateLayouts")]
        [GenericListValueConverter]
        public List<object> Items;
        public bool ItemsSet;        

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

        private List<object> _generatedItems;
        private List<object> _selectedItems;
        private View _parent;
        private View _template;

        [ChangeHandler("UpdateLayout")]
        public Margin ContentMargin;

        [ChangeHandler("UpdateLayouts")]
        public Orientation Orientation;

        [ChangeHandler("UpdateLayouts")]
        public ElementSize Spacing;
        public bool SpacingSet;

        [ChangeHandler("UpdateLayouts")]
        public ElementSize HorizontalSpacing;

        [ChangeHandler("UpdateLayouts")]
        public ElementSize VerticalSpacing;

        // flow list item header
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

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FlowList()
        {
            ItemImage = null;
            SelectedItemImage = null;
            HighlightedItemImage = null;
            Spacing = new ElementSize();
            HorizontalSpacing = new ElementSize();
            VerticalSpacing = new ElementSize();
            ContentMargin = new Margin();
            CanDeselect = false;
            CanMultiSelect = false;
            CanSelect = false;
            ItemColor = Color.clear;
            ItemImageType = UnityEngine.UI.Image.Type.Simple;
            SelectedItemColor = Color.clear;
            HighlightedItemColor = Color.clear;
            SelectedIndex = String.Empty;
            _firstItems = true;
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
                        GenerateFlowListItems();
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
                    GenerateFlowListItems(); // clear items
                }           
            }
            
            // update offsets of children
            var horizontalSpacing = SpacingSet ? Spacing : HorizontalSpacing;
            var verticalSpacing = SpacingSet ? Spacing : VerticalSpacing;
            bool isHorizontal = Orientation == Orientation.Horizontal;
            float xOffset = 0f;
            float yOffset = 0f;
            float maxColumnWidth = 0f;
            float maxRowHeight = 0f;
            float maxWidth = 0f;
            float maxHeight = 0f;
            bool firstItem = true;

            var rectTransform = GetComponent<RectTransform>();
            float flowListWidth = rectTransform.rect.width;
            float flowListHeight = rectTransform.rect.height;

            int childCount = transform.childCount;
            int index = 0;
            for (int i = 0; i < childCount; ++i)
            {
                var go = transform.GetChild(i);
                var view = go.GetComponent<View>() as FlowListItem;
                if (view == null || view.IsDestroyed)
                    continue;

                view.SetValue(() => view.Index, index + 1);
                view.SetValue(() => view.ZeroBasedIndex, index);
                view.SetValue(() => view.CanToggleOff, false); // let list handle selection logic
                view.SetValue(() => view.CanToggleOn, false);

                // set background images
                if (ItemImageSet)
                {
                    view.SetValue(() => view.BackgroundImage, ItemImage);
                    view.SetValue(() => view.BackgroundImageType, ItemImageType);
                }

                if (ItemColorSet)
                {
                    view.SetValue(() => view.BackgroundColor, ItemColor);
                }

                if (SelectedItemImageSet)
                {
                    view.SetValue(() => view.PressedImage, SelectedItemImage);
                }

                if (SelectedItemColorSet)
                {
                    view.SetValue(() => view.PressedColor, SelectedItemColor);
                }

                if (HighlightedItemImageSet)
                {
                    view.SetValue(() => view.HighlightedImage, HighlightedItemImage);
                }

                if (HighlightedItemColorSet)
                {
                    view.SetValue(() => view.HighlightedColor, HighlightedItemColor);
                }       

                // set header font style
                if (ItemFontSet)
                {
                    view.SetValue(() => view.Font, ItemFont);
                }

                if (ItemTextMarginSet)
                {
                    view.SetValue(() => view.TextMargin, ItemTextMargin);
                }

                if (ItemFontStyleSet)
                {
                    view.SetValue(() => view.FontStyle, ItemFontStyle);
                }

                if (ItemFontSizeSet)
                {
                    view.SetValue(() => view.FontSize, ItemFontSize);
                }

                if (ItemFontColorSet)
                {
                    view.SetValue(() => view.FontColor, ItemFontColor);
                }

                if (ItemTextAlignmentSet)
                {
                    view.SetValue(() => view.TextAlignment, ItemTextAlignment);
                }

                if (ItemShadowColorSet)
                {
                    view.SetValue(() => view.ShadowColor, ItemShadowColor);
                }

                if (ItemShadowDistanceSet)
                {
                    view.SetValue(() => view.ShadowDistance, ItemShadowDistance);
                }

                if (ItemOutlineColorSet)
                {
                    view.SetValue(() => view.OutlineColor, ItemOutlineColor);
                }

                if (ItemOutlineDistanceSet)
                {
                    view.SetValue(() => view.OutlineDistance, ItemOutlineDistance);
                }                

                // skip the rest of initialization for templates
                if (view == _template)
                    continue;

                if (!view.IsInitialized)
                {
                    // add view action entry for click events
                    view.ViewActions.First(y => y.Name == "Click").Entries.Add(
                            new ViewActionEntry("FlowListItemClick", gameObject)
                        );

                    view.IsInitialized = true;
                }

                ++index;

                if (view.Width.Unit == ElementSizeUnit.Percents || view.Height.Unit == ElementSizeUnit.Percents)
                {
                    Debug.LogError(String.Format("[MarkUX.323] Unable to list view \"{0}\" as it doesn't specify its width and height in pixels or elements.", view.Name));
                    return;
                }

                // set alignment
                view.Alignment = Alignment.TopLeft;

                // set vertical and horizontal offset of item
                if (isHorizontal)
                {
                    if (firstItem)
                    {
                        xOffset = 0;
                        firstItem = false;
                    }                    
                    else if ((xOffset + view.Width.Pixels + horizontalSpacing.Pixels) > flowListWidth)
                    {
                        // overflow to next row                        
                        xOffset = 0;
                        yOffset += maxRowHeight + verticalSpacing.Pixels;
                        maxRowHeight = 0;                        
                    }
                    else
                    {
                        // add spacing
                        xOffset += horizontalSpacing.Pixels;
                    }

                    // set offset
                    view.OffsetFromParent = new Margin(
                        new ElementSize(xOffset, ElementSizeUnit.Pixels),
                        new ElementSize(yOffset, ElementSizeUnit.Pixels)
                        );

                    xOffset += view.Width.Pixels;
                    maxRowHeight = Mathf.Max(maxRowHeight, view.Height.Pixels);
                    maxWidth = Mathf.Max(maxWidth, xOffset);
                    maxHeight = Mathf.Max(maxHeight, yOffset + view.Height.Pixels);
                }
                else
                {
                    if (firstItem)
                    {
                        yOffset = 0;
                        firstItem = false;
                    }
                    else if ((yOffset + view.Height.Pixels + verticalSpacing.Pixels) > flowListHeight)
                    {
                        // overflow to next column                        
                        yOffset = 0;
                        xOffset += maxColumnWidth + horizontalSpacing.Pixels;
                        maxColumnWidth = 0;
                    }
                    else
                    {
                        // add spacing
                        yOffset += verticalSpacing.Pixels;
                    }

                    // set offset
                    view.OffsetFromParent = new Margin(
                        new ElementSize(xOffset, ElementSizeUnit.Pixels),
                        new ElementSize(yOffset, ElementSizeUnit.Pixels)
                        );

                    yOffset += view.Height.Pixels;
                    maxColumnWidth = Mathf.Max(maxColumnWidth, view.Width.Pixels);
                    maxWidth = Mathf.Max(maxWidth, xOffset + view.Width.Pixels);
                    maxHeight = Mathf.Max(maxHeight, yOffset);
                }

                // update layout of child
                view.UpdateLayout();
            }

            // set total width/height based on flow direction
            if (isHorizontal)
            {
                maxHeight += Margin.Top.Pixels + Margin.Bottom.Pixels;
                Height = new ElementSize(maxHeight, ElementSizeUnit.Pixels);
            }
            else
            {
                maxWidth += Margin.Left.Pixels + Margin.Right.Pixels;
                Width = new ElementSize(maxWidth, ElementSizeUnit.Pixels);
            }

            // set selected items
            var previouslySelectedItems = _selectedItems != null ? new List<object>(_selectedItems) : new List<object>();
            _selectedItems.Clear();
            if (index > 0)
            {
                // first time populating list?
                if (_firstItems)
                {
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
                            Debug.LogError(String.Format("[MarkUX.324] {0}: Unable to parse SelectedIndex=\"{1}\". Improperly formatted string. Expected a comma-separated list of integers.", Name, SelectedIndex));
                            selectedIndexArray = new int[0];
                        }

                        int itemIndex = 0;
                        this.ForEachChild<FlowListItem>(x =>
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

                    _firstItems = false;
                }
                else
                {
                    // select previously selected items
                    this.ForEachChild<FlowListItem>(x =>
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
        public void FlowListItemClick(FlowListItem source)
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
            FlowListItem source = null;
            if (objSource is FlowListItem)
            {
                source = objSource as FlowListItem;
            }
            else
            {
                this.ForEachChild<FlowListItem>(x =>
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
                    SelectionChanged.Trigger(new FlowListSelectionActionData());
                }
            }
            else
            {
                // check if any other item is selected and unselect it
                if (!CanMultiSelect)
                {
                    this.ForEachChild<FlowListItem>(x =>
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
                SelectionChanged.Trigger(new FlowListSelectionActionData { FlowListItem = source });
                _selectedItems.Add(source.IsDynamic ? source.Item : source);
            }
        }

        /// <summary>
        /// Deselects all items in the flow-list.
        /// </summary>
        public void DeselectAll()
        {
            this.ForEachChild<FlowListItem>(x =>
            {
                if (x == _template || x.IsDestroyed)
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

        /// <summary>
        /// Generates flow list items.
        /// </summary>
        private void GenerateFlowListItems()
        {
            // clear all items except template and static items
            this.ForEachChild<FlowListItem>(x =>
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
                Debug.LogError(String.Format("[MarkUX.325] {0}: Unable to generate flow list from items. Template missing. Add a template by adding a <FlowListItem Id=\"Template\" /> to the flow list.", Name));
                return;
            }

            // create new items
            for (int i = 0; i < _generatedItems.Count; ++i)
            {
                // wrap template in list item if necessary
                FlowListItem flowListItem = null;
                if (_template is FlowListItem)
                {
                    flowListItem = CreateViewFromTemplate(_template.gameObject, gameObject, _parent) as FlowListItem;
                    flowListItem.Activate();
                }
                else
                {
                    flowListItem = CreateView<FlowListItem>(gameObject, this);
                    var listItemContent = CreateViewFromTemplate(_template.gameObject, flowListItem.gameObject, _parent);
                    listItemContent.Activate();
                }

                flowListItem.Id = String.Empty;
                flowListItem.Name = String.Format("FlowListItem ({0})", _generatedItems[i] != null ? _generatedItems[i].GetType().Name : "null");
                flowListItem.gameObject.name = flowListItem.Name;

                // handle special case when item text is bound to item data
                var itemDataTextFieldBinding = flowListItem.FieldBindings.FirstOrDefault(x => x.SourceFieldPathString == "Text" &&
                    x.TargetFieldPathString.StartsWith("Item."));
                if (itemDataTextFieldBinding != null)
                {
                    var textFieldBinding = flowListItem.FieldBindings.FirstOrDefault(x => x.SourceFieldPathString == "Text" && x.TargetFieldPathString == "Text");

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
                    flowListItem.FieldBindings.Remove(itemDataTextFieldBinding);
                }

                // update field bindings
                flowListItem.ForEachChild<View>(x =>
                {
                    foreach (var fieldBinding in x.FieldBindings)
                    {
                        // source need to be this parent
                        if (fieldBinding.Target != Parent)
                            continue;

                        // check if any bindings references "Item"
                        if (fieldBinding.TargetFieldPathString.StartsWith("Item.", StringComparison.OrdinalIgnoreCase))
                        {
                            // binding references item
                            fieldBinding.Target = flowListItem.gameObject;

                            // handle special case when binding refers to item index
                            if (fieldBinding.TargetFieldPathString.Equals("Item.Index", StringComparison.OrdinalIgnoreCase) ||
                                fieldBinding.TargetFieldPathString.Equals("Item.ZeroBasedIndex", StringComparison.OrdinalIgnoreCase))
                            {
                                fieldBinding.TargetFieldPathString = fieldBinding.TargetFieldPathString.Substring(5);
                            }

                            // add parent binding to list item
                            var newBinding = new FieldBinding(fieldBinding.TargetFieldPathString, fieldBinding.SourceFieldPathString, flowListItem.gameObject, x.gameObject, fieldBinding.FormatString, true);
                            newBinding.Update = true;

                            // remove format string from source
                            fieldBinding.FormatString = String.Empty;
                            flowListItem.FieldBindings.Add(newBinding);
                        }
                    }
                }, true, _parent);

                flowListItem.Item = _generatedItems[i];
                flowListItem.InitializeViews();
                flowListItem.UpdateViews();
            }
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Items = new List<object>();
            _generatedItems = new List<object>();
            _selectedItems = new List<object>();
            _firstItems = true;

            // set template and parent view
            _template = this.FindView<View>("Template", false);
            _parent = Parent != null ? Parent.GetComponent<View>() : null;
            if (_template != null)
            {
                _template.Deactivate();
            }
        }

        /// <summary>
        /// Called when a child layout has been updated.
        /// </summary>
        public void ChildLayoutUpdatedHasChanged()
        {
            UpdateLayout();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<FlowList Orientation=""Horizontal"">
                </FlowList>";
        }

        #endregion
    }
}
