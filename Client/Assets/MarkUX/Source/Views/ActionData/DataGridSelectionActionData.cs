#region Using Statements
using MarkUX.Views;
using System;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Contains datagrid selection action data.
    /// </summary>
    public class DataGridSelectionActionData : ActionData
    {
        public Row Row;
    }
}
