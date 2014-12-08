using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class InputManager
    {
        private Dictionary<KeyCode, float> lastHitTimes = new Dictionary<KeyCode, float>();
        private float _doubleHitThreshold = 0.3f;
        private KeyCode _doublePressKeyCode = KeyCode.None;

        public void Step()
        {
            _doublePressKeyCode = KeyCode.None;
            var keyCode = Event.current.keyCode;
            if (Event.current.type == EventType.KeyUp)
            {
                HandleKeyCodeUp(Event.current.keyCode);
            }
        }

        public Vector3 Get4DirectionalInput()
        {
            var direction = Vector3.zero;

            if (Input.GetButton("left"))
                direction += Vector3.left;
            if (Input.GetButton("right"))
                direction += Vector3.right;
            if (Input.GetButton("down"))
                direction += Vector3.down;
            if (Input.GetButton("up"))
                direction += Vector3.up;

            return direction;
        }



        // :(
        public Vector3 Get4DirectionalInputUp()
        {
            var direction = Vector3.zero;

            if (Input.GetButtonUp("left"))
                direction += Vector3.left;
            if (Input.GetButtonUp("right"))
                direction += Vector3.right;
            if (Input.GetButtonUp("down"))
                direction += Vector3.down;
            if (Input.GetButtonUp("up"))
                direction += Vector3.up;

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
            if (lastHitTimes.ContainsKey(keyCode) && Time.time - lastHitTimes[keyCode] < _doubleHitThreshold)
            {
                _doublePressKeyCode = keyCode;
            }
            lastHitTimes[keyCode] = Time.time;
        }
    }
}
