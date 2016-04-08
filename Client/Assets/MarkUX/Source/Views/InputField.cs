#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Input field view.
    /// </summary>
    [InternalView]
    [AddComponent(typeof(UnityEngine.UI.InputField))]
    public class InputField : ContentView
    {
        #region Fields

        [ChangeHandler("UpdateBehavior")]
        public Margin TextMargin;

        [ChangeHandler("UpdateBehavior")]
        public string Text;

        [ChangeHandler("UpdateBehavior")]
        public Font Font;

        [ChangeHandler("UpdateBehavior")]
        public FontStyle FontStyle;

        [ChangeHandler("UpdateBehavior")]
        public int FontSize;

        [ChangeHandler("UpdateBehavior")]
        public float LineSpacing;

        [ChangeHandler("UpdateBehavior")]
        public Color FontColor;
        
        public Label InputText;
        public View InputFieldPlaceholder;        

        [ChangeHandler("UpdateBehavior")]
        public int CharacterLimit;

        [ChangeHandler("UpdateBehavior")]
        public UnityEngine.UI.InputField.ContentType ContentType;

        [ChangeHandler("UpdateBehavior")]
        public UnityEngine.UI.InputField.LineType LineType;

        [ChangeHandler("UpdateBehavior")]
        public float CaretBlinkRate;

        [ChangeHandler("UpdateBehavior")]
        public Color TextSelectionColor;

        public bool SetValueOnEndEdit;

        public ViewAction EndEdit;
        public ViewAction ValueChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public InputField()
        {        
            TextMargin = new MarkUX.Margin();
            Text = String.Empty;
            FontStyle = UnityEngine.FontStyle.Normal;
            FontSize = 18;
            LineSpacing = 1;
            FontColor = Color.black;
            CharacterLimit = 0;
            ContentType = UnityEngine.UI.InputField.ContentType.Standard;
            LineType = UnityEngine.UI.InputField.LineType.SingleLine;
            CaretBlinkRate = 1.7f;
            TextSelectionColor = Color.blue;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the behavior of the view.
        /// </summary>
        public override void UpdateBehavior()
        {
            base.UpdateBehavior();
            var inputFieldComponent = GetComponent<UnityEngine.UI.InputField>();
            if (inputFieldComponent == null)
                return;

            inputFieldComponent.textComponent = InputText.GetComponent<UnityEngine.UI.Text>();
            inputFieldComponent.placeholder = InputFieldPlaceholder.GetComponent<UnityEngine.UI.Image>();
            inputFieldComponent.text = Text;
            inputFieldComponent.characterLimit = CharacterLimit;
            inputFieldComponent.contentType = ContentType;
            inputFieldComponent.lineType = LineType;
            inputFieldComponent.caretBlinkRate = CaretBlinkRate;
            inputFieldComponent.selectionColor = TextSelectionColor;
            inputFieldComponent.transition = Selectable.Transition.None;

            UpdatePlaceholder();
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // hook up input field event system triggers           
            var inputFieldComponent = GetComponent<UnityEngine.UI.InputField>();
            inputFieldComponent.onEndEdit.RemoveAllListeners();
            inputFieldComponent.onEndEdit.AddListener(InputFieldEndEdit);

            inputFieldComponent.onValueChange.RemoveAllListeners();
            inputFieldComponent.onValueChange.AddListener(InputFieldValueUpdated);
        }

        /// <summary>
        /// Called on input field end edit.
        /// </summary>
        public void InputFieldEndEdit(string value)
        {           
            if (SetValueOnEndEdit)
            {
                var inputFieldComponent = GetComponent<UnityEngine.UI.InputField>();
                SetValue(() => Text, inputFieldComponent.text);
            }

            UpdatePlaceholder();
            EndEdit.Trigger();
        }

        /// <summary>
        /// Called when input field value has been updated.
        /// </summary>
        public void InputFieldValueUpdated(string value)
        {
            if (!SetValueOnEndEdit)
            {
                var inputFieldComponent = GetComponent<UnityEngine.UI.InputField>();
                SetValue(() => Text, inputFieldComponent.text);
            }

            UpdatePlaceholder();
            ValueChanged.Trigger();
        }

        /// <summary>
        /// Shows or hides placeholder based on text.
        /// </summary>
        private void UpdatePlaceholder()
        {
            if (String.IsNullOrEmpty(Text))
            {
                InputFieldPlaceholder.Activate();                
            }
            else
            {
                InputFieldPlaceholder.Deactivate();
            }
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return
                @"<InputField BackgroundColor=""White"" TextMargin=""9,9,9,9"" FontStyle=""Normal"" FontSize=""18"" LineSpacing=""1"" FontColor=""Black"" Width=""8em"" Height=""1em"" CaretBlinkRate=""1.7"" Text="""" CharacterLimit=""0"" ContentType=""Standard"" LineType=""SingleLine"" TextSelectionColor=""#aaaaaa"">
                    <ContentContainer Id=""InputFieldPlaceholder"" />
                    <Label Id=""InputText"" Margin=""{TextMargin}"" Text=""{Text}"" RichText=""False"" TextAlignment=""TopLeft"" Width=""100%"" Height=""100%"" Font=""{Font}"" FontStyle=""{FontStyle}"" FontSize=""{FontSize}"" LineSpacing=""{LineSpacing}"" FontColor=""{FontColor}""/>
                </InputField>";
        }

        #endregion
    }
}
