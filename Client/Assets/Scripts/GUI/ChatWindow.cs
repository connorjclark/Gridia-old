using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Gridia
{
    // :( Todo: able to resize chat window
    public class ChatWindow : GridiaWindow
    {
        public TextField ChatInput { get; private set; }
        private TextArea ChatArea { get; set; }
        private int _maxChatAreaLength = 5000;

        public ChatWindow(Vector2 pos)
            : base(pos, "Chat")
        {
            ChatArea = new TextArea(Vector2.zero, 150, 125);
            AddChild(ChatArea);

            ChatInput = new TextField(new Vector2(0, ChatArea.Height + 10), "ChatInput", 150, 30);
            ChatInput.OnEnter = SendChatMessage;
            AddChild(ChatInput);

            ChatArea.Text = @"Type below to chat!

Press ENTER to focus/unfocus on the chat.

Press ESCAPE to hide the chat.

To pick up an item, either drag it to your player/inventory,
Or, use the arrow keys to select, and press SHIFT

To move an item, either drag it with your mouse to another location, your player, or your inventory window,
Or, use the arrow keys to select the item to move, hold ALT+SHIFT, and move the item with the arrow keys

To select an item in your inventory, either press 1,2,3...0 to select one of the first few items,
Or, hold CTRL and use WASD to move your selection

To use your selected item in the world, use the arrow keys and press SPACE

To use your hand in the world, use the arrows keys and press ALT

Press Q to drop a single item of your current selection

To equip an item, double click on it
Or, press E to equip your currently selected item
________________________

";
            SetScrollToMax();
        }

        private String ParseBold(String text)
        {
            return Regex.Replace(text, @"\*(.*)\*", "<b>$1</b>");
        }

        private String ParseItalics(String text)
        {
            return Regex.Replace(text, @"\*\*(.*)\*\*", "<i>$1</i>");
        }

        private String ParseRichText(String text)
        {
            return ParseBold(ParseItalics(text));
        }

        public void append(String username, String text) 
        {
            var isAtMaxScrol = ChatArea.MaxScrollY == ChatArea.ScrollPosition.y;
            ChatArea.Text += String.Format("<color=navy><b>{0} says</b></color>: {1}\n", username, ParseRichText(text));
            var length = ChatArea.Text.Length;
            if (length > _maxChatAreaLength)
            {
                ChatArea.Text = ChatArea.Text.Substring(length - _maxChatAreaLength, _maxChatAreaLength);
            }
            if (isAtMaxScrol)
            {
                SetScrollToMax();
            }
        }

        private void SendChatMessage(String message)
        {
            if (message.Length <= 200)
            {
                Locator.Get<ConnectionToGridiaServerHandler>().Chat(message);
                ChatInput.Text = "";
            }
            else
            {
                GridiaConstants.ErrorMessage = "Message is " + message.Length + " characters long. Max is 200.";
            }
        }

        private void SetScrollToMax() 
        {
            ChatArea.ScrollPosition = new Vector2(0, int.MaxValue);
        }
    }
}
