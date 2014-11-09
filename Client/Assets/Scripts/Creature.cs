using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class Creature {
        class PositionSnapshot 
        {
            public PositionSnapshot(Vector3 position, long time)
            {
                Position = position;
                Timestamp = time;
            }
            public Vector3 Position { get; set; }
            public long Timestamp { get; set; }
        }

        public Vector3 Position {
            set 
            {
                AddPositionSnapshot(value, getSystemTime());
            }
            get 
            {
                PositionSnapshot before = null;
                PositionSnapshot after = null;
                GetSnapshotBeforeAndAfter(getSystemTime() - RENDER_DELAY, out before, out after);
                if (before == null) return Vector3.zero;
                return before.Position;
            } 
        }
        public int Id { get; private set; }
        public int Image { get; set; }
        private List<PositionSnapshot> _positions = new List<PositionSnapshot>();

        public Creature(int id, int image, int x, int y, int z) {
            Id = id;
            Image = image;
            Position.Set(x, y, z);
        }

        public Vector3 Offset { get { return GetOffset(); } }

        public void AddPositionSnapshot(Vector3 position) 
        {
            AddPositionSnapshot(position, getSystemTime());
        }

        public void AddPositionSnapshot(Vector3 position, long time) 
        {
            var snapshot = new PositionSnapshot(position, time);
            _positions.Add(snapshot);
        }

        // : (
        private long getSystemTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private static int RENDER_DELAY = 300; // in ms

        // see https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking

        private void GetSnapshotBeforeAndAfter(long time, out PositionSnapshot before, out PositionSnapshot after) 
        {
            before = after = null;
            for (int i = _positions.Count - 1; i >= 0; i--) 
            {
                var snapshot = _positions[i];
                if (time > snapshot.Timestamp) 
                {
                    before = snapshot;
                    after = i  != _positions.Count - 1 ? _positions[i + 1] : before;
                    break;
                }
            }
        }
        
        public Vector3 GetOffset() 
        {
            long timeToRender = getSystemTime() - RENDER_DELAY;
            PositionSnapshot snapshotBefore = null;
            PositionSnapshot snapshotAfter = null;

            GetSnapshotBeforeAndAfter(timeToRender, out snapshotBefore, out snapshotAfter);

            if (snapshotAfter == snapshotBefore) return Vector3.zero;

            float interp = (float)(timeToRender - snapshotBefore.Timestamp) / (snapshotAfter.Timestamp - snapshotBefore.Timestamp);
            var snapshotPositionDelta = snapshotAfter.Position - snapshotBefore.Position;
            return snapshotPositionDelta * interp;
        }

        private Vector3 _currentPositionInTileMap;
        public void LoadPositionAtCurrentTime() {
            var currentPosition = Position;
            if (currentPosition != _currentPositionInTileMap) 
            {
                var tilemap = Locator.Get<GridiaGame>().tileMap;

                tilemap.GetTile(_currentPositionInTileMap).Creature = null;
                tilemap.GetTile(currentPosition).Creature = this;

                _currentPositionInTileMap = currentPosition;
            }
        }
    }
}
