namespace Gridia
{
    using System.Collections.Generic;

    public class ItemUse
    {
        #region Fields

        public int Focus;
        public List<int> Products;
        public int SuccessTool;
        public int Tool;

        #endregion Fields

        #region Methods

        public int GetIdentifyingItem()
        {
            if (SuccessTool > 0)
            {
                return SuccessTool;
            }
            return Products.Count != 0 ? Products[0] : 1;
        }

        #endregion Methods
    }
}