using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Пример: закрыть приложение по нажатию клавиши Esc
        {
            Application.Quit();
        }
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
