using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gridia
{
    public class ContentLoader<T> where T: new()
    {
        public List<T> Load(string json)
        {
            return LoadFromJSON(json);
        }

        private List<T> LoadFromJSON(string json)
        {
            var results = JsonConvert.DeserializeObject<List<T>>(json);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i] == null) {
                    results[i] = new T();
                }
            }

            return results;
        }
    }
}