namespace Gridia
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class InputManager
    {
        #region Fields

        private const float DoubleHitThreshold = 0.3f;

        private readonly KeyCode[] _arrowKeyCodes = { KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow };
        private readonly Dictionary<KeyCode, float> _lastHitTimes = new Dictionary<KeyCode, float>();
        private readonly KeyCode[] _wasdKeyCodes = { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A };

        private KeyCode _doublePressKeyCode = KeyCode.None;

        #endregion Fields

        #region Methods

        public Vector3 Get4DirectionalArrowKeysInputUp()
        {
            return Get4DirectionalInput(Input.GetKeyUp, _arrowKeyCodes);
        }

        public Vector3 Get4DirectionalWasdInput()
        {
            return Get4DirectionalInput(Input.GetKey, _wasdKeyCodes);
        }

        public Vector3 Get4DirectionalWasdInputUp()
        {
            return Get4DirectionalInput(Input.GetKeyUp, _wasdKeyCodes);
        }

        public bool GetKeyDoublePress(KeyCode keyCode)
        {
            return keyCode == _doublePressKeyCode;
        }

        public void Step()
        {
            _doublePressKeyCode = KeyCode.None;
            if (Event.current.type == EventType.KeyUp)
            {
                HandleKeyCodeUp(Event.current.keyCode);
            }
        }

        private Vector3 Get4DirectionalInput(Predicate<KeyCode> selector, KeyCode[] keys)
        {
            var direction = Vector3.zero;
            if (selector(keys[0]))
                direction += Vector3.up;
            if (selector(keys[1]))
                direction += Vector3.right;
            if (selector(keys[2]))
                direction += Vector3.down;
            if (selector(keys[3]))
                direction += Vector3.left;
            return direction;
        }

        private void HandleKeyCodeUp(KeyCode keyCode)
        {
            if (_lastHitTimes.ContainsKey(keyCode) && Time.time - _lastHitTimes[keyCode] < DoubleHitThreshold)
            {
                _doublePressKeyCode = keyCode;
            }
            _lastHitTimes[keyCode] = Time.time;
        }

        #endregion Methods
    }
}