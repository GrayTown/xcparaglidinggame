using System.Collections.Generic;
using UnityEngine;

public class ThermalPool : MonoBehaviour
{
    public static ThermalPool Instance;

    [Header("Настройки термиков")]
    public ThermalPoolSettings thermalSettings; // Подключаем настройки

    [Header("Префаб термика")]
    public GameObject thermalPrefab;

    private Queue<VerticalLine> thermalPool = new Queue<VerticalLine>();
    private List<VerticalLine> activeThermals = new List<VerticalLine>(); // Список активных термиков

    private void Awake()
    {
        Instance = this;
    }

    public VerticalLine GetThermal(Vector3 basePosition)
    {
        if (thermalPrefab == null || thermalSettings == null)
        {
            Debug.LogError("ThermalPool: Не заданы thermalPrefab или thermalSettings!");
            return null;
        }

        // Проверка на пересечение с уже существующими термиками
        foreach (VerticalLine activeThermal in activeThermals)
        {
            float distance = Vector3.Distance(basePosition, activeThermal.transform.position);
            if (distance < thermalSettings.minDistanceBetweenThermals)
            {
                // Если слишком близко, не создаём новый термик
                Debug.Log("Слишком близко термики не создаем!");
                return null;
            }
        }

        // Применяем шанс появления термика
        if (Random.value > thermalSettings.thermalSpawnChance)
        {
            Debug.Log("Без шанса на термик в этой точке...ждем погоды :)");
            return null; // Не создаем термик, если шанс не сработал
        }

        VerticalLine thermal;
        if (thermalPool.Count > 0)
        {
            thermal = thermalPool.Dequeue();
        }
        else
        {
            thermal = Instantiate(thermalPrefab, transform).GetComponent<VerticalLine>();
        }

        Transform cloudBase = GameObject.FindGameObjectWithTag("CloudBase")?.transform;
        if (cloudBase == null)
        {
            Debug.LogError("CloudBase не найден! Проверьте тег в сцене.");
            return null;
        }

        // ---- Генерация случайного смещения ----
        float randomX = Random.Range(thermalSettings.randomOffsetX.x, thermalSettings.randomOffsetX.y);
        float randomY = Random.Range(thermalSettings.randomOffsetY.x, thermalSettings.randomOffsetY.y);
        Vector3 spawnPosition = basePosition + new Vector3(randomX, randomY, 0);

        thermal.parentObject = new GameObject("ThermalBase").transform;
        thermal.parentObject.position = spawnPosition;
        thermal.cloudObject = cloudBase;

        // ---- Генерация случайных параметров ----
        thermal.addLiftForce = thermalSettings.addLiftForce;
        thermal.minLiftForceY = thermalSettings.minLiftForceY;
        thermal.liftForceY = thermalSettings.liftForceY;

        thermal.pointCount = Random.Range(thermalSettings.minPointCount, thermalSettings.maxPointCount);
        thermal.colliderThinkness = Random.Range(thermalSettings.thremalMinWidht, thermalSettings.thremalMaxWidht);

        thermal.angle = Random.Range(thermalSettings.minAngle, thermalSettings.maxAngle);
        thermal.amplitude = Random.Range(thermalSettings.minAmplitude, thermalSettings.maxAmplitude);
        thermal.frequency = Random.Range(thermalSettings.minFrequency, thermalSettings.maxFrequency);
        thermal.speed = Random.Range(thermalSettings.minSpeed, thermalSettings.maxSpeed);
        thermal.thermalLifetime = Random.Range(thermalSettings.minLifetime, thermalSettings.maxLifetime);

        thermal.useSinWave = thermalSettings.sinWave;

        thermal.transform.position = spawnPosition;
        thermal.gameObject.SetActive(true);
        thermal.ResetThermal();

        // Добавляем термик в список активных термиков
        activeThermals.Add(thermal);

        return thermal;
    }

    public void ReturnThermal(VerticalLine thermal)
    {
        thermal.gameObject.SetActive(false);
        activeThermals.Remove(thermal); // Убираем из списка активных термиков
        thermalPool.Enqueue(thermal);
    }
}

