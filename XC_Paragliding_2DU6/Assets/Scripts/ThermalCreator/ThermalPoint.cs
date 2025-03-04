using UnityEngine;

public class ThermalPoint : MonoBehaviour
{
    [SerializeField] private float gizmosPointSize = 5f;

    private float timeSinceLastCheck = 0f; // Время, прошедшее с последней попытки
    private float retryTime; // Случайное время ожидания для этой точки

    private void Start()
    {
        // Генерируем случайное время ожидания для этой точки
        retryTime = Random.Range(ThermalPool.Instance.thermalSettings.minRetryTime,
                                 ThermalPool.Instance.thermalSettings.maxRetryTime);
    }

    public void CheckAndSpawnThermal()
    {
        timeSinceLastCheck += Time.deltaTime; // Увеличиваем время с последней проверки

        if (timeSinceLastCheck >= retryTime)
        {
            // Если время ожидания прошло, проверяем шанс создания термика
            if (Random.value <= ThermalPool.Instance.thermalSettings.thermalSpawnChance)
            {
                // Если шанс сработал, создаём термик
                ThermalPool.Instance.GetThermal(transform.position);
            }

            // Сброс времени ожидания
            timeSinceLastCheck = 0f;
            // Генерируем новый случайный интервал ожидания
            retryTime = Random.Range(ThermalPool.Instance.thermalSettings.minRetryTime,
                                 ThermalPool.Instance.thermalSettings.maxRetryTime);
        }
    }

    // ✅ Рисуем точку в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Цвет точки
        Gizmos.DrawSphere(transform.position, gizmosPointSize); // Радиус 0.5 для наглядности
    }
}

