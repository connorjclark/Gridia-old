#region Using Statements
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.UnityProject
{
    /// <summary>
    /// View that showcases the DataGrid.
    /// </summary>
    [InternalView]
    public class Showcase_DataGrid : View
    {
        #region Fields

        public List<DataGrid_Highscore> Highscores;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Highscores = new List<DataGrid_Highscore>();
            Highscores.Add(new DataGrid_Highscore
            {
                Player = "Oiram",
                Score = 599,
                Level = "4-2"
            });
            Highscores.Add(new DataGrid_Highscore
            {
                Player = "Adlez",
                Score = 599,
                Level = "2-6"
            });
            Highscores.Add(new DataGrid_Highscore
            {
                Player = "jo",
                Score = 599,
                Level = "2-4"
            });
            Highscores.Add(new DataGrid_Highscore
            {
                Player = "Legend",
                Score = 994,
                Level = "4-1"
            });
            Highscores.Add(new DataGrid_Highscore
            {
                Player = "PacMan",
                Score = 687,
                Level = "3-5"
            });
            Highscores.Add(new DataGrid_Highscore
            {
                Player = "Mojo",
                Score = 2222,
                Level = "5-5"
            });
            Highscores.Add(new DataGrid_Highscore
            {
                Player = "Steve2",
                Score = 1843,
                Level = "4-2"
            });
        }

        #endregion
    }

    /// <summary>
    /// Example data for showcasing datagrid.
    /// </summary>
    public class DataGrid_Highscore
    {
        public string Player;
        public int Score;
        public string Level;
    }
}
