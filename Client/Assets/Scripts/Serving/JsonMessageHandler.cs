using Newtonsoft.Json.Linq;
using System.Text;

namespace Serving
{
    public abstract class JsonMessageHandler<S> : MessageHandler<S, JObject>
        where S : SocketHandler
    {
        protected override JObject Interpret(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JObject.Parse(json);
        }
    }
}
