using System;
using System.Collections.Generic;

public class Locator
{
    #region Fields

    private static readonly Dictionary<Type, Object> _services = new Dictionary<Type, Object>();

    #endregion Fields

    #region Methods

    public static T Get<T>()
    {
        if (!_services.ContainsKey(typeof(T)))
        {
            UnityEngine.Debug.LogError("Can not find service of type: " + typeof(T));
        }
        return (T)_services[typeof(T)];
    }

    public static void Provide<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    #endregion Methods
}