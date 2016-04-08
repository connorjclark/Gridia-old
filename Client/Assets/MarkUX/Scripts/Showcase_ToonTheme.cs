#region Using Statements
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#endregion

namespace MarkUX.UnityProject
{
    /// <summary>
    /// View that showcases the toon theme.
    /// </summary>
    public class Showcase_ToonTheme : View
    {
        #region Fields

        public List<Showcase_ToonTheme_GameLevel> Levels;
        public ViewSwitcher ViewSwitcher;

        // game settings
        public bool EasyMode;
        public float SoundEffectsVolume;
        public float MusicVolume;
        public string DisplayName;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Levels = new List<Showcase_ToonTheme_GameLevel>();
            for (int i = 0; i < 32; ++i)
            {
                Levels.Add(new Showcase_ToonTheme_GameLevel());
            }
        }

        public void PlayGame()
        {
            ViewSwitcher.SwitchTo(1);
        }

        public void Options()
        {
            ViewSwitcher.SwitchTo(2);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void BackToMainMenu()
        {
            ViewSwitcher.SwitchTo(0);
        }

        public void StartLevel(FlowListItem source)
        {
            //var level = source.Item as Showcase_ToonTheme_GameLevel;
            ViewSwitcher.SwitchTo(3);
        }

        #endregion
    }

    /// <summary>
    /// Game level data.
    /// </summary>
    public class Showcase_ToonTheme_GameLevel
    {
    }
}
