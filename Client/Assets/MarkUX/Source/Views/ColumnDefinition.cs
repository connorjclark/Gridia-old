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
    /// Contains a column definition. Used by the datagrid to determine how a column is presented.
    /// </summary>
    [InternalView]    
    public class ColumnDefinition : ContentView
    {
        #region Fields

        public string Header;
        public string Text;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ColumnDefinition()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Returns embedded XML for view.
        /// </summary>
        public override string GetEmbeddedXml()
        {
            return @"<ColumnDefinition>
                        <ContentContainer />
                    </ColumnDefinition>";
        }

        #endregion
    }
}
