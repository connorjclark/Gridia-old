namespace Gridia
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ItemInstance
    {
        #region Constructors

        public ItemInstance(Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }

        #endregion Constructors

        #region Properties

        public Item Item
        {
            get; set;
        }

        public int Quantity
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Item.Name + ((Quantity != 1) ? String.Format(" ({0})", Quantity) : "");
        }

        #endregion Methods
    }

    public class ItemInstanceConverter : JsonCreationConverter<ItemInstance>
    {
        #region Methods

        protected override ItemInstance Create(Type objectType, JObject jObject)
        {
            var id = (int)jObject["type"];
            var quantity = (int)jObject["quantity"];
            return Locator.Get<ContentManager>().GetItem(id).GetInstance(quantity);
        }

        #endregion Methods
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        #region Methods

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

        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        #endregion Methods
    }
}