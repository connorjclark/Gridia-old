#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MarkUX.Animation;
using MarkUX.ValueConverters;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// ComboBox view.
    /// </summary>
    [InternalView]
    public class ComboBox : ContentView
    {
        #region Fields

        public ViewAction SelectionChanged;

        [GenericListValueConverter]
        public List<object> Items;
        
        public Button ComboBoxButton;
        public List ComboBoxList;
        public ChildCanvas ComboBoxListCanvas;

        // combo-box button
        public Margin ButtonMargin;
        public Color ButtonColor;
        public Sprite ButtonImage;
        public UnityEngine.UI.Image.Type ButtonImageType;
        public Color ButtonPressedColor;
        public Sprite ButtonPressedImage;
        public Color ButtonHighlightedColor;
        public Sprite ButtonHighlightedImage;

        // combo-box button header
        public Font ButtonFont;
        public Margin ButtonTextMargin;
        public FontStyle ButtonFontStyle;
        public int ButtonFontSize;
        public Color ButtonFontColor;
        public Alignment ButtonTextAlignment;
        public Color ButtonShadowColor;
        public Vector2 ButtonShadowDistance;
        public Color ButtonOutlineColor;
        public Vector2 ButtonOutlineDistance;

        // combo-box list
        public Mask ListMask;
        public Margin ListMargin;
        public Sprite ListMaskImage;
        public UnityEngine.UI.Image.Type ListMaskImageType;
        public Color ListMaskColor;
        public Color ListBackgroundColor;
        public Sprite ListBackgroundImage;
        public UnityEngine.UI.Image.Type ListBackgroundImageType;

        // combo-box list item width/height
        public ElementSize ItemWidth;
        public ElementSize ItemHeight;

        // combo-box list images
        public Color ItemColor;
        public Sprite ItemImage;
        public UnityEngine.UI.Image.Type ItemImageType;
        public Color SelectedItemColor;
        public Sprite SelectedItemImage;
        public Color HighlightedItemColor;
        public Sprite HighlightedItemImage;

        // combo-box list item header
        public Font ItemFont;
        public Margin ItemTextMargin;
        public FontStyle ItemFontStyle;
        public int ItemFontSize;
        public Color ItemFontColor;
        public Alignment ItemTextAlignment;
        public Color ItemShadowColor;
        public Vector2 ItemShadowDistance;
        public Color ItemOutlineColor;
        public Vector2 ItemOutlineDistance;

        private UnityEngine.Canvas _canvas;
        private RectTransform _rectTransform;
        private object _selectedItem;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ComboBox()
        {
#if UNITY_4_6_0
            Debug.LogError("[MarkUX.373] Due to a bug in Unity 4.6.0 (653443) the ComboBox will not work correctly. The bug has been resolved in Unity 4.6.1p1.");
#endif
            ButtonTextMargin = new Margin();
            ButtonMargin = new Margin();
            ListMaskColor = new Color(1, 1, 1, 0);
            ListMargin = new Margin();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets currently selected item.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called each frame. Updates the view.
        /// </summary>
        public void Update()
        {
            // if list is open check if user has clicked outside
            if (ComboBoxList.Enabled)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    // get mouse screen position
                    Vector2 mouseScreenPosition = _canvas.GetMouseScreenPosition(Input.mousePosition); 

                    // get rect of combo-box
                    Rect rect = new Rect(_rectTransform.position.x - ActualWidth / 2f, _rectTransform.position.y - ActualHeight / 2f, ActualWidth, ActualHeight);

                    // check if mouse pointer outside the combo-box
                    if (!rect.Contains(mouseScreenPosition))
                    {
                        ComboBoxList.Deactivate();
                        ComboBoxButton.SetValue(() => ComboBoxButton.ToggleValue, false);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            base.UpdateLayout();

            ComboBoxListCanvas.OffsetFromParent = new Margin(0, ActualHeight, 0, 0);
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();
        }

        /// <summary>
        /// Called when mouse is clicked.
        /// </summary>
        public void ComboBoxButtonClick(Button source)
        {
            // toggle combo-box list
            if (source.ToggleValue)
            {
                ComboBoxList.Activate();
            }
            else
            {
                ComboBoxList.Deactivate();
            }
        }

        /// <summary>
        /// Called when combo-box list selection changes.
        /// </summary>
        public void ComboBoxListSelectionChanged(ListSelectionActionData item)
        {
            // close list and set selected item text
            ComboBoxButton.SetValue(() => ComboBoxButton.ToggleValue, false);
            ComboBoxButton.SetValue(() => ComboBoxButton.Text, item.ListItem != null ? item.ListItem.Text : String.Empty);
            ComboBoxList.Deactivate();

            // trigger selection changed action
            _selectedItem = item.ListItem.Item;
            SelectionChanged.Trigger(item);
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _rectTransform = GetComponent<RectTransform>();
            _canvas = RootCanvas.GetComponent<UnityEngine.Canvas>();
        }

        /// <summary>
        /// Gets embedded XML of the view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<ComboBox Width=""4em"" Height=""1em"">
                    <Button Style=""ComboBoxButton"" Id=""ComboBoxButton"" IsToggleButton=""True"" Width=""100%"" Height=""100%"" Click=""ComboBoxButtonClick"" Margin=""{ButtonMargin}""
                        BackgroundColor=""{ButtonColor}"" BackgroundImage=""{ButtonImage}"" BackgroundImageType=""{ButtonImageType}"" PressedColor=""{ButtonPressedColor}"" 
                        PressedImage=""{ButtonPressedImage}"" HighlightedColor=""{ButtonHighlightedColor}"" HighlightedImage=""{ButtonHighlightedImage}""
                        Font=""{ButtonFont}""
                        TextMargin=""{ButtonTextMargin}""
                        FontStyle=""{ButtonFontStyle}""
                        FontSize=""{ButtonFontSize}""
                        FontColor=""{ButtonFontColor}""
                        TextAlignment=""{ButtonTextAlignment}""
                        ShadowColor=""{ButtonShadowColor}""
                        ShadowDistance=""{ButtonShadowDistance}""
                        OutlineColor=""{ButtonOutlineColor}""
                        OutlineDistance=""{ButtonOutlineDistance}""
                        />
                    <ChildCanvas Alignment=""Top"" Id=""ComboBoxListCanvas"" OverrideSort=""True"" SortingOrder=""10"">
                        <List Style=""ComboBoxList"" Id=""ComboBoxList"" Width=""100%"" Margin=""{ListMargin}"" Items=""{Items}"" Alignment=""Top"" Enabled=""False"" SelectionChanged=""ComboBoxListSelectionChanged""
                              ListMaskImage=""{ListMaskImage}"" ListMaskImageType=""{ListMaskImageType}"" ListMaskColor=""{ListMaskColor}""
                              BackgroundImage=""{ListBackgroundImage}"" BackgroundImageType=""{ListBackgroundImageType}"" BackgroundColor=""{ListBackgroundColor}""
                              ItemWidth=""{ItemWidth}"" ItemHeight=""{ItemHeight}""
                              ItemColor=""{ItemColor}"" ItemImage=""{ItemImage}"" ItemImageType=""{ItemImageType}"" SelectedItemColor=""{SelectedItemColor}"" 
                              SelectedItemImage=""{SelectedItemImage}"" HighlightedItemColor=""{HighlightedItemColor}"" HighlightedItemImage=""{HighlightedItemImage}""
                              ItemFont=""{ItemFont}""
                              ItemTextMargin=""{ItemTextMargin}""
                              ItemFontStyle=""{ItemFontStyle}""
                              ItemFontSize=""{ItemFontSize}""
                              ItemFontColor=""{ItemFontColor}""
                              ItemTextAlignment=""{ItemTextAlignment}""
                              ItemShadowColor=""{ItemShadowColor}""
                              ItemShadowDistance=""{ItemShadowDistance}""
                              ItemOutlineColor=""{ItemOutlineColor}""
                              ItemOutlineDistance=""{ItemOutlineDistance}"">

                            <ContentContainer IncludeContainerInContent=""False"" />
                        </List>
                    </ChildCanvas>                    
                </ComboBox>";
        }

        #endregion
    }
}
