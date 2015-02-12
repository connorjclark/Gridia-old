using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.IO;
using System.Text;

namespace Serving
{
    public class JavaBinaryWriter : EndianBinaryWriter
    {
        public JavaBinaryWriter(Stream input)
            : base(EndianBitConverter.Big, input, Encoding.UTF8) { }

        public void WriteJavaUTF(String str)
        {
            Write((ushort)str.Length);
            Write(Encoding.UTF8.GetBytes(str));
        }
    }
}
