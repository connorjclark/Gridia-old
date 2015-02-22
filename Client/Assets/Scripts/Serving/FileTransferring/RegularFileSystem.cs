#if !UNITY_WEBPLAYER
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
            return new List<String>(Directory.GetFiles(directory, "*", SearchOption.AllDirectories));
        }

        public List<String> GetFiles(String directory, String searchPattern)
        {
            return new List<String>(Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories));
        }

        public List<String> GetFiles(String directory, String searchPattern, SearchOption searchOption)
        {
            return new List<String>(Directory.GetFiles(directory, searchPattern, searchOption));
        }

        public bool Exists(String path)
        {
            return File.Exists(path);
        }

        public byte[] ReadAllBytes(String path)
        {
            return File.ReadAllBytes(path);
        }

        public string ReadString(String path)
        {
            return File.ReadAllText(path);
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
#endif
