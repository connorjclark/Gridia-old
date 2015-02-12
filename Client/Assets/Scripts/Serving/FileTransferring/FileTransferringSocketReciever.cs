using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Serving.FileTransferring
{
    public class FileTransferringSocketReciever : SocketHandler
    {
        public String LocalDataFolder { get; private set; }

        private SocketHandler _socketHandler;
        private FileSystem _fileSystem;
        private JavaBinaryWriter _out;
        private JavaBinaryReader _in;

        public FileTransferringSocketReciever(SocketHandler socketHandler, FileSystem fileSystem)
        {
            _socketHandler = socketHandler;
            _fileSystem = fileSystem;
            _out = socketHandler.GetOutputStream();
            _in = socketHandler.GetInputStream();
        }

        public FileTransferringSocketReciever(SocketHandler socketHandler)
            : this(socketHandler, new RegularFileSystem())
        {
        }

        public void Start(Action onConnectionSettled, SocketHandler topLevelSocketHandler)
        {
            LocalDataFolder = _in.ReadJavaUTF();
            _fileSystem.InitDirectory(LocalDataFolder);
            RespondToHashes();
            ReadNewFilesFromServer();
            _socketHandler.Start(onConnectionSettled, topLevelSocketHandler);
        }

        public void Send(Message message)
        {
            _socketHandler.Send(message);
        }

        public void Close()
        {
            _socketHandler.Close();
        }

        private void RespondToHashes()
        {
            var hashes = ReadFileHashesFromServer();
            var localFiles = _fileSystem.GetFiles(LocalDataFolder);
            var filesToRequest = CompareFileHashes(localFiles, hashes);
            var json = JsonConvert.SerializeObject(filesToRequest);

            _out.WriteJavaUTF(json);
        }

        private Dictionary<String, sbyte[]> ReadFileHashesFromServer()
        {
            var jsonHashes = _in.ReadJavaUTF();
            return JsonConvert.DeserializeObject<Dictionary<String, sbyte[]>>(jsonHashes);
        }

        private List<String> CompareFileHashes(List<String> files, Dictionary<String, sbyte[]> hashes)
        {
            var filesToRequest = new List<String>();

            foreach (var entry in hashes)
            {
                var fileName = entry.Key;
                var fileHash = entry.Value;
                var path = System.IO.Path.Combine(LocalDataFolder, fileName);

                files.Remove(fileName);
                if (!_fileSystem.Exists(path))
                {
                    filesToRequest.Add(fileName);
                }
                else
                {
                    var bytes = _fileSystem.ReadAllBytes(path);
                    var localHash = MD5.Create().ComputeHash(bytes);
                    sbyte[] signed = new sbyte[localHash.Length]; // :(
                    Buffer.BlockCopy(localHash, 0, signed, 0, localHash.Length);
                    if (!fileHash.SequenceEqual(signed))
                    {
                        filesToRequest.Add(fileName);
                    }
                }
            }
            return filesToRequest;
        }

        private void ReadNewFilesFromServer()
        {
            int numFiles = _in.ReadInt32();
            for (int i = 0; i < numFiles; i++)
            {
                var fileName = _in.ReadJavaUTF();
                int length = _in.ReadInt32();
                var data = _in.ReadBytes(length);
                var path = System.IO.Path.Combine(LocalDataFolder, fileName);
                _fileSystem.CreateDirectory(Path.GetDirectoryName(path));
                _fileSystem.Write(path, data);
            }
            _out.Write((byte)0); //done updating files
        }
        
        public JavaBinaryWriter GetOutputStream()
        {
            return _socketHandler.GetOutputStream();
        }

        public JavaBinaryReader GetInputStream()
        {
            return _socketHandler.GetInputStream();
        }
    }
}
