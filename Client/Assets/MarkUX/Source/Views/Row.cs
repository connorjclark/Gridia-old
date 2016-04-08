#region Using Statements
using MarkUX.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Contains a row of items shown by the datagrid view.
    /// </summary>
    [InternalView]
    [CreatesView(typeof(Column))]
    public class Row : ContentView
    {
        #region Fields

        public ViewAction Click;
        public ViewAction MouseEnter;
        public ViewAction MouseExit;
        public ViewAction MouseDown;
        public ViewAction MouseUp;

        public int Index;
        public int ZeroBasedIndex;

        [NotSetFromXml]
        public Margin TextOffset;

        [NotSetFromXml]
        public Margin HeaderTextOffset;

        [NotSetFromXml]
        public Alignment TextAlignment;

        [NotSetFromXml]
        public Alignment HeaderTextAlignment;

        [NotSetFromXml]
        public Font Font;

        [NotSetFromXml]
        public FontStyle FontStyle;

        [NotSetFromXml]
        public int FontSize;

        [NotSetFromXml]
        public Color FontColor;

        [NotSetFromXml]
        public Color ShadowColor;

        [NotSetFromXml]
        public Vector2 ShadowDistance;

        [NotSetFromXml]
        public Color OutlineColor;

        [NotSetFromXml]
        public Vector2 OutlineDistance;

        [NotSetFromXml]
        public Font HeaderFont;

        [NotSetFromXml]
        public FontStyle HeaderFontStyle;

        [NotSetFromXml]
        public int HeaderFontSize;

        [NotSetFromXml]
        public Color HeaderFontColor;

        [NotSetFromXml]
        public Color HeaderShadowColor;

        [NotSetFromXml]
        public Vector2 HeaderShadowDistance;

        [NotSetFromXml]
        public Color HeaderOutlineColor;

        [NotSetFromXml]
        public Vector2 HeaderOutlineDistance;

        [NotSetFromXml]
        public ElementSize ColumnSpacing;

        [NotSetFromXml]
        public bool IsHeader;

        [NotSetFromXml]
        public object Item;

        [NotSetFromXml]
        public bool IsInitialized;

        [ChangeHandler("UpdateLayout")]
        public bool UpdateLayoutTrigger;

        private RowDefinition _rowDefinition;
        private bool _headerColumnsGenerated;

        [ChangeHandler("UpdateBehavior")]
        public bool Disabled;
        public bool DisabledSet;

        [ChangeHandler("UpdateBehavior")]
        public bool IsToggleButton;

        [ChangeHandler("UpdateBehavior")]
        public bool ToggleValue;

        public bool CanToggleOn;
        public bool CanToggleOff;

        // animation
        // color tint animation
        [ChangeHandler("UpdateBehavior")]
        public Color HighlightedColor;
        public bool HighlightedColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Color PressedColor;
        public bool PressedColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Color DisabledColor;
        public bool DisabledColorSet;

        // sprite swap animation
        [ChangeHandler("UpdateBehavior")]
        public Sprite HighlightedImage;
        public bool HighlightedImageSet;

        [ChangeHandler("UpdateBehavior")]
        public Sprite PressedImage;
        public bool PressedImageSet;

        [ChangeHandler("UpdateBehavior")]
        public Sprite DisabledImage;
        public bool DisabledImageSet;

        public ViewAnimation HighlightImageAnimation;
        public ViewAnimation PressedImageAnimation;
        public ViewAnimation HighlightColorAnimation;
        public ViewAnimation PressedColorAnimation;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Row()
        {
            ColumnSpacing = new ElementSize();

            HighlightedImage = null;
            PressedImage = null;
            DisabledImage = null;
            Disabled = false;
            IsToggleButton = false;
            ToggleValue = false;
            CanToggleOn = true;
            CanToggleOff = true;

            // animation
            // color tint animation
            HighlightedColor = Color.clear;
            PressedColor = Color.clear;
            DisabledColor = Color.clear;

            TextOffset = new Margin();
            HeaderTextOffset = new Margin();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            float defaultHeight = 1.5f;

            if (Application.isPlaying)
            {
                // is this a header row?
                if (IsHeader && !_headerColumnsGenerated && _rowDefinition != null)
                {
                    // yes. generate header columns
                    _headerColumnsGenerated = true;
                    _rowDefinition.ColumnDefinitions.ForEach(x =>
                    {
                        Column headerColumn = CreateView<Column>(gameObject, this);
                        headerColumn.Id = String.Empty;
                        headerColumn.Name = "Header Column";
                        headerColumn.gameObject.name = headerColumn.Name;
                        headerColumn.SetValue(() => headerColumn.Text, x.Header);

                        headerColumn.InitializeViews();
                        headerColumn.UpdateViews();
                    });
                }
            }

            // get size of content and set content offsets and alignment
            List<Column> columns = new List<Column>();
            this.ForEachChild<Column>(x => columns.Add(x), false);
            var rowHeight = (_rowDefinition != null && _rowDefinition.HeightSet) ? _rowDefinition.Height : Height;
            if (rowHeight.Unit == ElementSizeUnit.Percents)
            {
                rowHeight = new ElementSize(defaultHeight, ElementSizeUnit.Elements);
            }

            // set row height
            Height = new ElementSize(rowHeight);

            // set column values
            if (columns.Count > 0)
            {
                if (_rowDefinition == null || columns.Count > _rowDefinition.ColumnDefinitions.Count)
                {
                    if (_rowDefinition != null && _rowDefinition.ColumnDefinitions.Count > 0)
                    {
                        Debug.LogError("[MarkUX.363] Row contains more columns than specified by its rowdefinition.");
                    }

                    // adjust width of columns to actual width of row
                    float columnWidth = (ActualWidth - ((columns.Count - 1) * ColumnSpacing.Pixels)) / columns.Count;
                    columns.ForEach(x => x.Width = new ElementSize(columnWidth, ElementSizeUnit.Pixels));
                }
                else
                {
                    // adjust width of columns based on column definitions                    
                    float columnSpacing = ((columns.Count - 1) * ColumnSpacing.Pixels) / columns.Count;
                    List<Column> columnsToFill = new List<Column>();
                    float totalWidth = 0;
                    for (int i = 0; i < columns.Count; ++i)
                    {
                        var defWidth = _rowDefinition.ColumnDefinitions[i].Width;
                        if (!_rowDefinition.ColumnDefinitions[i].WidthSet || defWidth.Fill == true)
                        {
                            columnsToFill.Add(columns[i]);
                            continue;
                        }
                        else if (defWidth.Unit == ElementSizeUnit.Percents)
                        {
                            columns[i].Width = new ElementSize((defWidth.Percent * ActualWidth) - columnSpacing, ElementSizeUnit.Pixels);
                        }
                        else
                        {
                            columns[i].Width = new ElementSize(defWidth.Pixels - columnSpacing, ElementSizeUnit.Pixels);
                        }

                        totalWidth += columns[i].Width.Pixels;
                    }

                    // adjust width of fill columns
                    if (columnsToFill.Count > 0)
                    {
                        float columnWidth = Math.Max(columnSpacing, (ActualWidth - totalWidth) / columnsToFill.Count);
                        foreach (var column in columnsToFill)
                        {
                            column.Width = new ElementSize(columnWidth - columnSpacing, ElementSizeUnit.Pixels);
                        }
                    }
                }

                // set font, height, offsets and alignments
                float offset = 0;
                foreach (var column in columns)
                {
                    column.SetValue(() => column.TextAlignment, IsHeader ? HeaderTextAlignment : TextAlignment);
                    column.SetValue(() => column.TextOffset, IsHeader ? HeaderTextOffset : TextOffset);

                    column.SetValue(() => column.Font, IsHeader ? HeaderFont : Font);
                    column.SetValue(() => column.FontStyle, IsHeader ? HeaderFontStyle : FontStyle);
                    column.SetValue(() => column.FontSize, IsHeader ? HeaderFontSize : FontSize);
                    column.SetValue(() => column.FontColor, IsHeader ? HeaderFontColor : FontColor);
                    column.SetValue(() => column.ShadowColor, IsHeader ? HeaderShadowColor : ShadowColor);
                    column.SetValue(() => column.ShadowDistance, IsHeader ? HeaderShadowDistance : ShadowDistance);
                    column.SetValue(() => column.OutlineColor, IsHeader ? HeaderOutlineColor : OutlineColor);
                    column.SetValue(() => column.OutlineDistance, IsHeader ? HeaderOutlineDistance : OutlineDistance);

                    column.Height = new ElementSize(rowHeight);
                    column.Alignment = Alignment.TopLeft;
                    column.OffsetFromParent = new Margin(offset, 0, 0, 0);
                    offset += (column.Width.Pixels + ColumnSpacing.Pixels);

                    column.UpdateLayout();
                }
            }

            base.UpdateLayout();
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            // init animation values
            if (HighlightedImageSet)
            {
                SetChanged(() => HighlightedImage);
            }

            if (PressedImageSet)
            {
                SetChanged(() => PressedImage);
            }

            if (HighlightedColorSet)
            {
                SetChanged(() => HighlightedColor);
            }

            if (PressedColorSet)
            {
                SetChanged(() => PressedColor);
            }

            // handle disabled and toggle states
            UpdateBackground = !(DisabledSet || IsToggleButton);
            if (DisabledSet)
            {
                if (Disabled)
                {
                    SetBackground(DisabledImage, null, DisabledColor, DisabledColorSet);
                }
                else
                {
                    SetBackground();
                }
            }
            else if (IsToggleButton)
            {
                if (ToggleValue)
                {
                    SetBackground(PressedImage, null, PressedColor, PressedColorSet);
                }
                else
                {
                    SetBackground();
                }
            }

            base.UpdateBehavior();
        }

        /// <summary>
        /// Called on row click.
        /// </summary>
        public void RowMouseClick()
        {
            // if toggle-button change state
            if (IsToggleButton)
            {
                if (ToggleValue == true && !CanToggleOff)
                    return;
                if (ToggleValue == false && !CanToggleOn)
                    return;

                SetValue(() => ToggleValue, !ToggleValue);
            }
        }

        /// <summary>
        /// Calledon row mouse enter.
        /// </summary>
        public void RowMouseEnter()
        {
            if (!Disabled)
            {
                if (HighlightedImageSet)
                {
                    HighlightImageAnimation.StartAnimation();
                }

                if (HighlightedColorSet)
                {
                    HighlightColorAnimation.StartAnimation();
                }
            }
        }

        /// <summary>
        /// Called on row mouse exit.
        /// </summary>
        public void ButtonMouseExit()
        {
            if (!Disabled)
            {
                if (HighlightedImageSet)
                {
                    HighlightImageAnimation.ReverseAnimation();
                }

                if (HighlightedColorSet)
                {
                    HighlightColorAnimation.ReverseAnimation();
                }
            }
        }

        /// <summary>
        /// Called on row mouse down.
        /// </summary>
        public void ButtonMouseDown()
        {
            if (!Disabled)
            {
                if (PressedImageSet)
                {
                    PressedImageAnimation.StartAnimation();
                }

                if (PressedColorSet)
                {
                    PressedColorAnimation.StartAnimation();
                }
            }
        }

        /// <summary>
        /// Called on row mouse up.
        /// </summary>
        public void ButtonMouseUp()
        {
            if (!Disabled)
            {
                if (PressedImageSet)
                {
                    PressedImageAnimation.ReverseAnimation();
                }

                if (PressedColorSet)
                {
                    PressedColorAnimation.ReverseAnimation();
                }
            }
        }

        /// <summary>
        /// Gets column text.
        /// </summary>
        public string GetColumnText(int columnIndex)
        {
            string text = String.Empty;
            int index = 0;
            this.ForEachChild<Column>(x =>
            {
                if (index == columnIndex)
                {
                    text = x.Text;
                }
                ++index;
            }, false);
            return text;
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Row IsToggleButton=""True"" CanToggleOff=""False"" Click=""RowMouseClick"" MouseEnter=""RowMouseEnter"" MouseExit=""RowMouseExit"" MouseDown=""RowMouseDown"" MouseUp=""RowMouseUp"">
                        <ViewAnimation Id=""HighlightImageAnimation"">
                            <Animate Field=""BackgroundImage"" To=""{HighlightedImage}"" />
                        </ViewAnimation>
                        <ViewAnimation Id=""PressedImageAnimation"">
                            <Animate Field=""BackgroundImage"" To=""{PressedImage}"" />
                        </ViewAnimation>
                        <ViewAnimation Id=""HighlightColorAnimation"">
                            <Animate Field=""BackgroundColor"" To=""{HighlightedColor}"" ReverseSpeed=""0.25"" Duration=""0.1"" />
                        </ViewAnimation>
                        <ViewAnimation Id=""PressedColorAnimation"">
                            <Animate Field=""BackgroundColor"" To=""{PressedColor}"" />
                        </ViewAnimation>
                    </Row>";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets row definition.
        /// </summary>
        public RowDefinition RowDefinition
        {
            get
            {
                return _rowDefinition;
            }
            set
            {
                _rowDefinition = value;
            }
        }

        #endregion
    }
}
