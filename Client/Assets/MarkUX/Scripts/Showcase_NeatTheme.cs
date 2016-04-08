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
    /// View that showcases the neat theme.
    /// </summary>
    public class Showcase_NeatTheme : View
    {
        #region Fields

        public List<Showcase_NeatTheme_Picture> Pictures;
        public ViewSwitcher ViewSwitcher;
        public ViewAnimation ResizeMediaPlayerWindow;
        public ViewSwitcher MediaImageSwitcher;
        private bool _isMediaPlayerMaximized = false;
        private int _viewIndex = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Pictures = new List<Showcase_NeatTheme_Picture>();
            Pictures.Add(new Showcase_NeatTheme_Picture { Name = "Night Path" });
            Pictures.Add(new Showcase_NeatTheme_Picture { Name = "Reflection" });
            Pictures.Add(new Showcase_NeatTheme_Picture { Name = "Autumn Trees" });
        }

        public void More()
        {
            // loop view index between 0 - ViewCount
            _viewIndex = (_viewIndex + 1) % ViewSwitcher.ViewCount;
            ViewSwitcher.SwitchTo(_viewIndex);
        }

        public void ResizeMediaPlayer()
        {
            if (_isMediaPlayerMaximized)
            {
                ResizeMediaPlayerWindow.ReverseAnimation();
                _isMediaPlayerMaximized = false;
            }
            else
            {
                ResizeMediaPlayerWindow.StartAnimation();
                _isMediaPlayerMaximized = true;
            }
        }

        public void MediaSelectionChanged(ListSelectionActionData eventData)
        {
            // swap media based on selection
            MediaImageSwitcher.SwitchTo(eventData.ListItem.ZeroBasedIndex % 3);
        }

        #endregion
    }

    /// <summary>
    /// Neat pictures.
    /// </summary>
    public class Showcase_NeatTheme_Picture
    {
        public string Name;
    }
}
