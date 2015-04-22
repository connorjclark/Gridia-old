using Newtonsoft.Json.Linq;
using Serving;
using UnityEngine;

namespace Gridia.Protocol
{
    class SetLife : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int)data["id"];
            var currentHealth = (int)data["currentLife"];
            var maxHealth = (int)data["maxLife"];

            MainThreadQueue.Add(() =>
            {
                var sc = GameObject.Find("Creature " + id).GetComponent<StatusCircle>();
                sc.MaxHealth = maxHealth;
                sc.CurrentHealth = currentHealth;
            });
        }
    }
}
