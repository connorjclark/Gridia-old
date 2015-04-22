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
                var cre = GameObject.Find("Creature " + id);
                var sc = cre.GetComponent<StatusCircle>();
                var takeDamage = sc.IsActive() && currentHealth < sc.CurrentHealth;
                sc.MaxHealth = maxHealth;
                sc.CurrentHealth = currentHealth;
                if (takeDamage)
                {
                    iTween.ColorFrom(cre, Color.red, 1f);
                }
            });
        }
    }
}
