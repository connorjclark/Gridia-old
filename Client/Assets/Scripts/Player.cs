using UnityEngine;

namespace Gridia
{
    public static class Direction
    {
        public static readonly Vector2 None = new Vector2(0, 0);
        public static readonly Vector2 Up = new Vector2(0, 1);
        public static readonly Vector2 Left = new Vector2(-1, 0);
        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Down = new Vector2(0, -1);

        public static Vector2 RandomDirection() {
            int value = Random.Range(1, 4);
            switch (value) {
                case 1: return Up;
                case 2: return Down;
                case 3: return Left;
                case 4: return Right;
            }
            return None;
        }
    }

    public class Player : Creature {
    }

    public class Creature {
        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; }
        public int Image { get; set; }

        public Vector2 MovementDirection { get; set; }

        public Creature() {
            MovementDirection = Direction.None;
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
