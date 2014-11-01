using UnityEngine;

namespace Gridia
{
    public class Creature {
        public Vector3 Position { get; set; }
        public Vector3 Offset { get; set; }
        public int Id { get; private set; }
        public int Image { get; set; }

        public Vector3 MovementDirection { get; set; }

        public Creature(int id, int x, int y, int z) {
            MovementDirection = Direction.None;
            Id = id;
            Position.Set(x, y, z);
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
