using Gridia;
using MarkLight;
using MarkLight.Views.UI;
using MarkLight.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MarkLight.UnityProject
{
    public class ContainerView : UIView
    {
        public static Dictionary<int, ObservableList<ItemInstance>> ContainerModels = new Dictionary<int, ObservableList<ItemInstance>>();

        public int NumInRow = 10;
        public ObservableList<ItemInstance> Items;
        public Views.UI.List Container;
        public Views.UI.ListItem Template;

        public override void Initialize()
        {
            base.Initialize();

            float rowSpacing = Container.Spacing.Value.Pixels;
            float rowWidth = NumInRow * Template.Width.Value.Pixels + (NumInRow - 2) * 2 * rowSpacing;
            //Container.SetValue(() => Container.Width, ElementSize.FromPixels(rowWidth));
            Container.Width.Value = ElementSize.FromPixels(rowWidth);
        }
    }
}
