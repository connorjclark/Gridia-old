﻿using System;
using UnityEngine;

namespace Gridia
{
    public class Utilities
    {
        public static Vector2 Vector2FromAngle(double radians)
        {
            float x = (float)Math.Cos(radians);
            float y = (float)Math.Sin(radians);
            return new Vector2(x, y);
        }

        public static Vector2 Vector2Residual(Vector2 vector)
        {
            return new Vector2(vector.x % 1, vector.y % 1);
        }

        public static Vector2 Vector2Floor(Vector2 vector)
        {
            return new Vector2(Mathf.Floor(vector.x), Mathf.Floor(vector.y));
        }

        public static bool Vector2IsAbsoluteGreaterThanOne(Vector2 vector)
        {
            return Mathf.Abs(vector.x) >= 1 || Mathf.Abs(vector.y) >= 1;
        }

        public static int CompareAbsoluteValues(float a, float b) 
        {
            if (a == b) return 0;
            if (Mathf.Abs(a) > Mathf.Abs(b)) return 1;
            return -1;
        }

        public static Vector3 GetRelativeDirection(Vector3 from, Vector3 to) {
            Vector3 heading = to - from;
            return heading / heading.magnitude;
        }

        public static Vector3 RandomDirection()
        {
            int value = UnityEngine.Random.Range(1, 4);
            switch (value)
            {
                case 1: return Vector3.up;
                case 2: return Vector3.down;
                case 3: return Vector3.left;
                case 4: return Vector3.right;
            }
            return Vector3.zero;
        }
    }
}