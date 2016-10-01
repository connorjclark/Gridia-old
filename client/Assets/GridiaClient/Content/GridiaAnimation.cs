namespace Gridia
{
    using System;
    using System.Collections.Generic;

    public class Frame
    {
        #region Properties

        public String Sound
        {
            get; set;
        }

        public int Sprite
        {
            get; set;
        }

        #endregion Properties
    }

    public class GridiaAnimation
    {
        #region Properties

        public List<Frame> Frames
        {
            get; set;
        }

        public int Id
        {
            get; set;
        }

        public String Name
        {
            get; set;
        }

        #endregion Properties
    }
}