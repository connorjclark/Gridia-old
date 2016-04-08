#region Using Statements
using MarkUX.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Contains a row definition. Used by the datagrid to determine how rows are presented.
    /// </summary>
    [InternalView]    
    public class RowDefinition : ContentView
    {
        #region Fields

        private List<ColumnDefinition> _columnDefinitions;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public RowDefinition()
        {
            _columnDefinitions = new List<ColumnDefinition>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // look for row and column definitions
            this.ForEachChild<ColumnDefinition>(x =>
            {
                x.Deactivate();
                _columnDefinitions.Add(x);
            }, false);
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<RowDefinition />";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets column definitions.
        /// </summary>
        public List<ColumnDefinition> ColumnDefinitions
        {
            get 
            {
                return _columnDefinitions;
            }
        }

        #endregion
    }
}
