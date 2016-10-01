namespace Gridia
{
    using System;

    using UnityEngine;
    using UnityEngine.UI;

    // :( Todo: able to resize chat window
    public class ChatWindow2 : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        public InputField ChatInput;
        [SerializeField]
        public Text ChatText;
        [SerializeField]
        public Scrollbar Scrollbar;

        private readonly String _helpMessage;
        private readonly RichText _richText = new RichText();

        #endregion Fields

        #region Constructors

        public ChatWindow2()
        {
            Locator.Provide(this);
            // :(
            _helpMessage = @"<color=blue>*Gridia instructions*</color>

            If you find any *bugs*, please *report* them here: https://github.com/Hoten/Gridia/issues

            For a video demonstration and introduction to Gridia, check out this video on YouTube: https://www.youtube.com/watch?v=zpi_QMDMhW0

            Press *ENTER* to focus/unfocus on the chat. Press *ESCAPE* to hide the chat, and *ENTER* to bring it back.

            To pick up an item, either drag it to your player/inventory. Or, use the arrow keys to select, and press *SHIFT*

            To move an item, either drag it with your mouse to another location, your player, your inventory window, or a container/container window. Or, use the *arrow keys* to select the item to move, hold *ALT+SHIFT*, and move the item with the *arrow keys*

            To open a chest, first make sure that it is not closed. Use your hands on it by highlighting it with the *arrow keys* and pressing *ALT*. To view its contents, move into it, or if the container is solid, highlight it and press *ALT*.
            When you have multiple containers open, press *TAB* to change the container focus.

            To select an item in your inventory, either press 1,2,3...0 to select one of the first few items,
            Or, hold *CTRL* and use the *arrow keys* to move your selection

            To use your selected item in the world, use the *arrow keys* and press *SPACE*

            Right click on an item in a container to view its Recipe Book

            To use your hand in the world, use the arrows keys and press *ALT*

            Press *Q* to drop a single item of your current selection

            To equip an item, *double click* on it. Or, press *E* to equip your currently selected item

            Pretty chat: \**bold*\* \*\***italics**\*\* \*\*\****both***\*\*\*
            ________________________
            ";
        }

        #endregion Constructors

        #region Methods

        public void Append(String username, String text)
        {
            Append(String.Format("<color=navy>*{0} says*</color>: {1}", username, text));
        }

        public void Append(String text)
        {
            var isAtMaxScroll = Math.Abs(Scrollbar.value) < 0.0001;
            _richText.Append(text + "\n");
            ChatText.text = _richText.ToString();
            if (isAtMaxScroll)
            {
                SetScrollToMax();
            }
        }

        public void OnGUI()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                SendChatMessage();
            }
        }

        public void Start()
        {
            Append("Type !help for instructions.\nIf you find any *bugs*, please *report* them here: https://github.com/Hoten/Gridia/issues");

            SetScrollToMax();
        }

        private void SendChatMessage()
        {
            var message = ChatInput.text;
            if (message.Length == 0 || !Locator.Get<GridiaDriver>().IsGameReady()) return; // :(

            if (!RichText.HtmlIsValid(message))
            {
                GridiaConstants.ErrorMessage = "Invalid html.";
                return;
            }
            if (message.Length > 200)
            {
                GridiaConstants.ErrorMessage = "Message is " + message.Length + " characters long. Max is 200.";
                return;
            }

            if (message == "!help")
            {
                Append(_helpMessage);
            }
            else
            {
                Locator.Get<ConnectionToGridiaServerHandler>().Chat(message);
            }
            ChatInput.text = "";
            SetScrollToMax();
        }

        private void SetScrollToMax()
        {
            Scrollbar.value = 0;
        }

        #endregion Methods
    }
}