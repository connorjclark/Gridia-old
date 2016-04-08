#region Using Statements
using System;
using UnityEngine;
#endregion

namespace MarkUX
{
    /// <summary>
    /// Contains information about a formatting token found in text.
    /// </summary>
    public class TextToken
    {
        #region Fields
        
        public TextTokenType TextTokenType;
        public View EmbeddedView;
        public int FontSize;
        public Color FontColor;

        #endregion        
    }

    /// <summary>
    /// Text token type.
    /// </summary>
    public enum TextTokenType
    {
        Unknown = 0,
        EmbeddedView = 1,
        BoldStart = 2,
        BoldEnd = 3,
        ItalicStart = 4,
        ItalicEnd = 5,
        SizeStart = 6,
        SizeEnd = 7,
        ColorStart = 8,
        ColorEnd = 9,
    }
}
