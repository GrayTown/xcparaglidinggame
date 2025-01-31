using UnityEngine;
public class AutoScroll : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость движения

    void Update()
    {
        // Получаем ввод от игрока
        float moveX = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправо
        float moveY = Input.GetAxis("Vertical");   // W/S или стрелки вверх/вниз

        // Создаем вектор движения
        Vector2 movement = new Vector2(moveX, moveY);

        // Нормализуем вектор, чтобы диагональное движение не было быстрее
        movement = movement.normalized;

        // Перемещаем объект
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
