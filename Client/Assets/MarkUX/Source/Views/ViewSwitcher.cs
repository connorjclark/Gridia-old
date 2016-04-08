#region Using Statements
using MarkUX.ValueConverters;
using System;
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
    /// Switches between views.
    /// </summary>
    [InternalView]
    public class ViewSwitcher : ContentView
    {
        #region Fields

        public string StartView;
        public bool StartViewSet;

        public bool SwitchToDefault;

        [ChangeHandler("UpdateBehavior")]
        public string TransitionIn;

        [ChangeHandler("UpdateBehavior")]
        public string TransitionOut;

        [ChangeHandler("UpdateBehavior")]
        public ViewAnimation TransitionInAnimation;

        [ChangeHandler("UpdateBehavior")]
        public ViewAnimation TransitionOutAnimation;
                
        [NotSetFromXml]
        public int ViewCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewSwitcher()
        {
            SwitchToDefault = true;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initializes the view switcher.
        /// </summary>
        public override void Initialize()
        {
            if (!StartViewSet)
            {
                if (SwitchToDefault)
                {
                    SwitchTo(0, false);
                }
                else
                {
                    // deactive all views
                    ContentContainer.ForEachChild<View>(x => x.Deactivate(), false);
                }
            }
            else
            {
                SwitchTo(StartView, false);
            }
        }

        /// <summary>
        /// Switches to another view.
        /// </summary>
        public void SwitchTo(View view, bool animate = true)
        {
            ContentContainer.ForEachChild<View>(x => SwitchTo(x, x == view, animate), false);
        }

        /// <summary>
        /// Switches to another view.
        /// </summary>
        public void SwitchTo(string id, bool animate = true)
        {
            ContentContainer.ForEachChild<View>(x => SwitchTo(x, String.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase), animate), false);
        }

        /// <summary>
        /// Switches to another view.
        /// </summary>
        public void SwitchTo(int index, bool animate = true)
        {
            int i = 0;
            ContentContainer.ForEachChild<View>(x =>
            {
                SwitchTo(x, index == i, animate);
                ++i;
            }, false);
        }

        /// <summary>
        /// Switches to view.
        /// </summary>
        private void SwitchTo(View view, bool active, bool animate)
        {
            if (!active && view.Enabled && animate)
            {
                if (TransitionOutAnimation)
                {
                    TransitionOutAnimation.SetAnimationTarget(view.gameObject);
                    TransitionOutAnimation.StartAnimation();
                }
                else
                {
                    view.Deactivate(true);
                }
            }

            if (!animate)
            {
                if (active)
                {
                    view.Activate();
                }
                else
                {
                    view.Deactivate(true);
                }
            }

            // set animation target and start transition-in animation
            if (active && animate)
            {
                if (TransitionInAnimation != null)
                {
                    TransitionInAnimation.SetAnimationTarget(view.gameObject);
                    TransitionInAnimation.StartAnimation();
                }
                view.Activate();
            }
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();

            if (!String.IsNullOrEmpty(TransitionIn))
            {
                TransitionInAnimation = this.GetLayoutRoot().FindView<ViewAnimation>(TransitionIn);
            }

            if (!String.IsNullOrEmpty(TransitionOut))
            {
                TransitionOutAnimation = this.GetLayoutRoot().FindView<ViewAnimation>(TransitionOut);
            }

            ViewCount = 0;
            ContentContainer.ForEachChild<View>(x =>
            {
                ++ViewCount;
            }, false);
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<ViewSwitcher SwitchToDefault=""True"">
                    <ContentContainer ResizeToContent=""False"" />
                </ViewSwitcher>";
        }

        #endregion
    }
}
