using System.Collections.Generic;

namespace Gridia
{
    public class ItemUse
    {
        public int Tool;
        public int Focus;
        public int SuccessTool;
        public List<int> Products;

        public int GetIdentifyingItem()
        {
            if (SuccessTool > 0)
            {
                return SuccessTool;
            }
            return Products.Count != 0 ? Products[0] : 1;
        }
    }
}
