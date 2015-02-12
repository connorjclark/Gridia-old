using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Unity has a strict sandbox, and only allows 5MB of cache.
// Using AS3 and JS, we can get around this, and use Flash's
// more generous sandbox. This requires that a 'File System'
// is abstracted out of ServingJava's C# client.

namespace Serving.FileTransferring
{
    public interface FileSystem
    {
        void InitDirectory(String path);
        void CreateDirectory(String directory);
        List<String> GetFiles(String directory);
        bool Exists(String path);
        byte[] ReadAllBytes(String path);
        void Read(String path);
        void Write(String path, byte[] data);
    }
}
