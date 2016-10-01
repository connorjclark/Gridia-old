using MiscUtil.Conversion;
using MiscUtil.IO;
using System.IO;
using System.Text;

namespace Serving
{
    public class JavaBinaryReader : EndianBinaryReader
    {
        public JavaBinaryReader(Stream input)
            : base(EndianBitConverter.Big, input, Encoding.UTF8) { }

        public string ReadJavaUTF()
        {
            ushort len = ReadUInt16();
            byte[] chars = ReadBytes(len);
            return Encoding.UTF8.GetString(chars);
        }
    }
}
