using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    // :( Todo: able to resize chat window
    public class ChatWindow : GridiaWindow
    {
        private TextField ChatInput { get; set; }
        private TextArea ChatArea { get; set; }

        public ChatWindow(Vector2 pos)
            : base(pos, "Chat")
        {
            ChatArea = new TextArea(Vector2.zero, 150, 125);
            AddChild(ChatArea);

            ChatInput = new TextField(new Vector2(0, ChatArea.Height + 10), "ChatInput", 150, 30);
            ChatInput.OnEnter = SendChatMessage;
            AddChild(ChatInput);

            ChatInput.Text = "!name putYourNameHere";

            ChatArea.Text = @"Type below to chat!

Press ESCAPE once to unfocus the chat box, and again to hide the chat.

Press TAB once to show the chat, and again to set focus on it.

To move an item, either drag it with your mouse to another location, your player, or your inventory window,
Or, use the arrow keys and press shift

To select an item in your inventory, either press 1,2,3...0 to select on of the first few items,
Or, hold CTRL and press WASD to move your selection

To use your selected item in the world, use the arrow keys and press SPACE

To use your hand in the world, use the arrows keys and press ALT

Press Q to drop a single item of your current selection

";
            SetScrollToMax();
        }

        public void append(String line) 
        {
            var isAtMaxScrol = ChatArea.MaxScrollY == ChatArea.ScrollPosition.y;
            ChatArea.Text += line + '\n';
            if (isAtMaxScrol)
            {
                SetScrollToMax();
            }
        }

        private void SendChatMessage(String message) 
        {
            Locator.Get<ConnectionToGridiaServerHandler>().Chat(message);
        }

        private void SetScrollToMax() 
        {
            ChatArea.ScrollPosition = new Vector2(0, int.MaxValue);
        }
    }
}
