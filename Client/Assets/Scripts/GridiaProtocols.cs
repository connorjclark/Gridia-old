using Serving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GridiaProtocols : Protocols
{
    public enum Clientbound { AddCreature, MoveCreature, RemoveCreature }
    public enum Serverbound { PlayerMove }

    public GridiaProtocols()
    {
        Add(BoundDest.CLIENT, 0, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 1, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 2, DataMethod.JSON, false);

        Add(BoundDest.SERVER, 0, DataMethod.JSON, false);
    }
}