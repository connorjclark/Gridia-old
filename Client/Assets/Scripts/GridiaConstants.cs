using System;
using UnityEngine;

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
        public static String VERSION = "alpha-1.0"; // :(
        public static String WORLD_NAME;
        public static String ErrorMessage { get; set; }
        public static Action ErrorMessageAction { get; set; }

        // :(
        public static void DrawErrorMessage()
        {
            if (GridiaConstants.ErrorMessage != null)
            {
                var width = 600;
                var height = 75;
                var x = (Screen.width - width) / 2;
                var y = (Screen.height - height) / 2;
                GUI.Window(0, new Rect(x, y, width, height), id =>
                {
                    if (GUI.Button(new Rect((width - 50) / 2, 30, 50, 20), "OK"))
                    {
                        GridiaConstants.ErrorMessage = null;
                        if (ErrorMessageAction != null)
                        {
                            ErrorMessageAction();
                            ErrorMessageAction = null;
                        }
                    }
                }, GridiaConstants.ErrorMessage);
            }
        }
    }
}
