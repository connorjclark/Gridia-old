using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serving
{
    public interface SocketHandler
    {
        void Start(Action onConnectionSettled, SocketHandler topLevelSocketHandler);

        void Send(Message message);

        void Close();

        JavaBinaryWriter GetOutputStream();

        JavaBinaryReader GetInputStream();
    }
}
