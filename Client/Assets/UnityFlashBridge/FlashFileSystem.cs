using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class FlashFileSystem : MonoBehaviour
{
	public const string DIRECTORY = "directory";
	public const string FILE = "file";
	public const string BOTH = "both";
	public const string SUCCESS = "success";
	public const string PENDING = "pending";
	public const string FAILURE = "failure";
    public const int UNLIMITED_SIZE = 1024*1024*10 + 1;

	private static IDictionary<string, object> _callbacks = new Dictionary<string, object>();

	public void Init(string sharedObjectName, Action<string> callback)
	{
		var guid = Guid.NewGuid().ToString();
		_callbacks[guid] = callback;
		Application.ExternalCall("unityBridgeInit", sharedObjectName, guid);
	}

	public void GetAsString(string key, Action<string> callback)
	{
		var guid = Guid.NewGuid().ToString();
		_callbacks[guid] = callback;
		Application.ExternalCall("unityBridgeGet", key, guid);
	}

	public void GetAsByteArray(string key, Action<byte[]> callback)
	{
		GetAsString(key, value => {
			callback(Convert.FromBase64String(value));
		});
	}

	public void StringCallback(string parameters)
	{
		var split = parameters.Split(new char[]{'|'}, 2);
		var guid = split[0];
		var value = split[1];
		
		var action = _callbacks[guid] as Action<string>;
		action(value);
		_callbacks.Remove(guid);
	}

	public void Exists(string key, Action<bool> callback)
	{
		var guid = Guid.NewGuid().ToString();
		_callbacks[guid] = callback;
		Application.ExternalCall("unityBridgeExists", key, guid);
	}

	public void BooleanCallback(string parameters)
	{
		var split = parameters.Split(new char[]{'|'}, 2);
		var guid = split[0];
		var value = split[1];
		
		var action = _callbacks[guid] as Action<bool>;
		action(value  == SUCCESS);
		_callbacks.Remove(guid);
	}

	public void Set(string key, string value, Action<string> callback)
	{
		var guid = Guid.NewGuid().ToString();
		_callbacks[guid] = callback;
		Application.ExternalCall("unityBridgePut", key, value, guid);
	}

	public void Set(string key, byte[] bytes, Action<string> callback)
	{
		Set(key, Convert.ToBase64String(bytes), callback);
	}

	public void Delete(string key)
	{
		Application.ExternalCall("unityBridgeDelete", key);
	}

	public void GetFiles(string directory, string option, Action<List<string>> callback)
	{
		var guid = Guid.NewGuid().ToString();
		_callbacks[guid] = callback;
		Application.ExternalCall("unityBridgeGetFiles", directory, option, guid);
	}

	public void GetFilesRecursively(string directory, string option, Action<List<string>> callback)
	{
		var guid = Guid.NewGuid().ToString();
		_callbacks[guid] = callback;
		Application.ExternalCall("unityBridgeGetFilesRecursively", directory, option, guid);
	}

	public void StringListCallback(string parameters)
	{
		var split = parameters.Split(new char[]{'|'}, 2);
		var guid = split[0];
		var fileNames = split[1].Split(',').ToList<string>();
		
		var action = _callbacks[guid] as Action<List<string>>;
		action(fileNames);
		_callbacks.Remove(guid);
	}

	public void RequestUserToAdjustSharedObjectSettings(Action callback) 
	{
		var guid = Guid.NewGuid().ToString();
		_callbacks[guid] = callback;
		Application.ExternalCall("unityBridgeRequestUserToAdjustSharedObjectSettings", guid);
	}

    public void RequestMinimumSize(int size, Action<string> callback)
    {
        var guid = Guid.NewGuid().ToString();
        _callbacks[guid] = callback;
        Application.ExternalCall("unityBridgeRequestMinimumSize", size, guid);
    }

	public void NoArgumentCallback(string guid)
	{
		var action = _callbacks[guid] as Action;
		action();
		_callbacks.Remove(guid);
	}
}
