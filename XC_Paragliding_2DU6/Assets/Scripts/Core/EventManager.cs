using System;
using System.Collections.Generic;

public class EventManager
{
    private static EventManager _instance;
    private Dictionary<string, List<(Delegate handler, int priority, int order)>> eventDictionary = new();
    private List<(Delegate handler, int priority, int order)> globalListeners = new();
    
    private int _subscriptionOrder = 0;

    private EventManager() { }

    public static EventManager Instance => _instance ??= new EventManager();

    // Подписка на событие с параметром
    public void Subscribe<T>(string eventName, Action<T> listener, int priority = 0)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = new();
        }
        _subscriptionOrder++;
        eventDictionary[eventName].Add((listener, priority, _subscriptionOrder));
        SortListeners(eventName);
    }

    // Подписка на событие без параметра
    public void Subscribe(string eventName, Action listener, int priority = 0)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = new();
        }
        _subscriptionOrder++;
        eventDictionary[eventName].Add((listener, priority, _subscriptionOrder));
        SortListeners(eventName);
    }

    // Подписка на все события
    public void SubscribeToAll(Action<string> listener, int priority = 0)
    {
        _subscriptionOrder++;
        globalListeners.Add((listener, priority, _subscriptionOrder));
        globalListeners.Sort((a, b) => b.priority.CompareTo(a.priority) != 0 ? b.priority.CompareTo(a.priority) : a.order.CompareTo(b.order));
    }

    // Отписка от события с параметром
    public void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var listeners))
        {
            listeners.RemoveAll(e => e.handler.Equals(listener));
            if (listeners.Count == 0) eventDictionary.Remove(eventName);
        }
    }

    // Отписка от события без параметра
    public void Unsubscribe(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var listeners))
        {
            listeners.RemoveAll(e => e.handler.Equals(listener));
            if (listeners.Count == 0) eventDictionary.Remove(eventName);
        }
    }

    // Отписка от всех событий
    public void UnsubscribeFromAll(Action<string> listener)
    {
        globalListeners.RemoveAll(e => e.handler.Equals(listener));
    }

    // Вызов события с параметром
    public void Publish<T>(string eventName, T eventParam)
    {
        if (eventDictionary.TryGetValue(eventName, out var listeners))
        {
            foreach (var (handler, _, _) in listeners)
            {
                (handler as Action<T>)?.Invoke(eventParam);
            }
        }
        foreach (var (handler, _, _) in globalListeners)
        {
            (handler as Action<string>)?.Invoke(eventName);
        }
    }

    // Вызов события без параметра
    public void Publish(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out var listeners))
        {
            foreach (var (handler, _, _) in listeners)
            {
                (handler as Action)?.Invoke();
            }
        }
        foreach (var (handler, _, _) in globalListeners)
        {
            (handler as Action<string>)?.Invoke(eventName);
        }
    }

    // Сортировка подписчиков по приоритету и порядку подписки
    private void SortListeners(string eventName)
    {
        eventDictionary[eventName].Sort((a, b) => b.priority.CompareTo(a.priority) != 0 ? b.priority.CompareTo(a.priority) : a.order.CompareTo(b.order));
    }
}







