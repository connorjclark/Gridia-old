using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// Unity has a strict sandbox, and only allows 5MB of cache.
// Using AS3 and JS, we can get around this, and use Flash's
// more generous sandbox. This requires that a 'File System'
// is abstracted out of ServingJava's C# client.

// 4/2016 Update: Not supporting web version anymore since
// Unity WebPlayer is being phased out. Until the WebGL player
// is stable, this won't be necessary. Until then, just keep the
// abstraction anyways.

namespace Serving.FileTransferring
{
    public interface FileSystem
    {
        void InitDirectory(String path);
        void CreateDirectory(String directory);
        List<String> GetFiles(String directory);
        List<String> GetFiles(String directory, String searchPattern);
        List<String> GetFiles(String directory, String searchPattern, SearchOption searchOption);
        bool Exists(String path);
        bool DirectoryExists(String path);
        byte[] ReadAllBytes(String path);
        string ReadString(String path);
        void Write(String path, byte[] data);
    }
}
