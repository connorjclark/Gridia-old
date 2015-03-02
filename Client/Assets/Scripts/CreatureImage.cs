using Newtonsoft.Json.Linq;
using System;

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
                var image = new DefaultCreatureImage
                {
                    SpriteIndex = (int) jObject["spriteIndex"],
                    Width = (int) jObject["width"],
                    Height = (int) jObject["height"]
                };
                return image;
            }
            else
            {
                var image = new CustomPlayerImage
                {
                    Head = (int) jObject["head"],
                    Arms = (int) jObject["arms"],
                    Legs = (int) jObject["legs"],
                    Chest = (int) jObject["chest"],
                    Weapon = (int) jObject["weapon"],
                    Shield = (int) jObject["shield"]
                };
                return image;
            }
        }
    }
}
