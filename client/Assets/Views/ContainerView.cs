namespace MarkLight.UnityProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Gridia;

    using MarkLight;
    using MarkLight.ValueConverters;
    using MarkLight.Views.UI;

    using UnityEngine;

    public class ContainerView : UIView
    {
        #region Fields

        public Views.UI.List Container;
        public ObservableList<ItemInstance> Items;
        public int NumInRow = 10;
        public Views.UI.ListItem Template;

        #endregion Fields

        #region Methods

        public override void Initialize()
        {
            base.Initialize();

            float rowSpacing = Container.Spacing.Value.Pixels;
            float rowWidth = NumInRow * Template.Width.Value.Pixels + (NumInRow - 2) * 2 * rowSpacing;
            //Container.SetValue(() => Container.Width, ElementSize.FromPixels(rowWidth));
            Container.Width.Value = ElementSize.FromPixels(rowWidth);
        }

        #endregion Methods
    }
}