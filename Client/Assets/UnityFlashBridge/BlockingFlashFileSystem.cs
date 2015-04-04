/*
  This class provides a blocking interface for the Flash file system. 
  This class CAN NOT be used on the main thread.
  This class MUST BE instantiated on the main thread.
*/

using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class BlockingFlashFileSystem
{
    private FlashFileSystem _flashFileSystem;
    private GameObject flashGameObject = new GameObject();
    private EventWaitHandle _signal = new AutoResetEvent(false);

    public BlockingFlashFileSystem()
    {
        MainThreadQueue.Instantiate();
        Object.Instantiate(flashGameObject);
        flashGameObject.name = "FlashGameObject";
        _flashFileSystem = flashGameObject.AddComponent<FlashFileSystem>();
    }

    private EventWaitHandle GetLock()
    {
        //return new AutoResetEvent(false);
        return _signal;
    }

    public void EnsureSharedObjectSettingsAreValid()
    {
        var signal = GetLock();
        MainThreadQueue.Add(() =>
            _flashFileSystem.RequestUserToAdjustSharedObjectSettings(() =>
                signal.Set()
                )
            );
        signal.WaitOne();
    }

    public void Init(string key)
    {
        var signal = GetLock();
        var status = FlashFileSystem.FAILURE;
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.Init(key, theStatus =>
            {
                status = theStatus;
                signal.Set();
            });
        });
        signal.WaitOne();

        if (status == FlashFileSystem.FAILURE)
        {
            EnsureSharedObjectSettingsAreValid();
        }
    }

    public string GetAsString(string key)
    {
        var signal = GetLock();
        string result = "";
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.GetAsString(key, value =>
            {
                result = value;
                signal.Set();
            });
        });
        signal.WaitOne();
        return result;
    }

    public byte[] GetAsByteArray(string key)
    {
        var signal = GetLock();
        byte[] result = new byte[0];
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.GetAsByteArray(key, value =>
            {
                result = value;
                signal.Set();
            });
        });
        signal.WaitOne();
        return result;
    }

    public void Set(string key, string value)
    {
        var signal = GetLock();
        var status = FlashFileSystem.FAILURE;
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.Set(key, value, theStatus =>
            {
                status = theStatus;
                signal.Set();
            });
        });
        signal.WaitOne();

        if (status == FlashFileSystem.FAILURE || status == FlashFileSystem.PENDING)
        {
            EnsureSharedObjectSettingsAreValid();
        }
    }

    public void Set(string key, byte[] value)
    {
        Set(key, System.Convert.ToBase64String(value));
    }

    public bool Exists(string key)
    {
        var signal = GetLock();
        var result = false;
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.Exists(key, value =>
            {
                result = value;
                signal.Set();
            });
        });
        signal.WaitOne();
        return result;
    }

    public void Delete(string key)
    {
        MainThreadQueue.Add(() => {
                                      _flashFileSystem.Delete(key);
        });
    }

    public List<string> GetFiles(string directory, string option)
    {
        var signal = GetLock();
        List<string> result = new List<string>();
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.GetFiles(directory, option, value =>
            {
                result = value;
                signal.Set();
            });
        });
        signal.WaitOne();
        return result;
    }

    public List<string> GetFilesRecursively(string directory, string option)
    {
        var signal = GetLock();
        List<string> result = new List<string>();
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.GetFilesRecursively(directory, option, value =>
            {
                result = value;
                signal.Set();
            });
        });
        signal.WaitOne();
        return result;
    }

    public void RequestMinimumSize(int size)
    {
        var signal = GetLock();
        var status = FlashFileSystem.FAILURE;
        MainThreadQueue.Add(() =>
        {
            _flashFileSystem.RequestMinimumSize(size, theStatus =>
            {
                status = theStatus;
                signal.Set();
            });
        });
        signal.WaitOne();

        if (status == FlashFileSystem.FAILURE || status == FlashFileSystem.PENDING)
        {
            EnsureSharedObjectSettingsAreValid();
        }
    }
}
