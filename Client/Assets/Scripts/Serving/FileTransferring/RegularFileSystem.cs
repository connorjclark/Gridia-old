using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Serving.FileTransferring
{
    public class RegularFileSystem : FileSystem
    {
        public void InitDirectory(String path)
        {
            Directory.CreateDirectory(path);
        }

        public void CreateDirectory(String directory)
        {
            Directory.CreateDirectory(directory);
        }

        public List<String> GetFiles(String directory)
        {
            return new List<String>(Directory.GetFiles(directory));
        }

        public bool Exists(String path)
        {
            return File.Exists(path);
        }

        public byte[] ReadAllBytes(String path)
        {
            return File.ReadAllBytes(path);
        }

        public void Read(string path)
        {
            throw new NotImplementedException();
        }

        public void Write(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }
    }
}
