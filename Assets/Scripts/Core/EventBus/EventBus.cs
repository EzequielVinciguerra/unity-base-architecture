using System;
using System.Collections.Generic;

namespace Core.Events
{
    public class EventBus : IEventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                _subscribers[type] = new List<Delegate>();

            _subscribers[type].Add(callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
                _subscribers[type].Remove(callback);
        }

        public void Publish<T>(T eventData) where T : IEvent
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
                foreach (var callback in _subscribers[type])
                    ((Action<T>)callback)?.Invoke(eventData);
        }
    }
}