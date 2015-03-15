using System;
using System.Collections.Generic;
using UnityEngine;
using Serving.FileTransferring;

namespace Gridia
{
    // :(
    public class GridiaConstants
    {
        public const int NumTilesInSpritesheetRow = 10;
        public const int SpritesInSheet = NumTilesInSpritesheetRow * NumTilesInSpritesheetRow;
        public const int SpriteSize = 32;
        public const int SpriteSheetSize = 320;
        public const int NumRows = SpriteSheetSize / SpriteSize;
        public const float SpriteUv = 1.0f * SpriteSize / SpriteSheetSize;
        public static int Size, Depth, SectorSize; // :(
        public static long ServerTimeOffset;
        public static bool IsAdmin;
        public static String Version = "alpha-1.4-dev"; // :(
        public static String WorldName;
        public static String ErrorMessage { get; set; }
        public static Action ErrorMessageAction { get; set; }
        public static List<GUISkin> Skins { get; set; }
        public static float GuiScale { get; private set; }
        public static int FontSize { get; private set; }
        public static SoundPlayer SoundPlayer { get; private set; }

        public static void InitializeGuiStuff()
        {
            InitializeSoundPlayer();
            LoadGuiSkins();
            GuiScale = Screen.height / 125f / 4;
            FontSize = (int)(GuiScale * 10);
            var skin = Skins[0];
            skin.textArea.fontSize = FontSize;
            skin.textField.fontSize = FontSize;
            skin.label.fontSize = FontSize;
            skin.box.fontSize = FontSize;
            skin.window.fontSize = FontSize;
            skin.button.fontSize = FontSize;
            skin.toggle.fontSize = FontSize;
        }

        private static void InitializeSoundPlayer()
        {
            if (SoundPlayer != null)
            {
                return;
            }
            var gameObject = new GameObject("SoundPlayerGameObject");
            SoundPlayer = gameObject.AddComponent<SoundPlayer>();
            SoundPlayer.MusicAudio = gameObject.AddComponent<AudioSource>();
            SoundPlayer.SfxAudio = gameObject.AddComponent<AudioSource>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        private static void LoadGuiSkins()
        {
            Skins = new List<GUISkin>();
            for (var i = 0; i < 1; i++)
            {
                Skins.Add(Resources.Load("gridia-gui-skin-" + i) as GUISkin);
            }
        }

        // :(
        public static void DrawErrorMessage()
        {
            if (ErrorMessage == null) return;
            const int width = 600;
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

        // :(
        private static readonly Dictionary<Color, Texture2D> StaticRectTexture = new Dictionary<Color, Texture2D>();
        private static readonly Dictionary<Color, GUIStyle> StaticRectStyle = new Dictionary<Color, GUIStyle>();

        public static void GUIDrawSelector(Rect rect, Color color)
        {
            if (!StaticRectTexture.ContainsKey(color))
            {
                StaticRectTexture[color] = new Texture2D(1, 1);
                StaticRectStyle[color] = new GUIStyle {normal = {background = StaticRectTexture[color]}};
            }
            StaticRectTexture[color].SetPixel(0, 0, color);
            StaticRectTexture[color].Apply();
            GUI.Box(rect, GUIContent.none, StaticRectStyle[color]);
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
