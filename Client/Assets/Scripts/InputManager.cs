using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class InputManager
    {
        private readonly Dictionary<KeyCode, float> _lastHitTimes = new Dictionary<KeyCode, float>();
        private const float DoubleHitThreshold = 0.3f;
        private KeyCode _doublePressKeyCode = KeyCode.None;

        public void Step()
        {
            _doublePressKeyCode = KeyCode.None;
            if (Event.current.type == EventType.KeyUp)
            {
                HandleKeyCodeUp(Event.current.keyCode);
            }
        }

        public Vector3 Get4DirectionalInput()
        {
            var x = Math.Sign(Input.GetAxis("Horizontal"));
            var y = Math.Sign(Input.GetAxis("Vertical"));
            return new Vector3(x, y, 0);
        }

        // :(
        public Vector3 Get4DirectionalInputUp()
        {
            var direction = Vector3.zero;

            if (Input.GetKeyUp(KeyCode.W))
                direction += Vector3.up;
            if (Input.GetKeyUp(KeyCode.D))
                direction += Vector3.right;
            if (Input.GetKeyUp(KeyCode.S))
                direction += Vector3.down;
            if (Input.GetKeyUp(KeyCode.A))
                direction += Vector3.left;

            return direction;
        }

        // :(
        public Vector3 Get4DirectionalArrowKeysInputUp()
        {
            var direction = Vector3.zero;

            if (Input.GetKeyUp(KeyCode.UpArrow))
                direction += Vector3.up;
            if (Input.GetKeyUp(KeyCode.RightArrow))
                direction += Vector3.right;
            if (Input.GetKeyUp(KeyCode.DownArrow))
                direction += Vector3.down;
            if (Input.GetKeyUp(KeyCode.LeftArrow))
                direction += Vector3.left;

            return direction;
        }

        public bool GetKeyDoublePress(KeyCode keyCode) 
        {
            return keyCode == _doublePressKeyCode;
        }

        private void HandleKeyCodeUp(KeyCode keyCode) 
        {
            if (_lastHitTimes.ContainsKey(keyCode) && Time.time - _lastHitTimes[keyCode] < DoubleHitThreshold)
            {
                _doublePressKeyCode = keyCode;
            }
            _lastHitTimes[keyCode] = Time.time;
        }
    }
}
