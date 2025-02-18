using System.Collections.Generic;
using UnityEngine;

public class GameLogger : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.Subscribe<string>("GameStarted", OnGameStarted,0);
        EventManager.Instance.Subscribe<string>("GameEnded", OnGameEnded,0);
        EventManager.Instance.Subscribe<string>("GameStarted", OnGameInitialize,1);
        EventManager.Instance.Subscribe<Dictionary<int, GeneratedThermalData>>("RiseUp", RiseUp);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<string>("GameStarted", OnGameStarted);
        EventManager.Instance.Unsubscribe<string>("GameEnded", OnGameEnded);
        EventManager.Instance.Unsubscribe<string>("GameStarted", OnGameInitialize);
        EventManager.Instance.Unsubscribe<Dictionary<int, GeneratedThermalData>>("RiseUp", RiseUp);
    }

    private void OnGameStarted(string message)
    {
        Debug.Log($"[GameLogger] {message} -> Я подписчик на событие UI, срабатываю после класса");
    }

    private void OnGameEnded(string message)
    {
        Debug.Log($"[GameLogger] {message}");
    }

    private void OnGameInitialize(string message)
    {
        Debug.Log($"[GameLogger] Game {message} -> я подписчик на это же событие, но класс - сработаю до UI");
    }

    private void RiseUp(Dictionary<int, GeneratedThermalData> thermalDictionary) 
    {
        foreach (var thd in thermalDictionary) 
        {
            Debug.Log($"Key: {thd.Key}, ### Value: ID = {thd.Value.ID} ### ParentName = {thd.Value.ParentName} ### Name = {thd.Value.ThermalGameObject.name}");
        }
    }

}

