using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

        EventManager.Instance.Publish("GameStarted", " Я опубликовал cобытие старта игры");
    }

    public void EndGame()
    {
        EventManager.Instance.Publish("GameEnded", "Игра завершена");
    }

    private void OnApplicationQuit()
    {
        EndGame(); // Отправляем событие перед выходом
    }
}
