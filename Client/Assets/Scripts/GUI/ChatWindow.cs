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

            ChatInput = new TextField(new Vector2(0, ChatArea.Height + 10), 150, 30);
            ChatInput.OnEnter = SendChatMessage;
            AddChild(ChatInput);

            ChatInput.Text = "Hello!";
        }

        public void append(String line) 
        {
            ChatArea.Text += line + '\n';
            SetScrollToMax();
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
