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
    /// Animates views.
    /// </summary>
    [InternalView]
    public class ViewAnimation : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateBehavior")]
        public GameObject Target;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a boolean indicating whether this animation is active.
        /// </summary>
        public virtual bool IsAnimationActive
        {
            get
            {
                bool isActive = false;
                this.ForEachChild<ViewAnimation>(x => isActive = isActive || x.IsAnimationActive, false);
                return isActive;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is reversing.
        /// </summary>
        public virtual bool IsAnimationReversing
        {
            get
            {
                bool isReversing = false;
                this.ForEachChild<ViewAnimation>(x => isReversing = isReversing || x.IsAnimationReversing, false);
                return isReversing;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is completed.
        /// </summary>
        public virtual bool IsAnimationCompleted
        {
            get
            {
                bool isCompleted = false;
                this.ForEachChild<ViewAnimation>(x => isCompleted = isCompleted && x.IsAnimationCompleted, false);
                return isCompleted;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is paused.
        /// </summary>
        public virtual bool IsAnimationPaused
        {
            get
            {
                bool isPaused = false;
                this.ForEachChild<ViewAnimation>(x => isPaused = isPaused && x.IsAnimationPaused, false);
                return isPaused;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewAnimation()
        {
            Enabled = false;
            HideFlags = HideFlags.HideInHierarchy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public virtual void StartAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.StartAnimation(), false);
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public virtual void StopAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.StopAnimation(), false);
        }

        /// <summary>
        /// Resets the animation.
        /// </summary>
        public virtual void ResetAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ResetAnimation(), false);
        }

        /// <summary>
        /// Resets and stops the animation.
        /// </summary>
        public virtual void ResetAndStopAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ResetAndStopAnimation(), false);
        }

        /// <summary>
        /// Reverses the animation.
        /// </summary>
        public virtual void ReverseAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ReverseAnimation(), false);
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public virtual void PauseAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.PauseAnimation(), false);
        }

        /// <summary>
        /// Resumes the animation.
        /// </summary>
        public virtual void ResumeAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ResumeAnimation(), false);
        }

        /// <summary>
        /// Sets animation target.
        /// </summary>
        /// <param name="x"></param>
        public virtual void SetAnimationTarget(GameObject go)
        {
            this.ForEachChild<ViewAnimation>(x => x.SetAnimationTarget(go), false);
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<ViewAnimation />";
        }

        #endregion

    }
}
