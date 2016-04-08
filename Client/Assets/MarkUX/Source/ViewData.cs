#region Using Statements
using System.Collections;
using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;
using MarkUX.Animation;
using MarkUX.Views;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Contains global view data.
    /// </summary>
    public static class ViewData
    {
        #region Fields

        private static Dictionary<Type, Type> _typeValueInterpolators;
        private static Dictionary<Type, ValueConverter> _typeValueConverters;
        private static bool _isInitialized = false;
        private static float _elementSize;        

        public static float ElementSize
        {
            get
            {
                if (!_isInitialized)
                {
                    Debug.LogError("[MarkUX.312] ViewData used before data has been initialized.");
                }

                return _elementSize;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public static void Initialize(float elementSize)
        {
            _isInitialized = true;

            _elementSize = elementSize;
            _typeValueConverters = new Dictionary<Type, ValueConverter>();
            _typeValueInterpolators = new Dictionary<Type, Type>();

            // map types to value interpolators
            foreach (var type in TypeHelper.FindDerivedTypes(typeof(ValueInterpolator)))
            {
                var valueInterpolator = TypeHelper.CreateInstance(type) as ValueInterpolator;
                if (valueInterpolator.Type != null)
                {
                    _typeValueInterpolators.Add(valueInterpolator.Type, type);
                }
            }

            // map types to value converters
            foreach (var type in TypeHelper.FindDerivedTypes(typeof(ValueConverter)))
            {
                var valueConverter = TypeHelper.CreateInstance(type) as ValueConverter;
                if (valueConverter.Type != null)
                {
                    _typeValueConverters.Add(valueConverter.Type, valueConverter);
                }
            }     
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value interpolator for specified type.
        /// </summary>
        public static ValueInterpolator GetValueInterpolator(Type type)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[MarkUX.313] ViewData used before data has been initialized.");
                return null;
            }

            // check if a value interpolator exits for the specified type, otherwise return a default interpolator
            return _typeValueInterpolators.ContainsKey(type) ?
                TypeHelper.CreateInstance(_typeValueInterpolators[type]) as ValueInterpolator : new ValueInterpolator();
        }

        /// <summary>
        /// Gets value converter for specified type.
        /// </summary>
        public static ValueConverter GetValueConverter(Type type)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[MarkUX.314] ViewData used before data has been initialized.");
                return null;
            }

            // check if a value converter exits for the specified type, otherwise return a default converter
            return _typeValueConverters.ContainsKey(type) ? _typeValueConverters[type] : new ValueConverter();
        }

        #endregion
    }
}
