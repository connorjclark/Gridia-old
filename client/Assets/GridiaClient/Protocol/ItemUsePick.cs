namespace Gridia.Protocol
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using Serving;

    class ItemUsePick : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var uses = data["uses"].ToObject<List<ItemUse>>();

            var usePickWindow = Locator.Get<ItemUsePickWindow>();
            usePickWindow.Uses = uses;
            var tabbedUI = Locator.Get<TabbedUI>();
            tabbedUI.Add(10, usePickWindow, true);
            usePickWindow.ItemUsePickState = new ItemUsePickState(usePickWindow);
            Locator.Get<StateMachine>().SetState(usePickWindow.ItemUsePickState);
        }

        #endregion Methods
    }
}