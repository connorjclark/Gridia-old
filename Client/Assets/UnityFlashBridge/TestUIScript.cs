using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Threading;

public class TestUIScript : MonoBehaviour
{
	private FlashFileSystem _flashFileSystem;

	void Start ()
	{
		var getButton = GameObject.Find("GetButton").GetComponent<Button>();
		var setButton = GameObject.Find("SetButton").GetComponent<Button>();
		var deleteButton = GameObject.Find("DeleteButton").GetComponent<Button>();
		var getFilesButton = GameObject.Find("GetFilesButton").GetComponent<Button>();
		var keyField = GameObject.Find("KeyField").GetComponent<InputField>();
		var valueField = GameObject.Find("ValueField").GetComponent<InputField>();
		var recursiveToggle = GameObject.Find("Recursive").GetComponent<Toggle>();
		var fileToggle = GameObject.Find("Files").GetComponent<Toggle>();
		var directoryToggle = GameObject.Find("Directories").GetComponent<Toggle>();
		var asByteArrayToggle = GameObject.Find("AsByteArray").GetComponent<Toggle>();

		_flashFileSystem = GameObject.Find ("FlashGameObject").GetComponent<FlashFileSystem>();

		var sharedObjectName = "com.hoten.unity-flash-bridge-test";
		_flashFileSystem.Init(sharedObjectName, status => {
			Log(String.Format("Init Callback: {0} [{1}]", sharedObjectName, status));
		});

		getButton.onClick.AddListener ( () => {
			Log("Get Clicked");
			var key = keyField.text;
			if (asByteArrayToggle.isOn)
			{
				_flashFileSystem.GetAsByteArray(key, value => {
					Log(String.Format("Get Callback: {0} = {1} ({2})", key, value, value.GetType().Name));
					Log(String.Join(", ", value.Select(x => x.ToString()).ToArray()));
				});
			}
			else
			{
				_flashFileSystem.GetAsString(key, value => {
					Log(String.Format("Get Callback: {0} = {1} ({2})", key, value, value.GetType().Name));
				});
			}
		});

		setButton.onClick.AddListener (() => {
			Log("Set Clicked");
			var key = keyField.text;
			var value = valueField.text;
			if (asByteArrayToggle.isOn)
			{
				byte[] valueAsByteArray = value.Split(new char[]{','}).ToList().Select(v => {
					return Convert.ToByte(v.Trim());
				}).ToArray();
				Set(key, valueAsByteArray);
			}
			else
			{
				Set(key, value);
			}
		});

		deleteButton.onClick.AddListener (() => {
			Log("Delete Clicked");
			_flashFileSystem.Delete(keyField.text);
		});

		getFilesButton.onClick.AddListener (() => {
			Log("Get Files Clicked");

			string option;
			if (fileToggle.isOn && directoryToggle.isOn)
			{
				option = FlashFileSystem.BOTH;
			}
			else if (fileToggle.isOn)
			{
				option = FlashFileSystem.FILE;
			}
			else
			{
				option = FlashFileSystem.DIRECTORY;
			}

			if (recursiveToggle.isOn)
			{
				_flashFileSystem.GetFilesRecursively (keyField.text, option, (fileNames) => {
					Log("Get Files Recursively Callback: " + string.Join(", ", fileNames.ToArray()));
				});
			}
			else
			{
				_flashFileSystem.GetFiles (keyField.text, option, (fileNames) => {
					Log("Get Files Callback: " + string.Join(", ", fileNames.ToArray()));
				});
			}
		});

		StressTest();
	}

	void Log(string message)
	{
		Debug.Log(message);
		if (!Application.isEditor)
		{
			Application.ExternalCall("console.log", "UNITY: " + message);
		}
	}

	void Set(string key, string value)
	{
		_flashFileSystem.Set(key, value, status => {
			SetCallback(key, value, status);
		});
	}

		void Set(string key, byte[] value)
	{
		_flashFileSystem.Set(key, value, status => {
			SetCallback(key, value, status);
		});
	}

	void SetCallback(string key, object value, string status)
	{
		Log(String.Format("Set Callback: {0} = {1} [{2}]", key, value, status));
		if (status == FlashFileSystem.PENDING) 
		{
			Log("Request User To Adjust SharedObject Settings");
			_flashFileSystem.RequestUserToAdjustSharedObjectSettings(() => {
				Log("Request User To Adjust SharedObject Settings Done");
			});
		}
	}

	void StressTest()
	{
		Log("StressTest: stress.dat");
		var key = "stress.dat";
		var data = new byte[40570];
		for (var i = 0; i < data.Length; i++)
		{
			data[i] = (byte) (i % 256);
		}
		Set(key, data);
	}
}
