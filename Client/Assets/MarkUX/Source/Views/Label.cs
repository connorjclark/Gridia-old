#region Using Statements
using MarkUX.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Label view.
    /// </summary>
    [InternalView]
    [RemoveComponent(typeof(UnityEngine.UI.Image))]
    [AddComponent(typeof(Text))]
    public class Label : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateText")]
        public string Text;

        [ChangeHandler("UpdateLayout")]
        public AdjustToText AdjustToText;

        [ChangeHandler("UpdateBehavior")]
        public Font Font;

        [ChangeHandler("UpdateBehavior")]
        public FontStyle FontStyle;

        [ChangeHandler("UpdateBehavior")]
        public int FontSize;

        [ChangeHandler("UpdateBehavior")]
        public float LineSpacing;

        [ChangeHandler("UpdateBehavior")]
        public bool RichText;

        [ChangeHandler("UpdateBehavior")]
        public Alignment TextAlignment;

        [ChangeHandler("UpdateBehavior")]
        public Color FontColor;

        [ChangeHandler("UpdateBehavior")]
        public Color ShadowColor;
        public bool ShadowColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Vector2 ShadowDistance;
        public bool ShadowDistanceSet;

        [ChangeHandler("UpdateBehavior")]
        public Color OutlineColor;
        public bool OutlineColorSet;

        [ChangeHandler("UpdateBehavior")]
        public Vector2 OutlineDistance;
        public bool OutlineDistanceSet;

        [ChangeHandler("UpdateLayout")]
        public bool AdjustToEmbeddedContent;

        private View _parent;
        private static Regex _tagRegex = new Regex(@"\[(?<tag>[^\]]+)\]");
        private bool _initialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Label()
        {
            Text = String.Empty;
            AdjustToText = MarkUX.AdjustToText.None;
            FontStyle = UnityEngine.FontStyle.Normal;
            FontSize = 18;
            LineSpacing = 1;
            RichText = true;
            TextAlignment = MarkUX.Alignment.Left;
            Height = new ElementSize(1, ElementSizeUnit.Elements);
            FontColor = Color.black;
            AdjustToEmbeddedContent = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the layout of the view.
        /// </summary>
        public override void UpdateLayout()
        {
            var textComponent = GetComponent<Text>();
            if (textComponent == null)
                return;
                        
            // update text if it has changed
            if (HasChanged(() => Text) || !_initialized)
            {
                textComponent.text = ParseText();
                _initialized = true;
            }

            if (AdjustToText == AdjustToText.Width)
            {
                Width = new ElementSize(textComponent.preferredWidth, ElementSizeUnit.Pixels);
            }
            else if (AdjustToText == AdjustToText.Height)
            {
                Height = new ElementSize(textComponent.preferredHeight, ElementSizeUnit.Pixels);
            }
            else if (AdjustToText == AdjustToText.WidthAndHeight)
            {
                Width = new ElementSize(textComponent.preferredWidth, ElementSizeUnit.Pixels);
                Height = new ElementSize(textComponent.preferredHeight, ElementSizeUnit.Pixels);
            }

            base.UpdateLayout();
        }

        /// <summary>
        /// Replaces BBCode style tags with unity rich text syntax and parses embedded views.
        /// </summary>
        private string ParseText()
        {
            if (Text == null)
            {
                return String.Empty;
            }

            var textComponent = GetComponent<Text>();
            string formattedText = string.Empty;
            string separatorString = "&sp;";

            // clear all embedded views except templates
            if (Application.isPlaying)
            {
                this.ForEachChild<View>(x =>
                {
                    if (x.IsDynamic && !x.IsDestroyed)
                    {
                        x.Deactivate();
                        x.IsDestroyed = true;
                        GameObject.Destroy(x.gameObject);
                    }
                }, false);
            }

            // search for tokens and apply formatting and embedded views
            List<TextToken> tokens = new List<TextToken>();
            formattedText = _tagRegex.Replace(Text, x =>
            {
                string tag = x.Groups["tag"].Value.Trim();
                string tagNoWs = tag.RemoveWhitespace();

                // check if tag matches default tokens
                if (String.Equals("B", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // bold
                    tokens.Add(new TextToken { TextTokenType = TextTokenType.BoldStart });
                    return separatorString;
                }
                else if (String.Equals("/B", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // bold end
                    tokens.Add(new TextToken { TextTokenType = TextTokenType.BoldEnd });
                    return separatorString;
                }
                else if (String.Equals("I", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // italic
                    tokens.Add(new TextToken { TextTokenType = TextTokenType.ItalicStart });
                    return separatorString;
                }
                else if (String.Equals("/I", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // italic end
                    tokens.Add(new TextToken { TextTokenType = TextTokenType.ItalicEnd });
                    return separatorString;
                }
                else if (tagNoWs.StartsWith("SIZE=", StringComparison.OrdinalIgnoreCase))
                {                   
                    // parse size value
                    var vc = new IntValueConverter();
                    var convertResult = vc.Convert(tagNoWs.Substring(5), ValueConverterContext.Empty);
                    if (!convertResult.Success)
                    {
                        // unable to parse token
                        Debug.LogError(String.Format("[MarkUX.360] {0}: Unable to parse text embedded size tag \"[{1}]\". {2}", Name, tag, convertResult.ErrorMessage));
                        return String.Format("[{0}]", tag);
                    }

                    tokens.Add(new TextToken { TextTokenType = TextTokenType.SizeStart, FontSize = (int)convertResult.ConvertedObject });
                    return separatorString;
                }
                else if (String.Equals("/SIZE", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // size end
                    tokens.Add(new TextToken { TextTokenType = TextTokenType.SizeEnd });
                    return separatorString;
                }
                else if (tagNoWs.StartsWith("COLOR=", StringComparison.OrdinalIgnoreCase))
                {
                    // parse color value
                    var vc = new ColorValueConverter();
                    var convertResult = vc.Convert(tagNoWs.Substring(6), ValueConverterContext.Empty);
                    if (!convertResult.Success)
                    {
                        // unable to parse token
                        Debug.LogError(String.Format("[MarkUX.360] {0}: Unable to parse text embedded color tag \"[{1}]\". {2}", Name, tag, convertResult.ErrorMessage));
                        return String.Format("[{0}]", tag);
                    }

                    Color color = (Color)convertResult.ConvertedObject;
                    tokens.Add(new TextToken { TextTokenType = TextTokenType.ColorStart, FontColor = color });
                    return separatorString; 
                }
                else if (String.Equals("/COLOR", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // color end
                    tokens.Add(new TextToken { TextTokenType = TextTokenType.ColorEnd });
                    return separatorString;
                }                                

                // check if we have a template with the same ID
                var template = this.FindView<View>(tag, false);
                if (template != null && _parent != null)
                {
                    if (Application.isPlaying)
                    {
                        // instantiate embedded view
                        var embeddedView = CreateViewFromTemplate(template.gameObject, gameObject, _parent);
                        embeddedView.Activate();

                        embeddedView.Id = String.Empty;
                        embeddedView.Name = String.Format("Embedded View ({0})", template.Id);
                        embeddedView.gameObject.name = embeddedView.Name;

                        // make sure embedded view has dimensions set in pixels or elements
                        if (embeddedView.Width.Unit == ElementSizeUnit.Percents || embeddedView.Height.Unit == ElementSizeUnit.Percents)
                        {
                            Debug.LogError(String.Format("[MarkUX.359] {0}: Text embedded view \'{1}\' does not have its size specified in pixels or elements. Using default line height for dimensions.", Name, template.Id));
                            embeddedView.Width = ElementSize.GetPixels(21);
                            embeddedView.Height = ElementSize.GetPixels(21);
                        }

                        // set alignment
                        if (!embeddedView.AlignmentSet)
                        {
                            embeddedView.Alignment = Alignment.Bottom; // default alignment
                        }
                        else if (embeddedView.Alignment == Alignment.TopLeft || embeddedView.Alignment == Alignment.TopRight)
                        {
                            embeddedView.Alignment = Alignment.Top;
                        }
                        else if (embeddedView.Alignment == Alignment.Left || embeddedView.Alignment == Alignment.Right)
                        {
                            embeddedView.Alignment = Alignment.Center;
                        }
                        else if (embeddedView.Alignment == Alignment.BottomLeft || embeddedView.Alignment == Alignment.BottomRight)
                        {
                            embeddedView.Alignment = Alignment.Bottom;
                        }

                        embeddedView.InitializeViews();
                        embeddedView.UpdateViews();

                        tokens.Add(new TextToken { TextTokenType = TextTokenType.EmbeddedView, EmbeddedView = embeddedView });
                        return separatorString;
                    }
                }
                
                return String.Format("[{0}]", tag);
            });

            formattedText = formattedText.Replace("\\n", Environment.NewLine);

            // split the string up on each line
            StringBuilder result = new StringBuilder();
            var splitString = formattedText.Split(new string[] { separatorString }, StringSplitOptions.None);
            int splitIndex = 0;
            int xCaretPosition = 0;
            int yCaretPosition = 0;
            bool hasEmbeddedViews = tokens.Any(x => x.TextTokenType == TextTokenType.EmbeddedView);
            int fontBoldCount = 0;
            int fontItalicCount = 0;            
            Stack<int> fontSizeStack = new Stack<int>();
            int maxWordHeight = 0;
            List<View> lineEmbeddedViews = new List<View>();

            if (hasEmbeddedViews)
            {
                // restrict text-alignment on embedded views to top-left
                TextAlignment = Alignment.TopLeft;
                textComponent.alignment = TextAnchor;
            }

            // loop through each split string and apply tokens (embedded views & font styles)
            foreach (var str in splitString)
            {
                int tokenIndex = splitIndex - 1;
                var token = tokenIndex >= 0 && tokenIndex < tokens.Count ? tokens[tokenIndex] : null;
                ++splitIndex;

                // do we have a token?
                if (token != null)
                {
                    // yes. parse token type
                    switch (token.TextTokenType)
                    {
                        case TextTokenType.EmbeddedView:
                            // make space for embedded view and set its position                            
                            int xEmbeddedPosition = xCaretPosition;
                            int yEmbeddedPosition = yCaretPosition;
                            float embeddedSpacesWidth = token.EmbeddedView.Width.Pixels;
                            int lineHeight = Math.Max(maxWordHeight, GetDefaultLineHeight(textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize));
                            bool adjustWithPipe = AdjustToEmbeddedContent && token.EmbeddedView.Height.Pixels > lineHeight;
                            int adjustWithPipeFontSize = fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize;
                            float pipeWordLength = 0;
                            float pipeWordHeight = 0;
                            
                            if (adjustWithPipe)
                            {
                                // adjust line-height by adding a pipe character with a large enough font size
                                int startFontSize = adjustWithPipeFontSize;
                                for (int fontSize = startFontSize; fontSize < 500; ++fontSize)
                                {
                                    Vector2 wordSize = GetWordSize("|", textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSize);
                                    if (wordSize.y >= token.EmbeddedView.Height.Pixels)
                                    {
                                        // use this font size
                                        pipeWordLength = wordSize.x;
                                        pipeWordHeight = wordSize.y;
                                        embeddedSpacesWidth -= pipeWordLength;
                                        adjustWithPipeFontSize = fontSize;
                                        break;
                                    }
                                }
                            }

                            int spaceLength = CalculateWordLengthByCharInfo(" ", textComponent, fontBoldCount, fontItalicCount, fontSizeStack);
                            int spacesToAdd = Mathf.CeilToInt(embeddedSpacesWidth / spaceLength);
                            int embeddedWordLength = spacesToAdd > 0 ? spacesToAdd * spaceLength + (int)pipeWordLength : 0;

                            // if embedded word overflows append newline 
                            if (AdjustToText != AdjustToText.Width)
                            {
                                if (xCaretPosition + embeddedWordLength >= ActualWidth)
                                {
                                    // go through each embedded view on the line and adjust positioning depending on current line height                                    
                                    AdjustEmbeddedViews(lineEmbeddedViews, lineHeight);
                                    lineEmbeddedViews.Clear();

                                    // insert newline
                                    result.AppendLine();                                    
                                    yCaretPosition += Math.Max(maxWordHeight, GetDefaultLineHeight(textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize));
                                    xCaretPosition = 0;
                                    maxWordHeight = 0;
                                    xEmbeddedPosition = 0;
                                    yEmbeddedPosition = yCaretPosition;
                                }
                            }                           

                            // append spaces for embedded view
                            if (spacesToAdd > 0)
                            {
                                result.Append(new string(' ', spacesToAdd));
                            }
                            if (adjustWithPipe)
                            {
                                result.Append(String.Format("<size={0}><color=#00000000>|</color></size>", adjustWithPipeFontSize));
                                maxWordHeight = (int)pipeWordHeight;
                            }

                            xCaretPosition += embeddedWordLength;

                            // set embedded view position
                            int xOffset = 0;
                            if (embeddedWordLength > token.EmbeddedView.Width.Pixels)
                            {
                                // adjust x offset based on embedded word length
                                xOffset = (int)((embeddedWordLength - token.EmbeddedView.Width.Pixels) / 2f);
                            }

                            token.EmbeddedView.OffsetFromParent = new Margin(xEmbeddedPosition + xOffset, yEmbeddedPosition, 0, 0);
                            token.EmbeddedView.UpdateLayout();
                            lineEmbeddedViews.Add(token.EmbeddedView);
                            break;

                        case TextTokenType.BoldStart:
                            result.Append("<b>");
                            ++fontBoldCount;
                            break;

                        case TextTokenType.BoldEnd:
                            result.Append("</b>");
                            --fontBoldCount;
                            break;

                        case TextTokenType.ItalicStart:
                            result.Append("<i>");
                            ++fontItalicCount;
                            break;

                        case TextTokenType.ItalicEnd:
                            result.Append("</i>");
                            --fontItalicCount;
                            break;

                        case TextTokenType.SizeStart:
                            result.Append(String.Format("<size={0}>", token.FontSize));
                            fontSizeStack.Push(token.FontSize);
                            break;

                        case TextTokenType.SizeEnd:
                            result.Append("</size>");
                            fontSizeStack.Pop();
                            break;

                        case TextTokenType.ColorStart:
                            int r = (int)(token.FontColor.r * 255f);
                            int g = (int)(token.FontColor.g * 255f);
                            int b = (int)(token.FontColor.b * 255f);
                            int a = (int)(token.FontColor.a * 255f);
                            result.Append(String.Format("<color=#{0}{1}{2}{3}>", r.ToString("X2"), g.ToString("X2"), b.ToString("X2"), a.ToString("X2")));
                            break;

                        case TextTokenType.ColorEnd:
                            result.Append("</color>");
                            break;

                        case TextTokenType.Unknown:
                        default:
                            break;
                    }
                }

                // do we have embedded views?
                if (hasEmbeddedViews)
                {
#if !UNITY_4_6
                    textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
                    textComponent.verticalOverflow = VerticalWrapMode.Overflow;
#endif

                    // yes. then we want to make sure we calculate current caret position and handle
                    // line-breaks manually
                    string[] lines = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
                    {
                        string[] words = lines[lineIndex].Split(new char[] { ' ' }, StringSplitOptions.None);
                        for (int wordIndex = 0; wordIndex < words.Length; ++wordIndex)
                        {
                            string word = words[wordIndex];
                            if (wordIndex != words.Length - 1)
                            {
                                word += ' ';
                            }

                            // calculate length of word
                            int wordLength = CalculateWordLengthBySize(word, textComponent, fontBoldCount, fontItalicCount, fontSizeStack);
                            if (word.EndsWith(" "))
                            {
                                wordLength += CalculateWordLengthByCharInfo(" ", textComponent, fontBoldCount, fontItalicCount, fontSizeStack);
                            }

                            // insert newlines if we don't adjust to width
                            if (AdjustToText != AdjustToText.Width)
                            {
                                // if we overflow and it's not the first word then add newline
                                if (xCaretPosition + wordLength >= ActualWidth && wordIndex != 0)
                                {
                                    // go through each embedded view on the line and adjust positioning depending on current line height
                                    int lineHeight = Math.Max(maxWordHeight, GetDefaultLineHeight(textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize));
                                    AdjustEmbeddedViews(lineEmbeddedViews, lineHeight);
                                    lineEmbeddedViews.Clear();

                                    // insert newline
                                    result.AppendLine();
                                    xCaretPosition = 0;
                                    yCaretPosition += lineHeight;
                                    maxWordHeight = 0;                                    
                                }
                            }

                            // calculate height of word
                            maxWordHeight = Math.Max(GetWordHeight(word, textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize), maxWordHeight);

                            // add wordlength to caret position
                            xCaretPosition += wordLength;
                            result.Append(word);
                        }

                        // add newline unless it's the last line
                        if (lineIndex != lines.Length - 1)
                        {
                            // go through each embedded view on the line and adjust positioning depending on current line height
                            int lineHeight = Math.Max(maxWordHeight, GetDefaultLineHeight(textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize));
                            AdjustEmbeddedViews(lineEmbeddedViews, lineHeight);
                            lineEmbeddedViews.Clear();

                            result.AppendLine();
                            xCaretPosition = 0;
                            yCaretPosition += lineHeight;
                            maxWordHeight = 0;
                        }
                    }
                }
                else
                {
                    // use the default mechanism for line-breaks
#if !UNITY_4_6
                    textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
                    textComponent.verticalOverflow = VerticalWrapMode.Truncate;
#endif
                    result.Append(str);
                }
            }

            // adjust views that happens to be on the last line
            if (lineEmbeddedViews.Count > 0)
            {
                // go through each embedded view on the line and adjust positioning depending on current line height
                int lineHeight = Math.Max(maxWordHeight, GetDefaultLineHeight(textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize));
                AdjustEmbeddedViews(lineEmbeddedViews, lineHeight);
                lineEmbeddedViews.Clear();
            }


            return result.ToString();
        }

        /// <summary>
        /// Called when text has been changed.
        /// </summary>
        public virtual void UpdateText()
        {
            if (AdjustToText != AdjustToText.None)
            {
                // size of view changes with text so notify parents
                UpdateLayouts();
            }
            else
            {
                // size of view doesn't change with text, no need to notify parents
                UpdateLayout(); 
            }
        }

        /// <summary>
        /// Adjusts the positioning of embedded views on a line based on alignment.
        /// </summary>
        private void AdjustEmbeddedViews(List<View> lineEmbeddedViews, float lineHeight)
        {
            foreach (var view in lineEmbeddedViews)
            {
                if (view.Alignment == Alignment.Top)
                {
                    // no adjustment needed
                }
                else if (view.Alignment == Alignment.Center)
                {
                    // adjust positioning to center of line
                    float offsetCenter = view.Height.Pixels / 2f - lineHeight / 2f;
                    view.OffsetFromParent = new Margin(view.OffsetFromParent.Left.Pixels, view.OffsetFromParent.Top.Pixels - offsetCenter, 0, 0);
                }
                else
                {
                    // adjust positioning to bottom of line
                    float offsetBottom = view.Height.Pixels - lineHeight;
                    view.OffsetFromParent = new Margin(view.OffsetFromParent.Left.Pixels, view.OffsetFromParent.Top.Pixels - offsetBottom, 0, 0);
                }

                view.Alignment = Alignment.TopLeft;
                view.UpdateLayout();
            }

        }

        /// <summary>
        /// Calculates the default height of a line.
        /// </summary>
        private int GetDefaultLineHeight(Text textComponent, bool fontBold, bool fontItalic, int fontSize)
        {
            return GetWordHeight("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", textComponent, fontBold, fontItalic, fontSize);
        }

        /// <summary>
        /// Calculates the height of a word.
        /// </summary>
        private int GetWordHeight(string word, Text textComponent, bool fontBold, bool fontItalic, int fontSize)
        {
            return (int)GetWordSize(word, textComponent, fontBold, fontItalic, fontSize).y;
        }

        /// <summary>
        /// Gets width and height of a word.
        /// </summary>
        public Vector2 GetWordSize(string word, Text textComponent, bool fontBold, bool fontItalic, int fontSize)
        {
            FontStyle fontStyle = textComponent.fontStyle;
            if (fontBold && fontItalic)
            {
                fontStyle = UnityEngine.FontStyle.BoldAndItalic;
            }
            else if (fontBold)
            {
                fontStyle = UnityEngine.FontStyle.Bold;
            }
            else if (fontItalic)
            {
                fontStyle = UnityEngine.FontStyle.Italic;
            }

            // use GUI style to calculate the line height
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.font = textComponent.font;
            guiStyle.fontSize = fontSize;
            guiStyle.fontStyle = fontStyle;

            return guiStyle.CalcSize(new GUIContent(word));
        }

        /// <summary>
        /// Calculates the length of a word given a font style.
        /// </summary>
        private int CalculateWordLengthByCharInfo(string word, Text textComponent, int fontBoldCount, int fontItalicCount, Stack<int> fontSizeStack)
        {
            int fontSize = fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize;
            FontStyle fontStyle = textComponent.fontStyle;
            if (fontBoldCount > 0 && fontItalicCount > 0)
            {
                fontStyle = UnityEngine.FontStyle.BoldAndItalic;
            }
            else if (fontBoldCount > 0)
            {
                fontStyle = UnityEngine.FontStyle.Bold;
            }
            else if (fontItalicCount > 0)
            {
                fontStyle = UnityEngine.FontStyle.Italic;
            }

            return CalculateWordLengthByCharInfo(word, textComponent.font, fontSize, fontStyle);
        }

        /// <summary>
        /// Calculates the length of a word given a font style.
        /// </summary>
        private int CalculateWordLengthBySize(string word, Text textComponent, int fontBoldCount, int fontItalicCount, Stack<int> fontSizeStack)
        {
            int fontSize = fontSizeStack.Count > 0 ? fontSizeStack.Peek() : textComponent.fontSize;
            return (int)GetWordSize(word, textComponent, fontBoldCount > 0, fontItalicCount > 0, fontSize).x;
        }

        /// <summary>
        /// Calculates the length of a word using character-info.
        /// </summary>
        private int CalculateWordLengthByCharInfo(string word, Font font, int fontSize, FontStyle fontStyle)
        {
            int length = 0;
            foreach (var ch in word)
            {
                CharacterInfo chInfo;
                font.GetCharacterInfo(ch, out chInfo, fontSize, fontStyle);
#if UNITY_4_6
                length += (int)chInfo.width;
#else
                length += chInfo.advance;
#endif
            }

            return length;
        }

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();
            var textComponent = GetComponent<Text>();
            if (textComponent == null)
                return;

            if (Font != null)
            {
                textComponent.font = Font;
            }
            else
            {
                textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            }

            textComponent.fontSize = FontSize;
            textComponent.lineSpacing = LineSpacing;
            textComponent.supportRichText = RichText;
            textComponent.alignment = TextAnchor;
            textComponent.color = FontColor;
            textComponent.fontStyle = FontStyle;

            if (ShadowColorSet || ShadowDistanceSet)
            {
                var shadowComponent = GetComponent<Shadow>();
                if (shadowComponent == null)
                {
                    shadowComponent = gameObject.AddComponent<Shadow>();
                }

                shadowComponent.effectColor = ShadowColor;
                shadowComponent.effectDistance = ShadowDistance;
            }

            if (OutlineColorSet || OutlineDistanceSet)
            {
                var outlineComponent = GetComponent<Outline>();
                if (outlineComponent == null)
                {
                    outlineComponent = gameObject.AddComponent<Outline>();
                }

                outlineComponent.effectColor = OutlineColor;
                outlineComponent.effectDistance = OutlineDistance;
            }
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.ForEachChild<View>(x => x.Deactivate(), false);
            _parent = Parent != null ? Parent.GetComponent<View>() : null;
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<Label FontStyle=""Normal"" FontSize=""18"" LineSpacing=""1"" RichText=""True"" FontColor=""Black"" TextAlignment=""Left"" Width=""3em"" Height=""1em"">
                    </Label>";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets text anchor.
        /// </summary>
        public TextAnchor TextAnchor
        {
            get
            {
                switch (TextAlignment)
                {
                    case Alignment.TopLeft:
                        return TextAnchor.UpperLeft;
                    case Alignment.Top:
                        return TextAnchor.UpperCenter;
                    case Alignment.TopRight:
                        return TextAnchor.UpperRight;
                    case Alignment.Left:
                        return TextAnchor.MiddleLeft;
                    case Alignment.Right:
                        return TextAnchor.MiddleRight;
                    case Alignment.BottomLeft:
                        return TextAnchor.LowerLeft;
                    case Alignment.Bottom:
                        return TextAnchor.LowerCenter;
                    case Alignment.BottomRight:
                        return TextAnchor.LowerRight;
                    case Alignment.Center:
                    default:
                        return TextAnchor.MiddleCenter;
                }
            }
        }

        #endregion
    }
}
