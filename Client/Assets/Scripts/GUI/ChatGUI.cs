using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ChatGUI : GridiaWindow
    {
        public String ChatInput { get; set; }
        public String ChatArea { get; set; }
        private Rect ChatInputRect { get; set; }
        private Rect ChatAreaRect { get; set; }
        private Vector2 ScrollPosition { get; set; }

        private int ChatInputHeight { get; set; }

        public ChatGUI(Vector2 position)
            : base(position, "Chat")
        {
            ChatInputHeight = 20;
            ChatInput = "Hello!";
            ChatArea = "\n\n\n\n\n\n\n\n";
            //ChatInputRect = new Rect(Screen.width / 2, Screen.height - 20, Screen.width / 2, 20);
            //ChatAreaRect = new Rect(Screen.width / 2, Screen.height - 120, Screen.width / 2, 100);
        }

        protected override void RenderContents()
        {
            var chatInputRect = new Rect(0, WindowRect.height - BorderSize * 2 - ChatInputHeight, WindowRect.width - BorderSize * 2, ChatInputHeight);
            var chatAreaRect = new Rect(0, 0, WindowRect.width - BorderSize * 2, WindowRect.height - ChatInputHeight - BorderSize * 2);
            
            ChatInput = GUI.TextField(chatInputRect, ChatInput);

            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(chatInputRect.width), GUILayout.Height(chatAreaRect.height));
            GUILayout.TextArea(ChatArea);
            GUILayout.EndScrollView();

            if (ChatInput != "" && Event.current.type == EventType.keyDown && Event.current.character == '\n')
            {
                Locator.Get<ConnectionToGridiaServerHandler>().Chat(ChatInput);
                ChatInput = "";
            }
        }

        public void SetScrollToMax() 
        {
            ScrollPosition = new Vector2(0, int.MaxValue);
        }
    }
}
