using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gridia
{
    public interface CreatureImage
    {
    }

    public class DefaultCreatureImage : CreatureImage
    {
        public int SpriteIndex { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class CustomPlayerImage : CreatureImage
    {
        public int Head { get; set; }
        public int Chest { get; set; }
        public int Legs { get; set; }
        public int Arms { get; set; }
        public int Weapon { get; set; }
        public int Shield { get; set; }
    }

    public class CreatureImageConverter : JsonCreationConverter<CreatureImage>
    {
        protected override CreatureImage Create(Type objectType, JObject jObject)
        {
            if (jObject["spriteIndex"] != null)
            {
                var image = new DefaultCreatureImage();
                image.SpriteIndex = (int)jObject["spriteIndex"];
                image.Width = (int)jObject["width"];
                image.Height = (int)jObject["height"];
                return image;
            }
            else
            {
                var image = new CustomPlayerImage();
                image.Head = (int)jObject["head"];
                image.Arms = (int)jObject["arms"];
                image.Legs = (int)jObject["legs"];
                image.Chest = (int)jObject["chest"];
                image.Weapon = (int)jObject["weapon"];
                image.Shield = (int)jObject["shield"];
                return image;
            }
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}
