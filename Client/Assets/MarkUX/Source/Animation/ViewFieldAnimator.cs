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
    /// Animates a field.
    /// </summary>
    public class ViewFieldAnimator
    {
        #region Fields

        public EasingFunctionType EasingFunction;
        public bool AutoReset;
        public bool AutoReverse;
        public string Field;
        public object From;
        public object To;
        public float ReverseSpeed;
        public float Duration; // duration in seconds
        public float StartOffset;

        public string FromStringValue;
        public string ToStringValue;
        private View _view;
        private ValueInterpolator _valueInterpolator;
        private object _targetObject;
        private FieldInfo _viewFieldInfo;
        private FieldInfo _targetFieldInfo;
        private EasingFunctions.EasingFunction _easingFunction;

        // animation state
        private bool _active;
        private bool _reversing;
        private bool _completed;
        private bool _paused;
        private float _elapsedTime;
        private bool _isInitialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewFieldAnimator()
        {
            EasingFunction = EasingFunctionType.Linear;
            AutoReset = false;
            AutoReverse = false;
            Duration = 0f;
            ReverseSpeed = 1.0f;

            // default animation state
            _active = false;
            _reversing = false;
            _completed = true;
            _paused = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the animation each frame.
        /// </summary>
        public void Update()
        {
            if (_active)
            {
                UpdateAnimation(Time.unscaledDeltaTime);
            }
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public void StartAnimation()
        {           
            // do nothing if animation is already active
            if (_active)
            {
                return;
            }

            if (!_isInitialized)
            {
                Debug.LogWarning("[MarkUX.372] Animator started before setting target view. Make sure you call SetAnimationTarget() before starting animation.");
                return;
            }

            if (_targetFieldInfo == null)
            {
                Debug.LogWarning("[MarkUX.348] Animator started before setting animated field.");
                return;
            }

            // reset animation
            if (From == null)
            {
                // use current view field value as from value
                From = _targetFieldInfo.GetValue(_targetObject);
            }
            ResetAnimation();
            _active = true;
        }

        /// <summary>
        /// Updates the animator.
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds.</param>
        internal void UpdateAnimation(float deltaTime)
        {
            _elapsedTime = _reversing ? _elapsedTime - deltaTime * ReverseSpeed :
                _elapsedTime + deltaTime;

            // start animation once passed startOffset
            if (!_reversing && _elapsedTime < StartOffset)
                return;

            // clamp elapsed time to max duration
            float t = _reversing ? _elapsedTime.Clamp(0, Duration) : (_elapsedTime - StartOffset).Clamp(0, Duration);            
            float weight = Duration > 0 ? _easingFunction(t / Duration) : (_reversing ? 0f : 1f);

            object interpolatedValue = null;
            try
            {
                interpolatedValue = _valueInterpolator.Interpolate(From, To, weight);
            }
            catch (Exception e)
            {
                string viewName = _view != null ? _view.Name : "(null)";
                string fieldName = _targetFieldInfo != null ? _targetFieldInfo.Name : "Unknown";
                Debug.LogError(String.Format("[MarkUX.316] Animate: Unable to animate field {0}.{1}. Interpolator {2} threw exception: {3}. Stopping animation.", viewName, fieldName, _valueInterpolator.GetType().Name, e.Message));
                _active = false;
                return;
            }

            // set new value
            _view.SetValue(_viewFieldInfo, _targetFieldInfo, _targetObject, interpolatedValue, null, false, false, false);

            // is animation done?
            if ((_reversing && t <= 0) || (!_reversing && t >= Duration))
            {
                // yes. should animation auto-reverse?
                if (!_reversing && AutoReverse)
                {
                    // yes. reverse the animation
                    _reversing = true;
                    return;
                }

                // animation is complete
                if (AutoReset)
                {
                    ResetAndStopAnimation();
                }
                else
                {
                    PauseAnimation();
                }

                _completed = true;
            }
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void StopAnimation()
        {
            _active = false;
        }

        /// <summary>
        /// Resets and stops animation.
        /// </summary>
        public void ResetAndStopAnimation()
        {
            ResetAnimation();
            StopAnimation();
        }

        /// <summary>
        /// Reverses the animation. Resumes the animation if paused.
        /// </summary>
        public void ReverseAnimation()
        {
            // reverse animation if active
            if (_active)
            {
                _reversing = true;
            }
            else if (_paused)
            {
                _completed = false;
                _reversing = true;
                ResumeAnimation();
            }
        }

        /// <summary>
        /// Pauses animation.
        /// </summary>
        public void PauseAnimation()
        {
            _active = false;
            _paused = true;
        }

        /// <summary>
        /// Resumes paused animation.
        /// </summary>
        public void ResumeAnimation()
        {
            if (_paused)
            {                
                _active = true;
                _paused = false;
            }
        }

        /// <summary>
        /// Resets the animation to its initial state (doesn't stop it).
        /// </summary>
        public void ResetAnimation()
        {
            // resets the animation (but doesn't stop it)                        
            _elapsedTime = 0;
            _reversing = false;
            _paused = false;
            _completed = false;
            _view.SetValue(_viewFieldInfo, _targetFieldInfo, _targetObject, From, null, false, false, true);
        }

        /// <summary>
        /// Called once by view before animations are used.
        /// </summary>
        public void SetAnimationTarget(View target)
        {
            _isInitialized = false;
            _view = target;
            if (_view == null)
            {
                return;
            }
            
            object currentObject = _view;
            _targetFieldInfo = null;
            _targetObject = null;
            bool isViewField = true; // first field is a view field 

            if (From != null && From is String)
            {
                FromStringValue = (String)From;
            }

            if (To != null && To is String)
            {
                ToStringValue = (String)To;
            }

            if (String.IsNullOrEmpty(Field))
                return;

            var fields = Field.Split('.');
            for (int i = 0; i < fields.Length; ++i)
            {
                // parse index
                int index = -1;
                string fieldName = fields[i];

                int end = fields[i].IndexOf("]");
                if (end > 0)
                {
                    int start = fields[i].IndexOf('[') + 1;
                    if (!Int32.TryParse(Field.Substring(start, end - start), out index))
                    {
                        Debug.LogError(String.Format("[MarkUX.317] Unable to parse animation target field path \"{1}\".", Field));
                        return;
                    }

                    fieldName = fields[i].Substring(0, start - 1);
                }

                var fieldInfo = currentObject.GetType().GetField(fieldName);
                if (fieldInfo == null)
                {
                    // if it's the first field and not last - check if the view is in the hierarchy
                    if (i == 0 && fields.Length != 1)
                    {
                        var result = _view.FindView(fieldName, true);
                        if (result != null)
                        {
                            // view found
                            _view = result;
                            currentObject = result;
                            continue;
                        }
                    }

                    _targetFieldInfo = null;
                    Debug.LogError(String.Format("[MarkUX.318] Unable to parse animation target field path \"{1}\". Couldn't find field/view \"{2}\".", Field, fields[i]));
                    return;
                }

                // is this a class field?
                if (isViewField)
                {
                    // yes. 
                    _viewFieldInfo = fieldInfo;
                }

                // check if field is of type View
                if (fieldInfo.FieldType.IsSubclassOf(typeof(View)) || fieldInfo.FieldType == typeof(View))
                {
                    // next field will be a view field
                    isViewField = true;
                    _view = _viewFieldInfo.GetValue(currentObject) as View;
                }
                else
                {
                    isViewField = false;
                }

                // is this the last field?
                if (i == fields.Length - 1)
                {
                    // yes. it's the object field
                    if (index != -1)
                    {
                        Debug.LogError(String.Format("[MarkUX.319] Unable to parse animation target field path \"{1}\". Last member access can't be indexed.", Field));
                        return;
                    }

                    _targetFieldInfo = fieldInfo;
                    _targetObject = currentObject;
                }
                else
                {
                    // get next object
                    currentObject = fieldInfo.GetValue(currentObject);
                    if (index != -1)
                    {
                        // indexed object
                        var getItemMethod = currentObject.GetType().GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (getItemMethod == null)
                        {
                            return;
                        }
                        currentObject = getItemMethod.Invoke(currentObject, new object[] { index });
                    }
                }

                if (currentObject == null)
                {
                    Debug.LogError(String.Format("[MarkUX.320] Unable to parse animation target field path \"{1}\". Object \"{2}\" in path was null.", Field, fields[i]));
                    return;
                }
            }

            _valueInterpolator = ViewData.GetValueInterpolator(_targetFieldInfo.FieldType);
            _easingFunction = EasingFunctions.GetEasingFunction(EasingFunction);

            // set to and from values
            var converter = ViewData.GetValueConverter(_targetFieldInfo.FieldType);
            if (From == null && !String.IsNullOrEmpty(FromStringValue))
            {
                var result = converter.Convert(FromStringValue, ValueConverterContext.Empty);
                if (!result.Success)
                {
                    Debug.LogError(String.Format("[MarkUX.321] Unable to parse animation FromValue \"{1}\". {2}", From, result.ErrorMessage));
                    return;
                }

                From = result.ConvertedObject;
            }

            if (From == null && _targetFieldInfo != null)
            {
                // use current view field value as from value
                From = _targetFieldInfo.GetValue(_targetObject);
            }
            
            if (To == null && !String.IsNullOrEmpty(ToStringValue))
            {
                var result = converter.Convert(ToStringValue, ValueConverterContext.Empty);
                if (!result.Success)
                {
                    Debug.LogError(String.Format("[MarkUX.322] Unable to parse animation ToValue \"{1}\". {2}", To, result.ErrorMessage));
                    return;
                }

                To = result.ConvertedObject;
            }

            _isInitialized = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this animation is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this animation is reversing.
        /// </summary>
        public bool Reversing
        {
            get
            {
                return _reversing;
            }
        }

        /// <summary>
        /// Gets boolean indicating if animation is completed.
        /// </summary>
        public bool Completed
        {
            get
            {
                return _completed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this animation is paused.
        /// </summary>
        public bool Paused
        {
            get
            {
                return _paused;
            }
        }

        /// <summary>
        /// View which field is being animated.
        /// </summary>
        public View View
        {
            get
            {
                return _view;
            }
        }

        #endregion
    }
}
