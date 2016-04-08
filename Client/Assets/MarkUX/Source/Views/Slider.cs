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
    /// Slider view.
    /// </summary>
    [InternalView]
    public class Slider : View
    {
        #region Fields

        public ViewAction ValueChanged;
        public ViewAction BeginDrag;
        public ViewAction EndDrag;
        public ViewAction Drag;
        public ViewAction InitializePotentialDrag;

        public ElementSize SliderHandleLength;
        public ElementSize SliderHandleBreadth;

        public bool SetValueOnDragEnded;

        [NotSetFromXml]
        public Image SHandle;

        [NotSetFromXml]
        public Region SSlideRegion;

        [NotSetFromXml]
        public Image SFill;

        [NotSetFromXml]
        public Region SFillRegion;

        [ChangeHandler("UpdateLayout")]
        public ElementSize Length;

        [ChangeHandler("UpdateLayout")]
        public ElementSize Breadth;

        [ChangeHandler("UpdateLayout")]
        public Orientation Orientation;

        [ChangeHandler("UpdateBehavior")]
        public Sprite SliderImage;

        [ChangeHandler("UpdateBehavior")]
        public Color SliderColor;

        [ChangeHandler("UpdateBehavior")]
        public UnityEngine.UI.Image.Type SliderImageType;

        [ChangeHandler("UpdateBehavior")]
        public Sprite SliderFillImage;

        [ChangeHandler("UpdateBehavior")]
        public Color SliderFillColor;

        [ChangeHandler("UpdateBehavior")]
        public Sprite SliderHandleImage;

        [ChangeHandler("UpdateBehavior")]
        public UnityEngine.UI.Image.Type SliderHandleImageType;

        [ChangeHandler("UpdateBehavior")]
        public Color SliderHandleColor;

        [ChangeHandler("UpdateBehavior")]
        public Margin SliderFillMargin;

        [ChangeHandler("UpdateBehavior")]
        public float Max;

        [ChangeHandler("UpdateBehavior")]
        public float Min;

        [ChangeHandler("UpdateBehavior")]
        public float Value;

        [ChangeHandler("UpdateBehavior")]
        public bool CanSlide;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class. 
        /// </summary>
        public Slider()
        {
            SliderImage = null;
            SliderFillImage = null;
            SliderHandleImage = null;
            Length = new ElementSize(4, ElementSizeUnit.Elements);
            Breadth = new ElementSize(1, ElementSizeUnit.Elements);
            SliderFillMargin = new Margin();
            SliderHandleLength = new ElementSize(20, ElementSizeUnit.Pixels);
            SliderHandleBreadth = new ElementSize(1, ElementSizeUnit.Percents);
            Min = 0;
            Max = 100;
            SetValueOnDragEnded = false;
            SliderColor = Color.white;
            SliderImageType = UnityEngine.UI.Image.Type.Simple;
            SliderFillColor = Color.white;
            Value = 0;
            Orientation = Orientation.Horizontal;
            CanSlide = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            Width = WidthSet ? Width : (Orientation == Orientation.Horizontal ? Length : Breadth);
            Height = HeightSet ? Height : (Orientation == Orientation.Horizontal ? Breadth : Length);

            base.UpdateLayout();

            // if vertical slider rotate slide region 90 degrees
            var transform = GetComponent<RectTransform>();
            if (Orientation == Orientation.Vertical)
            {
                SSlideRegion.Width = new ElementSize(transform.rect.height, ElementSizeUnit.Pixels);
                SSlideRegion.Height = new ElementSize(transform.rect.width, ElementSizeUnit.Pixels);
                SSlideRegion.Rotation = new Vector3(0, 0, 90);
                SSlideRegion.RotationSet = true;
                SSlideRegion.UpdateLayout();
            }
            SetSliderPosition(Value);
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();

            SetSliderPosition(Value);
        }

        /// <summary>
        /// Called on slider drag begin.
        /// </summary>
        /// <param name="eventData"></param>
        public void SliderBeginDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position);
        }

        /// <summary>
        /// Called on slider drag end.
        /// </summary>
        public void SliderEndDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position, true);
        }

        /// <summary>
        /// Called on slider drag.
        /// </summary>
        /// <param name="eventData"></param>
        public void SliderDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position);
        }

        /// <summary>
        /// Called on potential drag begin (click).
        /// </summary>
        /// <param name="eventData"></param>
        public void SliderInitializePotentialDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position);
        }

        /// <summary>
        /// Sets slider value.
        /// </summary>
        public void SlideTo(float value)
        {
            float clampedValue = value.Clamp(Min, Max);
            SetValue(() => Value, clampedValue);
        }

        /// <summary>
        /// Slides the slider to the given position.
        /// </summary>
        private void SetSlideTo(Vector2 mouseScreenPositionIn, bool isEndDrag = false)
        {
            var transform = SFillRegion.GetComponent<RectTransform>();

            // get canvas
            UnityEngine.Canvas canvas = RootCanvas.GetComponent<UnityEngine.Canvas>();

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mouseScreenPositionIn, canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);

            // calculate slide percentage (transform.position.x/y is center of fill area)
            float p = 0;
            float slideAreaLength = transform.rect.width - SliderHandleLength.Pixels;
            if (Orientation == Orientation.Horizontal)
            {
                p = ((mouseScreenPosition.x - transform.position.x + slideAreaLength / 2f) / slideAreaLength).Clamp(0, 1);
            }
            else
            {
                p = ((mouseScreenPosition.y - transform.position.y + slideAreaLength / 2f) / slideAreaLength).Clamp(0, 1);
            }

            // set value
            float newValue = (Max - Min) * p + Min;
            if (!SetValueOnDragEnded || (SetValueOnDragEnded && isEndDrag))
            {
                SetValue(() => Value, newValue);
                ValueChanged.Trigger();
            }
            else
            {
                SetSliderPosition(newValue);
            }
        }

        /// <summary>
        /// Sets slider position based on value.
        /// </summary>
        private void SetSliderPosition(float value)
        {
            float p = (value - Min) / (Max - Min);
            var fillTransform = SFillRegion.GetComponent<RectTransform>();

            // set handle offset
            float fillWidth = fillTransform.rect.width;
            float slideAreaWidth = fillWidth - SliderHandleLength.Pixels;
            float handleOffset = p * slideAreaWidth + SFillRegion.Margin.Left.Pixels;

            SHandle.OffsetFromParent = Margin.FromLeft(new ElementSize(handleOffset, ElementSizeUnit.Pixels));
            SHandle.UpdateLayout();

            // set fill percentage as to match the offset of the handle
            float fillP = (handleOffset + SliderHandleLength.Pixels / 2f) / fillWidth;
            SFill.Width = new ElementSize(fillP, ElementSizeUnit.Percents);
            SFill.UpdateLayout();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<Slider Length=""4em"" Breadth=""1em"" Orientation=""Horizontal"" Drag=""SliderDrag"" BeginDrag=""SliderBeginDrag"" EndDrag=""SliderEndDrag"" InitializePotentialDrag=""SliderInitializePotentialDrag"" SliderHandleLength=""20px"" SliderHandleBreadth=""100%"" Min=""0"" Max=""100"" SliderFillMargin=""0"">
                    <Region Id=""SSlideRegion"">
                        <Image Path=""{SliderImage}"" Type=""{SliderImageType}"" Color=""{SliderColor}"" />
                        <Region Id=""SFillRegion"" Margin=""{SliderFillMargin}"">
                            <Image Id=""SFill"" Path=""{SliderFillImage}"" Type=""{SliderImageType}"" Color=""{SliderFillColor}"" Alignment=""Left"" />
                        </Region>
                        <Image Id=""SHandle"" Path=""{SliderHandleImage}"" Type=""{SliderHandleImageType}"" Color=""{SliderHandleColor}"" Width=""{SliderHandleLength}"" Height=""{SliderHandleBreadth}"" Alignment=""Left"" />
                    </Region>
                </Slider>";
        }

        #endregion
    }
}
