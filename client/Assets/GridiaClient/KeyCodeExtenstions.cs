namespace Gridia
{
    using System;

    using UnityEngine;

    public static class KeyCodeExtenstions
    {
        #region Methods

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

        #endregion Methods
    }
}