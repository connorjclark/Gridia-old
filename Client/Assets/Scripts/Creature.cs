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

            public override string ToString() {
                return Timestamp + " " + Position;
            }
        }

        public Vector3 Position {
            set 
            {
                AddPositionSnapshot(value, getSystemTime() - RENDER_DELAY); // needed?
            }
            get 
            {
                return GetPosition();
            } 
        }
        public int Id { get; private set; }
        public CreatureImage Image { get; set; }
        private List<PositionSnapshot> _positions = new List<PositionSnapshot>();

        public Creature(int id, CreatureImage image, int x, int y, int z) {
            Id = id;
            Image = image;
            Position = new Vector3(x, y, z);
        }

        public void ClearSnapshots(int amountToKeep = 0) 
        {
            _positions.RemoveRange(0, _positions.Count - amountToKeep);
        }

        public void AddPositionSnapshot(Vector3 position) 
        {
            AddPositionSnapshot(position, getSystemTime());
        }

        public void AddPositionSnapshot(Vector3 position, long time) 
        {
            var snapshot = new PositionSnapshot(position, time);
            _positions.Add(snapshot);
            if (_positions.Count > 6) 
            {
                _positions.RemoveRange(0, 3);
            }
        }

        // :(
        private long getSystemTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - GridiaConstants.SERVER_TIME_OFFSET;
        }

        public static int RENDER_DELAY = 100; // in ms

        // see https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking

        private void GetSnapshotBeforeAndAfter(long time, out PositionSnapshot before, out PositionSnapshot after) 
        {
            before = after = null;
			// :( think about synchornizing access to _positions
            for (int i = _positions.Count - 1; i >= 0; i--) 
            {
                var snapshot = _positions[i];
                if (time >= snapshot.Timestamp) 
                {
                    before = snapshot;
                    after = i  != _positions.Count - 1 ? _positions[i + 1] : before;
                    break;
                }
            }
        }

        public Vector3 GetPosition() 
        {
            if (_positions.Count == 1)
            {
                return _positions[0].Position;
            }

            long timeToRender = getSystemTime() - RENDER_DELAY;
            PositionSnapshot snapshotBefore = null;
            PositionSnapshot snapshotAfter = null;

            GetSnapshotBeforeAndAfter(timeToRender, out snapshotBefore, out snapshotAfter);

            if (snapshotBefore == null)
            {
                return snapshotAfter != null ? snapshotAfter.Position : Vector3.zero;
            }
            if (snapshotAfter == snapshotBefore) return snapshotBefore.Position;

            float interp = (float)(timeToRender - snapshotBefore.Timestamp) / (snapshotAfter.Timestamp - snapshotBefore.Timestamp);
            var snapshotPositionDelta = snapshotAfter.Position - snapshotBefore.Position;
            return snapshotBefore.Position + snapshotPositionDelta * interp;
        }
    }
}
