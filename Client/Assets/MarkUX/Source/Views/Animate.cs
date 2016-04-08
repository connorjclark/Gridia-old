#region Using Statements
using MarkUX.Animation;
using MarkUX.ValueConverters;
using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Animates views.
    /// </summary>
    [InternalView]
    public class Animate : ViewAnimation
    {
        #region Fields

        [ChangeHandler("UpdateBehavior")]
        public EasingFunctionType EasingFunction;

        [ChangeHandler("UpdateBehavior")]
        public bool AutoReset;

        [ChangeHandler("UpdateBehavior")]
        public bool AutoReverse;

        [ChangeHandler("UpdateBehavior")]
        public string Field;

        [ChangeHandler("UpdateBehavior")]
        public object From;

        [ChangeHandler("UpdateBehavior")]
        public object To;

        [ChangeHandler("UpdateBehavior")]
        public float ReverseSpeed;

        [ChangeHandler("UpdateBehavior")]
        [DurationValueConverter]
        public float Duration; // duration in seconds

        [ChangeHandler("UpdateBehavior")]
        [DurationValueConverter]
        public float StartOffset;

        [NotSetFromXml]
        public string FromStringValue;

        [NotSetFromXml]
        public string ToStringValue;

        private ViewFieldAnimator _viewFieldAnimator;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a boolean indicating whether this animation is active.
        /// </summary>
        public override bool IsAnimationActive
        {
            get
            {
                return _viewFieldAnimator.Active;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is reversing.
        /// </summary>
        public override bool IsAnimationReversing
        {
            get
            {
                return _viewFieldAnimator.Reversing;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is completed.
        /// </summary>
        public override bool IsAnimationCompleted
        {
            get
            {
                return _viewFieldAnimator.Completed;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is paused.
        /// </summary>
        public override bool IsAnimationPaused
        {
            get
            {
                return _viewFieldAnimator.Paused;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Animate()
        {
            Enabled = false;
            HideFlags = HideFlags.HideInHierarchy;
            EasingFunction = EasingFunctionType.Linear;
            AutoReset = false;
            AutoReverse = false;
            Duration = 0f;
            ReverseSpeed = 1.0f;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the animation each frame.
        /// </summary>
        public void Update()
        {
            if (Application.isPlaying && _viewFieldAnimator != null)
            {
                _viewFieldAnimator.Update();
            }
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public override void StartAnimation()
        {
            _viewFieldAnimator.StartAnimation();
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public override void StopAnimation()
        {
            _viewFieldAnimator.StopAnimation();
        }

        /// <summary>
        /// Resets and stops animation.
        /// </summary>
        public override void ResetAndStopAnimation()
        {
            _viewFieldAnimator.ResetAndStopAnimation();
        }

        /// <summary>
        /// Reverses the animation. Resumes the animation if paused.
        /// </summary>
        public override void ReverseAnimation()
        {
            _viewFieldAnimator.ReverseAnimation();
        }

        /// <summary>
        /// Pauses animation.
        /// </summary>
        public override void PauseAnimation()
        {
            _viewFieldAnimator.PauseAnimation();
        }

        /// <summary>
        /// Resumes paused animation.
        /// </summary>
        public override void ResumeAnimation()
        {
            _viewFieldAnimator.ResumeAnimation();
        }

        /// <summary>
        /// Resets the animation to its initial state (doesn't stop it).
        /// </summary>
        public override void ResetAnimation()
        {
            _viewFieldAnimator.ResetAnimation();
        }

        /// <summary>
        /// Called once by view before animations are used.
        /// </summary>
        public override void UpdateBehavior()
        {             
            base.UpdateBehavior();

            UpdateViewFieldAnimator();
        }

        /// <summary>
        /// Sets animation target.
        /// </summary>
        /// <param name="x"></param>
        public override void SetAnimationTarget(GameObject go)
        {
            Target = go;
            UpdateViewFieldAnimator();
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            _viewFieldAnimator = new ViewFieldAnimator();
            UpdateViewFieldAnimator();
        }

        /// <summary>
        /// Updates view field animator.
        /// </summary>
        private void UpdateViewFieldAnimator()
        {
            //Debug.Log(String.Format("Updating View Field Animator: {0}: {1}, {2}", Field, From, To));
            if (From != null && From is String)
            {
                FromStringValue = (String)From;
            }

            if (To != null && To is String)
            {
                ToStringValue = (String)To;
            }

            if (_viewFieldAnimator == null)
            {
                _viewFieldAnimator = new ViewFieldAnimator();
            }

            _viewFieldAnimator.EasingFunction = EasingFunction;
            _viewFieldAnimator.AutoReset = AutoReset;
            _viewFieldAnimator.AutoReverse = AutoReverse;
            _viewFieldAnimator.Field = Field;
            _viewFieldAnimator.From = From;
            _viewFieldAnimator.To = To;
            _viewFieldAnimator.FromStringValue = FromStringValue;
            _viewFieldAnimator.ToStringValue = ToStringValue;
            _viewFieldAnimator.ReverseSpeed = ReverseSpeed;
            _viewFieldAnimator.Duration = Duration;
            _viewFieldAnimator.StartOffset = StartOffset;

            var view = Target != null ? Target.GetComponent<View>() :
                (Parent != null ? Parent.GetComponent<View>() : null);
            _viewFieldAnimator.SetAnimationTarget(view);
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Animate AutoReset=""False"" AutoReverse=""False"" Duration=""0s"" ReverseSpeed=""1"" />";
        }

        #endregion
    }
}
