using System;
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
    }
}