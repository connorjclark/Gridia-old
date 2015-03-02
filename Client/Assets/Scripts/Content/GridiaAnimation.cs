using System;
using System.Collections.Generic;

namespace Gridia
{
    public class Frame 
    {
        public int Sprite { get; set; }
        public String Sound { get; set; }
    }

    public class GridiaAnimation
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public List<Frame> Frames { get; set; }
    }
}
