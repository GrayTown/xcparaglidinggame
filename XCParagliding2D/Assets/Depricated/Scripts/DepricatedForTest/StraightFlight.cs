using UnityEngine;

public class StraightFlight : MonoBehaviour
{
    [Header("Flight Parameters")]
    public float forwardSpeed = 10f; // Скорость полета вперед
    public float descentRate = 1.42f; // Скорость снижения (глайд 7:1)
    public float turnTime = 2f; // Время, необходимое для смены направления

    private float turnTimer = 0f;
    private int turnDirection = 0; // -1 (влево), 1 (вправо), 0 (не поворачиваем)
    private int currentDirection = 1; // 1 - летим вправо, -1 - летим влево

    void Update()
    {
        // Обрабатываем ввод с клавиатуры
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnDirection = -1; // Влево
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            turnDirection = 1; // Вправо
        }
        else
        {
            turnDirection = 0; // Если не нажимается ни одна кнопка, не поворачиваем
            turnTimer = 0f; // Сбрасываем таймер, чтобы при следующем нажатии снова ждать turnTime
        }

        // Если кнопка нажата, начинаем отсчет времени
        if (turnDirection != 0)
        {
            turnTimer += Time.deltaTime;
            if (turnTimer >= turnTime)
            {
                currentDirection *= -1; // Меняем направление на противоположное
                turnTimer = 0f; // Сбрасываем таймер, чтобы снова ждать следующий поворот
            }
        }

        // Двигаем параплан в выбранном направлении
        transform.position += new Vector3(currentDirection * forwardSpeed * Time.deltaTime, -descentRate * Time.deltaTime, 0);
    }
}

