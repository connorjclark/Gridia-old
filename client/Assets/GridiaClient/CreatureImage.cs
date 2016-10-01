namespace Gridia
{
    using System;

    using Newtonsoft.Json.Linq;

    public interface CreatureImage
    {
    }

    public class CreatureImageConverter : JsonCreationConverter<CreatureImage>
    {
        #region Methods

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

        #endregion Methods
    }

    public class CustomPlayerImage : CreatureImage
    {
        #region Properties

        public int Arms
        {
            get; set;
        }

        public int Chest
        {
            get; set;
        }

        public int Head
        {
            get; set;
        }

        public int Legs
        {
            get; set;
        }

        public int Shield
        {
            get; set;
        }

        public int Weapon
        {
            get; set;
        }

        #endregion Properties
    }

    public class DefaultCreatureImage : CreatureImage
    {
        #region Properties

        public int Height
        {
            get; set;
        }

        public int SpriteIndex
        {
            get; set;
        }

        public int Width
        {
            get; set;
        }

        #endregion Properties
    }
}