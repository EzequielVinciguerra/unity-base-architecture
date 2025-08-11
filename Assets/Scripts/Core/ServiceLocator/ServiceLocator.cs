using System;
using System.Collections.Generic;

namespace Core.Services
{
    public class ServiceLocator
    {
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();
        private static ServiceLocator _instance;
        private static readonly Dictionary<Type, object> _services = new();

        public void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
                _services[type] = service;
            else
                _services.Add(type, service);
        }
        
        public void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
                _services.Remove(type);
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
                return service as T;

            UnityEngine.Debug.LogError($"[ServiceLocator] Service not found: {type}");
            return null;
        }

        public void Clear() => _services.Clear();
    }
}
