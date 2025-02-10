using UnityEngine;

public class ThermalGenerator : MonoBehaviour
{
    [Header("Cloud Base Settings")]
    public GameObject cloudBase; // Объект CloudBase
    public float deltaCloudShift = 1.0f;

    [Header("Top of Theraml Prefab Settings")]
    public GameObject topOfThermalPrefab; // Префаб для самого верха

    [Header("Thermal Stack Settings")]
    public GameObject stackThermalPrefab; // Префаб для стопки
    public int thermalCountInStack = 3; // Количество префабов в стопке
    public float thermalDeltaXShift = 1.0f; // Значение сдвига по горизонтали

    [Header("Thermal spawn distance")]
    public float thermalDistanceMinSpawn = 0f;
    public float thermalDistanceMaxSpawn = 100f;

    [Header("Thermal Settings")]
    public float minSpawnTime = 2.0f; // Минимальное время задержки спавна термика
    public float maxSpawnTime = 5.0f; // Максимальное время задержки спавна термика
    [Range(0, 1)]
    public float spawnChance = 0.5f; // Вероятность спавна термика (от 0 до 1)

    private void Start()
    {
        // Запуск первого спауна с случайным интервалом
        Invoke(nameof(TrySpawn), Random.Range(minSpawnTime, maxSpawnTime));
    }

    private void TrySpawn()
    {
        // Проверяем шанс спауна
        if (Random.value <= spawnChance)
        {
            // Вызов спауна
            SpawnThermal();
        }
        else
        {
            Debug.Log("Spawn skipped.");
        }

        // Повторный вызов с случайным интервалом
        Invoke(nameof(TrySpawn), Random.Range(minSpawnTime, maxSpawnTime));
    }

    private void SpawnThermal()
    {
        if (cloudBase == null)
        {
            Debug.LogError("CloudBase не назначен!");
            return;
        }

        // Получаем нижнюю грань CloudBase
        SpriteRenderer cloudBaseRenderer = cloudBase.GetComponent<SpriteRenderer>();
        if (cloudBaseRenderer == null)
        {
            Debug.LogError("У CloudBase нет компонента SpriteRenderer!");
            return;
        }

        float cloudBaseBottom = cloudBase.transform.position.y - (cloudBaseRenderer.bounds.size.y / 2);

        // Получаем размеры верхнего префаба
        SpriteRenderer topRenderer = topOfThermalPrefab.GetComponent<SpriteRenderer>();
        if (topRenderer == null)
        {
            Debug.LogError("У topPrefab нет компонента SpriteRenderer!");
            return;
        }

        float topHeight = topRenderer.bounds.size.y;
        float xPoint = 0;
        xPoint += Random.Range(thermalDistanceMinSpawn, thermalDistanceMaxSpawn);

        // Позиция для верхнего префаба
        Vector2 topPosition = new Vector2(xPoint, cloudBaseBottom + (topHeight / 2) - topRenderer.bounds.size.y / 2);

        // Создаем верхний префаб
        Instantiate(topOfThermalPrefab, topPosition, Quaternion.identity);

        // Получаем размеры префаба для стопки
        SpriteRenderer stackRenderer = stackThermalPrefab.GetComponent<SpriteRenderer>();
        // Позиция для первого префаба в стопке
        Vector2 stackPosition = new Vector2(topPosition.x, cloudBaseBottom - stackRenderer.bounds.size.y / 2);

        if (stackRenderer == null)
        {
            Debug.LogError("У stackPrefab нет компонента SpriteRenderer!");
            return;
        }

        float stackHeight = stackRenderer.bounds.size.y;

        // Создаем стопку префабов
        for (int i = 0; i < thermalCountInStack; i++)
        {
            // Случайный выбор сдвига: 0 — без сдвига, 1 — вправо, 2 — влево
            int shiftDirection = 0;
            shiftDirection += Random.Range(0, 3);

            // Создаем экземпляр префаба
            GameObject instance = Instantiate(stackThermalPrefab, stackPosition, Quaternion.identity);

            // Применяем сдвиг в зависимости от выбранного направления
            switch (shiftDirection)
            {
                case 0: // Без сдвига
                    break;
                case 1: // Сдвиг вправо
                    stackPosition.x += thermalDeltaXShift;
                    break;
                case 2: // Сдвиг влево
                    stackPosition.x -= thermalDeltaXShift;
                    break;
            }

            // Сдвиг вниз на высоту префаба
            stackPosition.y -= stackHeight;
        }
    }

}
