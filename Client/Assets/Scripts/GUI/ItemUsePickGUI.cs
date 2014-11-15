using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ItemUsePickGUI : GridiaWindow
    {
        private ItemUse[] _uses;
        private int _listEntry;
        private bool _listShow;
        private GUIStyle _listStyle;

        public ItemUsePickGUI(Rect rect, List<ItemUse> uses)
            : base(rect, "Chat")
        {
            _uses = uses.ToArray();
            // Make a GUIStyle that has a solid white hover/onHover background to indicate highlighted items
	        _listStyle = new GUIStyle();
	        _listStyle.normal.textColor = Color.white;
	        var tex = new Texture2D(2, 2);
	        var colors = new Color[4];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
            }
	        tex.SetPixels(colors);
	        tex.Apply();
	        _listStyle.hover.background = tex;
	        _listStyle.onHover.background = tex;
	        _listStyle.padding.left = _listStyle.padding.right = _listStyle.padding.top = _listStyle.padding.bottom = 4;
        }

        protected override void RenderContents()
        {
            Popup.List(new Rect(0, 0, 100, 20), ref _listShow, ref _listEntry, new GUIContent("Click Me!"), _uses, _listStyle, () => 
            {
                Debug.Log("CHOSE " + _listEntry);
            });
        }
    }

    // Popup list created by Eric Haines
    // Popup list Extended by John Hamilton. john@nutypeinc.com
 
    class Popup {
        static int popupListHash = "PopupList".GetHashCode();
	    // Delegate
	    public delegate void ListCallBack();

        public static bool List(Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, object[] list,
                                 GUIStyle listStyle, ListCallBack callBack) 
        {
            return List(position, ref showList, ref listEntry, buttonContent, list, "button", "box", listStyle, callBack);
	    }
 
        public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent,  object[] list,
                                 GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle, ListCallBack callBack) {
            int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
            bool done = false;
            switch (Event.current.GetTypeForControl(controlID)) {
                case EventType.mouseDown:
                    if (position.Contains(Event.current.mousePosition)) {
                        GUIUtility.hotControl = controlID;
                        showList = true;
                    }
                    break;
                case EventType.mouseUp:
                    if (showList) {
                        done = true;
                         // Call our delegate method
				    callBack();
                    }
                    break;
            }
 
            GUI.Label(position, buttonContent, buttonStyle);
            if (showList) {
 
			    // Get our list of strings
			    string[] text = new string[list.Length];
			    // convert to string
			    for (int i =0; i<list.Length; i++)
			    {
				    text[i] = list[i].ToString();
			    }
 
                Rect listRect = new Rect(position.x, position.y, position.width, list.Length * 20);
                GUI.Box(listRect, "", boxStyle);
                listEntry = GUI.SelectionGrid(listRect, listEntry, text, 1, listStyle);
            }
            if (done) {
                showList = false;
            }
            return done;
        }
    }
}
