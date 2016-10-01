namespace Gridia
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ContentLoader<T>
        where T : new()
    {
        #region Methods

        public List<T> Load(string json)
        {
            return LoadFromJSON(json);
        }

        private List<T> LoadFromJSON(string json)
        {
            var results = JsonConvert.DeserializeObject<List<T>>(json);

            for (var i = 0; i < results.Count; i++)
            {
                if (results[i] == null) {
                    results[i] = new T();
                }
            }

            return results;
        }

        #endregion Methods
    }
}