using UnityEngine;

namespace Gridia
{
    public class Creature {
        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; }
        public int Id { get; private set; }
        public int Image { get; set; }

        public Vector2 MovementDirection { get; set; }

        public Creature(int id, int x, int y) {
            MovementDirection = Direction.None;
            Id = id;
            Position.Set(x, y);
        }

        public void Move(float amount) {
            Offset = Offset + MovementDirection * amount;
            if (Utilities.Vector2IsAbsoluteGreaterThanOne(Offset))
            {
                Locator.Get<GridiaGame>().tileMap.UpdateCreature(this, Position + MovementDirection);
                Offset = Vector2.zero;
                MovementDirection = Direction.None;
            }
        }
    }
}
