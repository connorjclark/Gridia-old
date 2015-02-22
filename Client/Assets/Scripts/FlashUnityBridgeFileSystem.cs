#if UNITY_WEBPLAYER
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using Serving.FileTransferring;

public class FlashUnityBridgeFileSystem : FileSystem
{
    private BlockingFlashFileSystem _flash;
    private AutoResetEvent _signal = new AutoResetEvent(false);

    public FlashUnityBridgeFileSystem()
    {
        _flash = new BlockingFlashFileSystem();
    }

    public void InitDirectory(String path)
    {
        _flash.Init(path);
    }

    public void CreateDirectory(String directory)
    {
        // ...
    }

    public List<String> GetFiles(String directory)
    {
        return _flash.GetFilesRecursively(directory, FlashFileSystem.FILE);
    }

    // TODO: implement
    public List<String> GetFiles(String directory, String searchPattern)
    {
        return _flash.GetFilesRecursively(directory, FlashFileSystem.FILE);
    }

    // TODO: implement
    public List<String> GetFiles(String directory, String searchPattern, SearchOption searchOption)
    {
        return _flash.GetFilesRecursively(directory, FlashFileSystem.FILE);
    }

    public bool Exists(String path)
    {
        return _flash.Exists(path);
    }

    public byte[] ReadAllBytes(String path)
    {
        return _flash.GetAsByteArray(path);
    }

    public String ReadString(String path)
    {
        return _flash.GetAsString(path);
    }

    public void Write(string path, byte[] data)
    {
        UnityEngine.Debug.Log("Write " + path + " data length: " + data.Length);
        _flash.Set(path, data);
    }
}
#endif
