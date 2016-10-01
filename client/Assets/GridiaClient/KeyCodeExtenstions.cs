using System;
using UnityEngine;

namespace Gridia
{
    public static class KeyCodeExtenstions
    {
        public static String ToShortString(this KeyCode key)
        {
            if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
            {
                var num = (int) key - (int) KeyCode.Alpha0;
                return num.ToString();
            }
            switch (key)
            {
                case KeyCode.Plus:
                    return "+";
                case KeyCode.Minus:
                    return "-";
                default:
                    return key.ToString();
            }
        }
    }
}
