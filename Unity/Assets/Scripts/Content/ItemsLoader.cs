using System;

namespace Gridia
{
    public class ItemsLoader : ContentLoader<Item>
    {
        protected override int ProcessId (JSONObject json)
        {
            return json.GetField ("item").i;
        }
        
        protected override Item ProcessJson (JSONObject json)
        {
            Item item = new Item ();
            item.Id = GetInt (json, "id");
            item.Animations = ConvertJsonArray (json.GetField ("animations"));
            item.Light = GetInt (json, "light");
            return item;
        }

        private int GetInt (JSONObject json, String name)
        {
            JSONObject field = json.GetField (name);
            return field != null ? field.i : 0;
        }
        
        private int[] ConvertJsonArray (JSONObject json)
        {
            if (json == null || json.IsNull)
                return new int[0];
            int[] result = new int[json.list.Count];
            for (int i = 0; i < result.Length; i++) {
                result [i] = json.list [i].i;
            }
            return result;
        }
    }
}