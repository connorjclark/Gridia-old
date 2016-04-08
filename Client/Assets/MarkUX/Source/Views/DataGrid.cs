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
    /// Shows a grid of views based on a collection of data.
    /// </summary>
    [InternalView]
    [CreatesView(typeof(Row))]
    [CreatesView(typeof(Column))]
    public class DataGrid : Group
    {
        #region Fields

        public ViewAction SelectionChanged;
        public string SelectedIndex;
        public bool CanDeselect;
        public bool CanMultiSelect;
        public bool CanSelect;

        [ChangeHandler("UpdateLayout")]
        public int SortColumnIndex;
        public bool SortColumnIndexSet;

        // row height
        public ElementSize RowHeight;

        // column text
        [ChangeHandler("UpdateLayout")]
        public Margin TextOffset;

        [ChangeHandler("UpdateLayout")]
        public Margin HeaderTextOffset;
        public bool HeaderTextOffsetSet;

        [ChangeHandler("UpdateLayout")]
        public Alignment TextAlignment;

        [ChangeHandler("UpdateLayout")]
        public Alignment HeaderTextAlignment;
        public bool HeaderTextAlignmentSet;
                
        public Font Font;
        public FontStyle FontStyle;
        public int FontSize;
        public Color FontColor;        
        public Color ShadowColor;
        public Vector2 ShadowDistance;
        public Color OutlineColor;
        public Vector2 OutlineDistance;

        public Font HeaderFont;
        public FontStyle HeaderFontStyle;
        public int HeaderFontSize;
        public Color HeaderFontColor;
        public Color HeaderShadowColor;
        public Vector2 HeaderShadowDistance;
        public Color HeaderOutlineColor;
        public Vector2 HeaderOutlineDistance;

        [ChangeHandler("UpdateLayout")]
        public bool ShowHeaders;

        [ChangeHandler("UpdateLayout")]
        public ElementSize RowSpacing;
        public bool RowSpacingSet;

        [ChangeHandler("UpdateLayout")]
        public ElementSize ColumnSpacing;
        public bool ColumnSpacingSet;

        [ChangeHandler("UpdateLayouts")]
        [GenericListValueConverter]
        public List<object> Items;
        public bool ItemsSet;

        // header row
        [ChangeHandler("UpdateLayout")]
        public Sprite HeaderRowImage;
        public bool HeaderRowImageSet;

        [ChangeHandler("UpdateLayout")]
        public Color HeaderRowColor;
        public bool HeaderRowColorSet;

        // row
        [ChangeHandler("UpdateLayout")]
        public Color RowColor;
        public bool RowColorSet;

        [ChangeHandler("UpdateLayout")]
        public Sprite RowImage;
        public bool RowImageSet;

        [ChangeHandler("UpdateLayout")]
        public UnityEngine.UI.Image.Type RowImageType;
        public bool RowImageTypeSet;

        // selected background
        [ChangeHandler("UpdateLayout")]
        public Color SelectedRowColor;
        public bool SelectedRowColorSet;

        [ChangeHandler("UpdateLayout")]
        public Sprite SelectedRowImage;
        public bool SelectedRowImageSet;

        // highlighted background
        [ChangeHandler("UpdateLayout")]
        public Color HighlightedRowColor;
        public bool HighlightedRowColorSet;

        [ChangeHandler("UpdateLayout")]
        public Sprite HighlightedRowImage;
        public bool HighlightedRowImageSet;

        public Row HeaderRow;

        private List<object> _generatedItems;
        private List<object> _selectedItems;
        private View _parent;
        private bool _firstItems;
        private RowDefinition _rowDefinition;
        private bool _actualWidthWasZero;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DataGrid()
        {
            RowHeight = new ElementSize(1, ElementSizeUnit.Elements);
            RowSpacing = new ElementSize();
            ColumnSpacing = new ElementSize();
            RowImage = null;
            SelectedRowImage = null;
            HighlightedRowImage = null;
            Spacing = new ElementSize();
            CanDeselect = false;
            CanMultiSelect = false;
            CanSelect = true;
            CanSelect = true;
            RowColor = Color.clear;
            RowImageType = UnityEngine.UI.Image.Type.Simple;
            SelectedRowColor = Color.clear;
            HighlightedRowColor = Color.clear;
            SelectedIndex = String.Empty;
            _firstItems = true;
            ShowHeaders = true;
            TextAlignment = Alignment.Center;
            HeaderTextAlignment = Alignment.Center;
            TextOffset = new Margin();
            HeaderTextOffset = new Margin();

            HeaderRowImage = null;
            HeaderRowColor = Color.clear;
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
        /// Called every frame to update the view.
        /// </summary>
        public void Update()
        {
            // workaround for issue were ActualWidth is 0 while view is about to be presented
            if (ActualWidth <= 0)
            {
                _actualWidthWasZero = true;
                return;
            }
            else if (_actualWidthWasZero)
            {
                _actualWidthWasZero = false;
                UpdateLayout();
            }
        }

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            if (!HeaderTextAlignmentSet)
            {
                HeaderTextAlignment = TextAlignment;
            }

            if (!HeaderTextOffsetSet)
            {
                HeaderTextOffset = TextOffset;
            }
                        
            if (CanMultiSelect)
            {
                CanSelect = true;
            }

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
                        GenerateRows();
                    }
                }
                else
                {
                    // no. clear list of items
                    _generatedItems = new List<object>();
                    GenerateRows(); // clear items
                }
            }

            // set grid row values
            int index = 0;
            this.ForEachChild<Row>(x =>
            {
                if (x.IsDestroyed)
                    return;
                             
                x.TextAlignment = TextAlignment;
                x.HeaderTextAlignment = HeaderTextAlignment;
                x.TextOffset = TextOffset;
                x.HeaderTextOffset = HeaderTextOffset;
                x.ColumnSpacing = ColumnSpacingSet ? ColumnSpacing : Spacing;
                x.RowDefinition = _rowDefinition;

                x.Font = Font;
                x.FontStyle = FontStyle;
                x.FontSize = FontSize;
                x.FontColor = FontColor;
                x.ShadowColor = ShadowColor;
                x.ShadowDistance = ShadowDistance;
                x.OutlineColor = OutlineColor;
                x.OutlineDistance = OutlineDistance;

                x.HeaderFont = HeaderFont;
                x.HeaderFontStyle = HeaderFontStyle;
                x.HeaderFontSize = HeaderFontSize;
                x.HeaderFontColor = HeaderFontColor;
                x.HeaderShadowColor = HeaderShadowColor;
                x.HeaderShadowDistance = HeaderShadowDistance;
                x.HeaderOutlineColor = HeaderOutlineColor;
                x.HeaderOutlineDistance = HeaderOutlineDistance;

                x.SetValue(() => x.Index, index + 1);
                x.SetValue(() => x.ZeroBasedIndex, index);
                x.SetValue(() => x.CanToggleOff, false); // let datagrid handle selection logic
                x.SetValue(() => x.CanToggleOn, false);
                x.SetChanged(() => x.UpdateLayoutTrigger); // trigger layout update

                if (!x.HeightSet)
                {
                    x.SetValue(() => x.Height, RowHeight);
                }

                if (x.IsHeader && HeaderRowImageSet)
                {
                    x.SetValue(() => x.BackgroundImage, HeaderRowImage);
                }
                else if (RowImageSet)
                {
                    x.SetValue(() => x.BackgroundImage, RowImage);
                }

                if (RowImageTypeSet)
                {
                    x.SetValue(() => x.BackgroundImageType, RowImageType);
                }

                if (x.IsHeader && HeaderRowColorSet)
                {
                    x.SetValue(() => x.BackgroundColor, HeaderRowColor);
                }
                else if (RowColorSet)
                {
                    x.SetValue(() => x.BackgroundColor, RowColor);
                }

                if (SelectedRowImageSet)
                {
                    x.SetValue(() => x.PressedImage, SelectedRowImage);
                }

                if (SelectedRowColorSet)
                {
                    x.SetValue(() => x.PressedColor, SelectedRowColor);
                }

                if (HighlightedRowImageSet)
                {
                    x.SetValue(() => x.HighlightedImage, HighlightedRowImage);
                }

                if (HighlightedRowColorSet)
                {
                    x.SetValue(() => x.HighlightedColor, HighlightedRowColor);
                }

                if (!x.IsInitialized)
                {
                    // add view action entry for click events
                    x.ViewActions.First(y => y.Name == "Click").Entries.Add(
                            new ViewActionEntry("RowClick", gameObject)
                        );

                    x.IsInitialized = true;
                }

                ++index;
            }, false);

            // set selected rows
            var previouslySelectedItems = new List<object>(_selectedItems);
            _selectedItems.Clear();
            if (index > 0)
            {
                // first time populating grid?
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
                            Debug.LogError(String.Format("[MarkUX.365] {0}: Unable to parse SelectedIndex=\"{1}\". Improperly formatted string. Expected a comma-separated list of integers.", Name, SelectedIndex));
                            selectedIndexArray = new int[0];
                        }

                        int itemIndex = 0;
                        this.ForEachChild<Row>(x =>
                        {
                            if (x.IsDestroyed)
                                return;

                            // select item
                            if (selectedIndexArray.Contains(x.ZeroBasedIndex))
                            {
                                x.ToggleValue = false; // make sure selection gets re-triggered when grid is initialized
                                SelectRow(x);
                            }

                            ++itemIndex;
                        }, false);
                    }
                }
                else
                {
                    // select previously selected items
                    this.ForEachChild<Row>(x =>
                    {
                        if (x.IsDestroyed)
                            return;

                        if (x.ToggleValue == false && previouslySelectedItems.Contains(x.IsDynamic ? x.Item : x))
                        {
                            SelectRow(x);
                        }
                    }, false);
                }
            }

            // set SortIndex on rows
            if (SortColumnIndexSet)
            {
                List<Row> rows = new List<Row>();
                this.ForEachChild<Row>(x => rows.Add(x), false);
                int sortIndex = 1;
                foreach (var row in rows.OrderBy(x => x.GetColumnText(SortColumnIndex)))
                {
                    if (row.IsHeader)
                        continue;

                    row.SortIndex = sortIndex;
                    ++sortIndex;
                }
            }

            // set spacing and orientation
            Spacing = RowSpacingSet ? RowSpacing : Spacing;
            Orientation = Orientation.Vertical;
            base.UpdateLayout();
        }

        /// <summary>
        /// Called when a row is clicked.
        /// </summary>
        public void RowClick(Row source)
        {
            if (source != null && source.IsHeader)
                return;

            SelectRow(source, true);
        }

        /// <summary>
        /// Selects row in datagrid.
        /// </summary>
        public void SelectRow(object objSource, bool userTriggered = false)
        {
            if (userTriggered && !CanSelect)
                return;

            // find item to select
            Row source = null;
            if (objSource is Row)
            {
                source = objSource as Row;
            }
            else
            {
                this.ForEachChild<Row>(x =>
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
                    SelectionChanged.Trigger(new DataGridSelectionActionData());                    
                }
            }
            else
            {
                // check if any other item is selected and unselect it
                if (!CanMultiSelect)
                {
                    this.ForEachChild<Row>(x =>
                    {
                        if (x == source || x.IsDestroyed)
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
                SelectionChanged.Trigger(new DataGridSelectionActionData { Row = source });
                _selectedItems.Add(source.IsDynamic ? source.Item : source); 
            }
        }
                
        /// <summary>
        /// Generate list items.
        /// </summary>
        private void GenerateRows()
        {
            // clear all items except row definition and static items
            this.ForEachChild<View>(x =>
            {
                if (x != _rowDefinition && x.IsDynamic && !x.IsDestroyed)
                {
                    x.Deactivate();
                    x.IsDestroyed = true;
                    GameObject.Destroy(x.gameObject);
                }
            }, false);

            // create header row            
            if (ShowHeaders && _rowDefinition != null)
            {
                HeaderRow.Activate();
            }
            else
            {
                HeaderRow.Deactivate();
            }

            // create new rows
            if (_rowDefinition != null && _rowDefinition.ColumnDefinitions.Count > 0)
            {
                for (int i = 0; i < _generatedItems.Count; ++i)
                {
                    Row row = CreateView<Row>(gameObject, this);
                    row.Id = String.Empty;
                    row.Name = String.Format("Row ({0})", _generatedItems[i] != null ? _generatedItems[i].GetType().Name : "null");
                    row.gameObject.name = row.Name;
                    row.RowDefinition = _rowDefinition;
                    row.Item = _generatedItems[i];

                    // create columns
                    foreach (var columnDef in _rowDefinition.ColumnDefinitions)
                    {
                        Column column = CreateView<Column>(row.gameObject, _parent);
                        column.Text = columnDef.Text;

                        // copy text field path binding (if any)
                        var fieldBinding = columnDef.FieldBindings.FirstOrDefault(x => x.SourceFieldPathString == "Text");
                        if (fieldBinding != null)
                        {
                            column.FieldBindings.Add(new FieldBinding(fieldBinding.SourceFieldPathString, fieldBinding.TargetFieldPathString, fieldBinding.Source, fieldBinding.Target, fieldBinding.FormatString, true));                            
                        }
                        
                        // create column content if any exists
                        if (columnDef.ContentContainer.transform.childCount > 0)
                        {
                            var columnContent = CreateViewFromTemplate(columnDef.ContentContainer, column.gameObject, _parent);
                            columnContent.Activate();
                        }
                    }

                    // update field bindings
                    row.ForEachChild<View>(x =>
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
                                fieldBinding.Target = row.gameObject;

                                // handle special case when binding refers to item index
                                if (fieldBinding.TargetFieldPathString.Equals("Item.Index", StringComparison.OrdinalIgnoreCase) ||
                                    fieldBinding.TargetFieldPathString.Equals("Item.ZeroBasedIndex", StringComparison.OrdinalIgnoreCase))
                                {
                                    fieldBinding.TargetFieldPathString = fieldBinding.TargetFieldPathString.Substring(5);
                                }

                                // add parent binding to item
                                var newBinding = new FieldBinding(fieldBinding.TargetFieldPathString, fieldBinding.SourceFieldPathString, row.gameObject, x.gameObject, fieldBinding.FormatString, true);
                                newBinding.Update = true;

                                // remove format string from source
                                fieldBinding.FormatString = String.Empty;
                                row.FieldBindings.Add(newBinding);
                            }
                        }
                    }, true, _parent);

                    row.InitializeViews();
                    row.UpdateViews();
                }
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

            bool rowDefinitionFound = false;          

            // look for row and column definitions
            this.ForEachChild<RowDefinition>(x =>
            {
                x.Deactivate();
                if (rowDefinitionFound)
                {
                    Debug.LogError(String.Format("[MarkUX.361] {0}: Only one row-definition is allowed in a datagrid. Using the first one found.", Name));
                    return;
                }

                rowDefinitionFound = true;
                _rowDefinition = x;
            }, false);

            HeaderRow.IsHeader = true;
            HeaderRow.RowDefinition = _rowDefinition;

            // set parent view
            _parent = Parent != null ? Parent.GetComponent<View>() : null;
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<DataGrid Spacing=""0"" Orientation=""Vertical"" CanDeselect=""False"">
                        <Row Id=""HeaderRow"" />
                     </DataGrid>";
        }

        #endregion
    }
}
