using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serving
{
    public class JsonMessageBuilder
    {
        private Dictionary<String, Object> _map = new Dictionary<String, Object>();
        private String _type;
        private bool _compressed;

        public JsonMessageBuilder Set(String key, Object value)
        {
            _map[key] = value;
            return this;
        }

        public JsonMessageBuilder Type(String type)
        {
            _type = type;
            return this;
        }

        public JsonMessageBuilder Compressed(bool compressed)
        {
            _compressed = compressed;
            return this;
        }

        public Message Build()
        {
            var json = JsonConvert.SerializeObject(_map);
            var data = Encoding.UTF8.GetBytes(json);
            return new Message(data, _type, _compressed);
        }
    }
}
