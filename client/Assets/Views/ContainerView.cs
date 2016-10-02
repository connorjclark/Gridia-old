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
    using UnityEngine.EventSystems;
    public class ContainerView : UIView
    {
        #region Fields

        public Views.UI.List Container;
        public ObservableList<ItemInstance> Items;
        public int NumInRow = 10;
        public Views.UI.ListItem Template;

        public Views.UI.Label ContainerNameLabel;
        public String ContainerName;

        public int ContainerId;

        #endregion Fields

        #region Methods

        public override void Initialize()
        {
            base.Initialize();

            float rowSpacing = Container.Spacing.Value.Pixels;
            float rowWidth = NumInRow * Template.Width.Value.Pixels + (NumInRow + 1) * rowSpacing;
            //Container.SetValue(() => Container.Width, ElementSize.FromPixels(rowWidth));
            Container.Width.Value = ElementSize.FromPixels(rowWidth);
        }

        // TODO is there a way to run a callback when a field changes?
        public void SetItems(ObservableList<ItemInstance> items)
        {
            SetValue("Items", items);

            int NumInColumn = (int) Math.Ceiling((float)items.Count() / NumInRow);
            Height.Value = ElementSize.FromPixels(NumInColumn * Template.Height.Value.Pixels + ContainerNameLabel.Height.Value.Pixels);
        }

        public void ClickHandler(PointerEventData eventData)
        {
            Debug.Log("click");
        }

        #endregion Methods
    }
}