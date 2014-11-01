using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gridia
{
    public class ContentLoader<T> where T: new()
    {
        public List<T> Load(string filepath)
        {
            return LoadFromJSON(filepath);
        }

        private List<T> LoadFromJSON(string filepath)
        {
            var data = File.ReadAllText(filepath, Encoding.UTF8);
            var results = JsonConvert.DeserializeObject<List<T>>(data);

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