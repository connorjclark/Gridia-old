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
    /// Radial menu view.
    /// </summary>
    [InternalView]
    public class RadialMenu : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateBehavior")]
        public ElementSize Radius;

        [ChangeHandler("UpdateBehavior")]
        public float StartAngle;

        [ChangeHandler("UpdateBehavior")]
        public float EndAngle;

        [ChangeHandler("UpdateBehavior")]
        [DurationValueConverter]
        public float AnimationDuration;
        public bool AnimationDurationSet;

        private bool _isOpen;
        private List<ViewFieldAnimator> _viewFieldAnimators;
        private List<View> _menuItems;
        private List<View> _deactivatedMenuItems;
        private Vector2 _menuOffset;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public RadialMenu()
        {
            _isOpen = false;
            Radius = new ElementSize(100, ElementSizeUnit.Pixels);
            StartAngle = 0;
            EndAngle = 360;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates view field animators.
        /// </summary>
        public void Update()
        {
            _viewFieldAnimators.ForEach(x =>
            {
                x.Update();
                if (x.Completed && !_isOpen)
                {
                    x.View.Deactivate();
                }
            });
        }

        /// <summary>
        /// Toggles radial menu.
        /// </summary>
        public void Toggle(bool animate = true)
        {
            if (_isOpen)
            {
                Close(animate);
            }
            else
            {
                Open(animate);
            }
        }

        /// <summary>
        /// Toggles radial menu.
        /// </summary>
        public void ToggleAt(Vector2 position, bool animate = true)
        {
            if (_isOpen)
            {
                Close(animate);
            }
            else
            {
                OpenAt(position, animate);
            }
        }

        /// <summary>
        /// Opens radial menu at position.
        /// </summary>
        public void OpenAt(Vector2 mouseScreenPositionIn, bool animate = true)
        {            
            // get canvas
            UnityEngine.Canvas canvas = RootCanvas.GetComponent<UnityEngine.Canvas>();

            // calculate menu offset
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mouseScreenPositionIn, canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);
            _menuOffset.x = mouseScreenPosition.x - transform.position.x;
            _menuOffset.y = -(mouseScreenPosition.y - transform.position.y);
            UpdateMenu();

            Open(animate);
        }

        /// <summary>
        /// Opens the radial menu.
        /// </summary>
        public void Open(bool animate = true)
        {
            _isOpen = true;

            // activate views 
            if (!animate)
            {
                int childCount = _menuItems.Count() - _deactivatedMenuItems.Count();
                if (childCount > 0)
                {
                    float deltaAngle = Mathf.Deg2Rad * ((EndAngle - StartAngle) / childCount);
                    float angle = Mathf.Deg2Rad * StartAngle;

                    foreach (var child in _menuItems)
                    {    
                        if (_deactivatedMenuItems.Contains(child))
                        {
                            continue;
                        }

                        // set offset
                        float xOffset = Radius.Pixels * Mathf.Sin(angle);
                        float yOffset = Radius.Pixels * Mathf.Cos(angle);

                        child.OffsetFromParent = new Margin(xOffset + _menuOffset.x, -yOffset + _menuOffset.y, 0, 0);
                        child.Alignment = Alignment.Center;
                        child.Activate();
                        child.UpdateLayout();
                        angle += deltaAngle;
                    }
                }
            }
            else
            {
                _menuItems.ForEach(x =>
                {
                    if (!_deactivatedMenuItems.Contains(x))
                    {
                        x.Activate();
                    }
                });

                _viewFieldAnimators.ForEach(x => x.StartAnimation());
            }
        }

        /// <summary>
        /// Closes the radial menu.
        /// </summary>
        public void Close(bool animate = true)
        {
            _isOpen = false;

            // deactivate views
            if (animate)
            {
                _viewFieldAnimators.ForEach(x => x.ReverseAnimation());
            }
            else
            {
                _menuItems.ForEach(x =>
                {
                    x.Deactivate();
                });
            }
        }

        /// <summary>
        /// Activates a view within the radial menu.
        /// </summary>
        public void ActivateMenuItem(string id)
        {
            var view = ContentContainer.FindView(id, false);
            if (view == null)
            {
                Debug.LogError(String.Format("[MarkUX.371] {0}: Unable to activate menu item. Menu item \"{1}\" not found.", Name, id));
                return;
            }

            ActivateMenuItem(view.GetComponent<View>());
        }

        /// <summary>
        /// Activates a view within the radial menu.
        /// </summary>
        public void ActivateMenuItem(int index)
        {
            if (index >= _menuItems.Count() || index < 0)
            {
                Debug.LogError(String.Format("[MarkUX.370] {0}: Unable to activate menu item. Index out of range.", Name));
                return;
            }

            DeactivateMenuItem(_menuItems[index]);
        }

        /// <summary>
        /// Activates a view within the radial menu.
        /// </summary>
        public void ActivateMenuItem(View view)
        {
            if (_deactivatedMenuItems.Contains(view))
            {
                _deactivatedMenuItems.Remove(view);
                UpdateMenu();
            }            
        }

        /// <summary>
        /// Deactivates a view within the radial-menu.
        /// </summary>
        public void DeactivateMenuItem(string id)
        {
            var view = ContentContainer.FindView(id, false);
            if (view == null)
            {
                Debug.LogError(String.Format("[MarkUX.369] {0}: Unable to deactivate menu item. Menu item \"{1}\" not found.", Name, id));
                return;
            }

            DeactivateMenuItem(view.GetComponent<View>());
        }

        /// <summary>
        /// Deactivates a view within the radial-menu.
        /// </summary>
        public void DeactivateMenuItem(int index)
        {
            if (index >= _menuItems.Count() || index < 0)
            {
                Debug.LogError(String.Format("[MarkUX.368] {0}: Unable to deactivate menu item. Index out of range.", Name));
                return;
            }

            DeactivateMenuItem(_menuItems[index]);
        }

        /// <summary>
        /// Deactivates a view within the radial-menu.
        /// </summary>
        public void DeactivateMenuItem(View view)
        {
            if (!_deactivatedMenuItems.Contains(view))
            {
                _deactivatedMenuItems.Add(view);
                UpdateMenu();
            }            
        }

        /// <summary>
        /// Updates menu views and offset animators.
        /// </summary>
        public void UpdateMenu()
        {
            _viewFieldAnimators.Clear();
            int activeChildCount = _menuItems.Count() - _deactivatedMenuItems.Count();
            if (activeChildCount > 0)
            {
                float deltaAngle = Mathf.Deg2Rad * ((EndAngle - StartAngle) / activeChildCount);
                float angle = Mathf.Deg2Rad * StartAngle;

                foreach (var child in _menuItems)
                {
                    if (_deactivatedMenuItems.Contains(child))
                    {
                        continue;
                    }

                    // calculate offset
                    float xOffset = Radius.Pixels * Mathf.Sin(angle);
                    float yOffset = Radius.Pixels * Mathf.Cos(angle);

                    // set offset animator
                    var offsetAnimator = new ViewFieldAnimator();
                    offsetAnimator.EasingFunction = EasingFunctionType.Linear;
                    offsetAnimator.Field = "OffsetFromParent";
                    offsetAnimator.From = new Margin(_menuOffset.x, _menuOffset.y);
                    offsetAnimator.To = new Margin(xOffset + _menuOffset.x, -yOffset + _menuOffset.y, 0, 0);
                    offsetAnimator.Duration = AnimationDurationSet ? AnimationDuration : 0.2f;
                    offsetAnimator.SetAnimationTarget(child);
                    _viewFieldAnimators.Add(offsetAnimator);

                    child.OffsetFromParent = new Margin(_menuOffset.x, _menuOffset.y, 0, 0);
                    child.Alignment = Alignment.Center;
                    child.Deactivate();
                    child.UpdateLayout();
                    angle += deltaAngle;
                }
            }

            foreach (var deactivatedItem in _deactivatedMenuItems)
            {
                deactivatedItem.Deactivate();
                deactivatedItem.UpdateLayout();
            }
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _deactivatedMenuItems = new List<View>();
            _menuOffset = new Vector2();

            // create animators for each view
            _viewFieldAnimators = new List<ViewFieldAnimator>();
            _menuItems = ContentContainer.GetChildren<View>(false);

            UpdateMenu();            
            Close(false);
        }

        /// <summary>
        /// Gets embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<RadialMenu>
                    <ContentContainer />
                </RadialMenu>";
        }

        #endregion
    }
}
