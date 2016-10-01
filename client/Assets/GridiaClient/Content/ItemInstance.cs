using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gridia
{
    public class ItemInstance
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
        
        public ItemInstance(Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return Item.Name + ((Quantity != 1) ? String.Format(" ({0})", Quantity) : "");
        }
    }

    public class ItemInstanceConverter : JsonCreationConverter<ItemInstance>
    {
        protected override ItemInstance Create(Type objectType, JObject jObject)
        {
            var id = (int)jObject["type"];
            var quantity = (int)jObject["quantity"];
            return Locator.Get<ContentManager>().GetItem(id).GetInstance(quantity);
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
                                        Type objectType,
                                         object existingValue,
                                         JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer,
                                       object value,
                                       JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}