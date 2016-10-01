using System.IO;

namespace Serving
{
    public abstract class BinaryMessageHandler<S> : MessageHandler<S, JavaBinaryReader>
        where S : SocketHandler
    {
        protected override JavaBinaryReader Interpret(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            return new JavaBinaryReader(stream);
        }
    }
}
