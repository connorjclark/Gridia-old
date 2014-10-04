using System;
using UnityEngine;
using System.Collections.Generic;

namespace Gridia
{
    public abstract class ContentLoader<T> where T: new()
    {
        public List<T> Load (string filepath)
        {
            return LoadFromJSON (filepath);
        }
        
        private List<T> LoadFromJSON (string filepath)
        {
            var result = new List<T> ();
            var data = (Resources.Load (filepath) as TextAsset).text;
            var contentList = new JSONObject (data).list;
            foreach (var json in contentList) {
                if (json.IsNull)
                    continue;
                int id = ProcessId (json);
                while (id >= result.Count) {
                    result.Add (new T ());
                }
                result [id] = ProcessJson (json);
            }
            return result;
        }
        
        protected abstract int ProcessId (JSONObject json);
        protected abstract T ProcessJson (JSONObject json);
    }
}