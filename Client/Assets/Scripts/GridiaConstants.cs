using System;
using System.Collections.Generic;
using UnityEngine;
using Serving.FileTransferring;

namespace Gridia
{
    // :(
    public class GridiaConstants
    {
        public const int NUM_TILES_IN_SPRITESHEET_ROW = 10;
        public const int SPRITES_IN_SHEET = NUM_TILES_IN_SPRITESHEET_ROW * NUM_TILES_IN_SPRITESHEET_ROW;
        public const int SPRITE_SIZE = 32;
        public const int SPRITE_SHEET_SIZE = 320;
        public const int NUM_ROWS = SPRITE_SHEET_SIZE / SPRITE_SIZE;
        public const float SPRITE_UV = 1.0f * SPRITE_SIZE / SPRITE_SHEET_SIZE;
        public static int SIZE, DEPTH, SECTOR_SIZE; // :(
        public static long SERVER_TIME_OFFSET;
        public static bool IS_ADMIN;
        public static String VERSION = "alpha-1.2.dev"; // :(
        public static String WORLD_NAME;
        public static String ErrorMessage { get; set; }
        public static Action ErrorMessageAction { get; set; }
        public static List<GUISkin> Skins { get; set; }

        public static void LoadGUISkins()
        {
            Skins = new List<GUISkin>();
            for (int i = 0; i < 1; i++)
            {
                Skins.Add(Resources.Load("gridia-gui-skin-" + i) as GUISkin);
            }
        }

        // :(
        public static void DrawErrorMessage()
        {
            if (ErrorMessage != null)
            {
                var width = 600;
                var height = GUI.skin.GetStyle("TextArea").CalcHeight(new GUIContent(ErrorMessage), width - 20) + 50;
                var x = (Screen.width - width) / 2;
                var y = (Screen.height - height) / 2;
                GUI.Window(0, new Rect(x, y, width, height + 5), id =>
                {
                    GUILayout.TextArea(ErrorMessage);
                    GUILayout.BeginArea(new Rect((width - 50) / 2, height - 25, 50, 50));
                    if (GUILayout.Button("OK"))
                    {
                        ErrorMessage = null;
                        if (ErrorMessageAction != null)
                        {
                            ErrorMessageAction();
                            ErrorMessageAction = null;
                        }
                    }
                    GUILayout.EndArea();
                }, "Error.");
            }
        }

        // :(
        private static Dictionary<Color, Texture2D> _staticRectTexture = new Dictionary<Color, Texture2D>();
        private static Dictionary<Color, GUIStyle> _staticRectStyle = new Dictionary<Color, GUIStyle>();

        public static void GUIDrawSelector(Rect rect, Color color)
        {
            if (!_staticRectTexture.ContainsKey(color))
            {
                _staticRectTexture[color] = new Texture2D(1, 1);
                _staticRectStyle[color] = new GUIStyle();
                _staticRectStyle[color].normal.background = _staticRectTexture[color];
            }
            _staticRectTexture[color].SetPixel(0, 0, color);
            _staticRectTexture[color].Apply();
            GUI.Box(rect, GUIContent.none, _staticRectStyle[color]);
        }

        public static void OnApplicationQuit()
        {
            Application.CancelQuit();
            Locator.Get<ConnectionToGridiaServerHandler>().Close();
            if (!Application.isEditor)
            {
                #if !UNITY_WEBPLAYER
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                #endif
            }
        }

        public static FileSystem GetFileSystem() {
            #if UNITY_WEBPLAYER
                return new FlashUnityBridgeFileSystem();
            #else
                return new RegularFileSystem();
            #endif
        }
    }
}
