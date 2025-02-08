using UnityEngine;

public class GameLogger : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.Subscribe<string>("GameStarted", OnGameStarted,0);
        EventManager.Instance.Subscribe<string>("GameEnded", OnGameEnded,0);
        EventManager.Instance.Subscribe<string>("GameStarted", OnGameInitialize,1);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<string>("GameStarted", OnGameStarted);
        EventManager.Instance.Unsubscribe<string>("GameEnded", OnGameEnded);
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
}

