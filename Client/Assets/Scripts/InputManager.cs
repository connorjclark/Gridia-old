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

        public bool Valid9DirectionalInput() 
        {
            return Get9DirectionalInput() != Vector3.zero || Input.GetKeyDown(KeyCode.S);
        }

        public Vector3 Get9DirectionalInput() 
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                return Vector3.left + Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.W)) 
            {
                return Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                return Vector3.right + Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                return Vector3.left;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                return Vector3.zero;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                return Vector3.right;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                return Vector3.left + Vector3.down;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                return Vector3.down;
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                return Vector3.right + Vector3.down;
            }
            return Vector3.zero;
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
