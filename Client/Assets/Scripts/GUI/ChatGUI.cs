using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ChatGUI : GridiaWindow
    {
        private String ChatInput { get; set; }
        private String ChatArea { get; set; }
        private Vector2 ScrollPosition { get; set; }

        private int ChatInputHeight { get; set; }

        public ChatGUI(Vector2 position)
            : base(position, "Chat")
        {
            ChatInputHeight = 20;
            ChatInput = "Hello!";
            ChatArea = "\n\n\n\n\n\n\n\n";
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

        public void append(String line) 
        {
            ChatArea += line + '\n';
            SetScrollToMax();
        }

        private void SetScrollToMax() 
        {
            ScrollPosition = new Vector2(0, int.MaxValue);
        }
    }
}
