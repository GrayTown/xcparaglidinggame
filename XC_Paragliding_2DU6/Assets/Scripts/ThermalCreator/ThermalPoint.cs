using UnityEngine;

public class ThermalPoint : MonoBehaviour
{
    [SerializeField] private float _gizmosPointSize = 5f;

    private float _timeSinceLastCheck = 0f; // Время, прошедшее с последней попытки
    private float _retryTime; // Случайное время ожидания для этой точки

    private void Start()
    {
        // Генерируем случайное время ожидания для этой точки
        _retryTime = Random.Range(ThermalPool.Instance.thermalSettings.minRetryTime,
                                 ThermalPool.Instance.thermalSettings.maxRetryTime);
    }

    public void CheckAndSpawnThermal()
    {
        _timeSinceLastCheck += Time.deltaTime; // Увеличиваем время с последней проверки

        if (_timeSinceLastCheck >= _retryTime)
        {
            // Если время ожидания прошло, проверяем шанс создания термика
            if (Random.value <= ThermalPool.Instance.thermalSettings.thermalSpawnChance)
            {
                // Если шанс сработал, создаём термик
                ThermalPool.Instance.GetThermal(transform.position);
            }

            // Сброс времени ожидания
            _timeSinceLastCheck = 0f;
            // Генерируем новый случайный интервал ожидания
            _retryTime = Random.Range(ThermalPool.Instance.thermalSettings.minRetryTime,
                                 ThermalPool.Instance.thermalSettings.maxRetryTime);
        }
    }

    // ✅ Рисуем точку в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Цвет точки
        Gizmos.DrawSphere(transform.position, _gizmosPointSize); // Радиус 0.5 для наглядности
    }
}

