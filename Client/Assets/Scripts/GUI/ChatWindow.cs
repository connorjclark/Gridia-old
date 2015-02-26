using System;
using UnityEngine;

namespace Gridia
{
    // :( Todo: able to resize chat window
    public class ChatWindow : GridiaWindow
    {
        public TextField ChatInput { get; private set; }
        private TextArea ChatArea { get; set; }
        private String _helpMessage;

        public ChatWindow(Vector2 pos)
            : base(pos, "Chat")
        {
            ChatArea = new TextArea(Vector2.zero, 150, 125) {MaxLength = 500};
            AddChild(ChatArea);

            ChatInput = new TextField(new Vector2(0, ChatArea.Height + 10), "ChatInput", 150, 30)
            {
                OnEnter = SendChatMessage
            };
            AddChild(ChatInput);

            Append("Type !help for instructions.");

            // :(
            _helpMessage = @"<color=blue>*Gridia instructions*</color>

For a video demonstration and introduction to Gridia, check out this video on YouTube: https://www.youtube.com/watch?v=zpi_QMDMhW0

Press *ENTER* to focus/unfocus on the chat. Press *ESCAPE* to hide the chat, and *ENTER* to bring it back.

To pick up an item, either drag it to your player/inventory. Or, use the arrow keys to select, and press *SHIFT*

To move an item, either drag it with your mouse to another location, your player, your inventory window, or a container/container window. Or, use the *arrow keys* to select the item to move, hold *ALT+SHIFT*, and move the item with the *arrow keys*

To open a chest, first make sure that it is not closed. Use your hands on it by highlighting it with the *arrow keys* and pressing *ALT*. To view its contents, move into it, or if the container is solid, highlight it and press *ALT*.

To select an item in your inventory, either press 1,2,3...0 to select one of the first few items,
Or, hold *CTRL* and use the *arrow keys* to move your selection

To use your selected item in the world, use the *arrow keys* and press *SPACE*

To use your hand in the world, use the arrows keys and press *ALT*

Press *Q* to drop a single item of your current selection

To equip an item, *double click* on it. Or, press *E* to equip your currently selected item

Pretty chat: \*bold\* \*\*italics\*\* \*\*\*both\*\*\*
________________________

";
            SetScrollToMax();
        }

        public void Append(String username, String text) 
        {
            Append(String.Format("<color=navy><b>{0} says</b></color>: {1}", username, text));
        }

        public void Append(String text)
        {
            var isAtMaxScroll = ChatArea.MaxScrollY == ChatArea.ScrollPosition.y;
            ChatArea.Append(text + "\n");
            if (isAtMaxScroll)
            {
                SetScrollToMax();
            }
        }

        private void SendChatMessage(String message)
        {
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
            ChatInput.Text = "";
        }

        private void SetScrollToMax() 
        {
            ChatArea.ScrollPosition = new Vector2(0, int.MaxValue);
        }
    }
}
