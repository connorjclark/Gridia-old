#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Displays content in a window.
    /// </summary>
    [InternalView]
    public class Window : ContentView
    {
        #region Fields

        public ViewAction WindowClosed;
        public bool IsClosable;
        public bool IsMovable;
        public Margin ContentMargin;

        // window
        public Region WindowRegion;
        public ElementSize WindowWidth;
        public ElementSize WindowHeight;
        public Color WindowBackgroundColor;
        public Sprite WindowBackgroundImage;
        public UnityEngine.UI.Image.Type WindowBackgroundImageType;
        public Material WindowBackgroundMaterial;

        // window title label
        public string Title;
        public ElementSize TitleHeight;
        public Font TitleFont;
        public Margin TitleTextMargin;
        public FontStyle TitleFontStyle;
        public int TitleFontSize;
        public Color TitleFontColor;
        public Alignment TitleTextAlignment;
        public Color TitleShadowColor;
        public Vector2 TitleShadowDistance;
        public Color TitleOutlineColor;
        public Vector2 TitleOutlineDistance;

        // window title region
        public Alignment TitleAlignment;
        public Margin TitleOffset;
        public Margin TitleRegionOffset;
        public ElementSize TitleRegionWidth;
        public ElementSize TitleRegionHeight;
        public Color TitleRegionBackgroundColor;
        public Sprite TitleRegionBackgroundImage;
        public UnityEngine.UI.Image.Type TitleRegionBackgroundImageType;
        public Material TitleRegionBackgroundMaterial;

        // close button
        public Alignment CloseButtonAlignment;
        public Button WindowCloseButton;
        public ElementSize CloseButtonWidth;
        public ElementSize CloseButtonHeight;
        public Margin CloseButtonOffset;
        public Color CloseButtonHighlightedColor;
        public Color CloseButtonPressedColor;
        public Color CloseButtonColor;
        public Sprite CloseButtonImage;
        public Sprite CloseButtonHighlightedImage;
        public Sprite CloseButtonPressedImage;
        public UnityEngine.UI.Image.Type CloseButtonImageType;

        private Vector2 _initialWindowOffset;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Window()
        {
#if UNITY_4_6_0
            Debug.LogError("[MarkUX.376] Due to a bug in Unity 4.6.0 (653443) the Window will not work correctly. The bug has been resolved in Unity 4.6.1p1.");
#endif

            IsClosable = false;
            IsMovable = true;
            ContentMargin = new Margin();

            // window
            WindowWidth = new ElementSize(300, ElementSizeUnit.Pixels);
            WindowHeight = new ElementSize(300, ElementSizeUnit.Pixels);
            WindowBackgroundColor = Color.clear;
            WindowBackgroundImage = null;
            WindowBackgroundImageType = UnityEngine.UI.Image.Type.Simple;
            WindowBackgroundMaterial = null;

            // window title
            Title = String.Empty;
            TitleHeight = new ElementSize(1, ElementSizeUnit.Percents);
            TitleFont = null;
            TitleAlignment = Alignment.Center;
            TitleOffset = new Margin();
            TitleRegionOffset = new Margin();
            TitleRegionWidth = new ElementSize(1, ElementSizeUnit.Percents);
            TitleRegionHeight = new ElementSize(1, ElementSizeUnit.Elements);
            TitleRegionBackgroundColor = Color.clear;
            TitleRegionBackgroundImage = null;
            TitleRegionBackgroundImageType = UnityEngine.UI.Image.Type.Simple;
            TitleRegionBackgroundMaterial = null;

            // close button
            CloseButtonAlignment = Alignment.TopRight;
            CloseButtonWidth = new ElementSize(18, ElementSizeUnit.Pixels);
            CloseButtonHeight = new ElementSize(18, ElementSizeUnit.Pixels);
            CloseButtonOffset = new Margin();
            CloseButtonColor = Color.clear;
            CloseButtonHighlightedColor = Color.clear;
            CloseButtonPressedColor = Color.clear;
            CloseButtonImage = null;
            CloseButtonHighlightedImage = null;
            CloseButtonPressedImage = null;
            CloseButtonImageType = UnityEngine.UI.Image.Type.Simple;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();
            WindowCloseButton.SetValue(() => WindowCloseButton.Enabled, IsClosable);  
        }

        /// <summary>
        /// Called when window close button is clicked.
        /// </summary>
        public void WindowCloseButtonClick()
        {
            WindowClosed.Trigger();
            Deactivate();
        }

        /// <summary>
        /// Called on window drag begin.
        /// </summary>
        public void WindowBeginDrag(PointerEventData eventData)
        {
            if (!IsMovable)
            {
                return;
            }

            // get canvas
            UnityEngine.Canvas canvas = RootCanvas.GetComponent<UnityEngine.Canvas>();

            // calculate window offset
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);
            _initialWindowOffset.x = (mouseScreenPosition.x - transform.position.x) - WindowRegion.Offset.Left.Pixels;
            _initialWindowOffset.y = -(mouseScreenPosition.y - transform.position.y) - WindowRegion.Offset.Top.Pixels;            
        }

        /// <summary>
        /// Called on window drag end.
        /// </summary>
        public void WindowEndDrag(PointerEventData eventData)
        {
        }

        /// <summary>
        /// Called on window drag.
        /// </summary>
        /// <param name="eventData"></param>
        public void WindowDrag(PointerEventData eventData)
        {
            if (!IsMovable)
            {
                return;
            }

            // calculate the position of the window based on offset from initial click position
            Vector2 offset;

            // get canvas
            UnityEngine.Canvas canvas = RootCanvas.GetComponent<UnityEngine.Canvas>();

            // calculate window offset
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);
            offset.x = mouseScreenPosition.x - transform.position.x;
            offset.y = -(mouseScreenPosition.y - transform.position.y);

            // set window offset
            WindowRegion.SetValue(() => WindowRegion.Offset, new Margin(offset.x - _initialWindowOffset.x, offset.y - _initialWindowOffset.y));
        }

        /// <summary>
        /// Called on potential drag begin (click).
        /// </summary>
        /// <param name="eventData"></param>
        public void WindowInitializePotentialDrag(PointerEventData eventData)
        {
        }

        /// <summary>
        /// Gets embedded XML for the view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Window>
                      <ChildCanvas Alignment=""Top"" Id=""WindowCanvas"" OverrideSort=""True"" SortingOrder=""5"">
                        <Region Id=""WindowRegion"" Width=""{WindowWidth}"" Height=""{WindowHeight}"" BackgroundColor=""{WindowBackgroundColor}"" 
                              BackgroundImage=""{WindowBackgroundImage}"" BackgroundImageType=""{WindowBackgroundImageType}"" 
                              BackgroundMaterial=""{WindowBackgroundMaterial}"">
                          <InteractableRegion Id=""TitleRegion"" Width=""{TitleRegionWidth}"" Height=""{TitleRegionHeight}"" Offset=""{TitleRegionOffset}""
                              BackgroundColor=""{TitleRegionBackgroundColor}"" BackgroundImage=""{TitleRegionBackgroundImage}""
                              BackgroundImageType=""{TitleRegionBackgroundImageType}"" BackgroundMaterial=""{TitleRegionBackgroundMaterial}"" Alignment=""Top""
                              Drag=""WindowDrag"" BeginDrag=""WindowBeginDrag"" EndDrag=""WindowEndDrag"" InitializePotentialDrag=""WindowInitializePotentialDrag"">
                            <Label Text=""{Title}"" Alignment=""{TitleAlignment}"" Offset=""{TitleOffset}"" Font=""{TitleFont}"" AdjustToText=""Width"" Height=""{TitleHeight}""
                                   FontStyle=""{TitleFontStyle}"" FontSize=""{TitleFontSize}"" FontColor=""{TitleFontColor}"" TextAlignment=""{TitleTextAlignment}"" ShadowColor=""{TitleShadowColor}"" ShadowDistance=""{TitleShadowDistance}"" OutlineColor=""{TitleOutlineColor}"" OutlineDistance=""{TitleOutlineDistance}""
                                   />
                          </InteractableRegion>
                          <ContentContainer Margin=""{ContentMargin}"" />

                          <Button Id=""WindowCloseButton"" BackgroundImage=""{CloseButtonImage}"" BackgroundColor=""{CloseButtonColor}""
                            BackgroundImageType=""{CloseButtonImageType}""
                            HighlightedColor=""{CloseButtonHighlightedColor}""
                            PressedColor=""{CloseButtonPressedColor}"" HighlightedImage=""{CloseButtonHighlightedImage}""
                            PressedImage=""{CloseButtonPressedImage}"" Click=""WindowCloseButtonClick"" Alignment=""{CloseButtonAlignment}"" Offset=""{CloseButtonOffset}""
                            Width=""{CloseButtonWidth}"" Height=""{CloseButtonHeight}"" />

                        </Region>
                      </ChildCanvas>
                    </Window>";
        }
        
        #endregion
    }
}
