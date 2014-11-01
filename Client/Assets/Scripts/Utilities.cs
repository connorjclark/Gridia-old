using System;
using UnityEngine;

namespace Gridia
{
    public class Direction
    {
        public static readonly Vector2 None = new Vector2(0, 0);
        public static readonly Vector2 Up = new Vector2(0, 1);
        public static readonly Vector2 Left = new Vector2(-1, 0);
        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Down = new Vector2(0, -1);
    }

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

        public static Vector2 GetRelativeDirection(Vector2 from, Vector2 to) {
            Vector2 heading = to - from;
            return heading / heading.magnitude;
        }

        public static Vector2 RandomDirection()
        {
            int value = UnityEngine.Random.Range(1, 4);
            switch (value)
            {
                case 1: return Direction.Up;
                case 2: return Direction.Down;
                case 3: return Direction.Left;
                case 4: return Direction.Right;
            }
            return Direction.None;
        }
    }
}