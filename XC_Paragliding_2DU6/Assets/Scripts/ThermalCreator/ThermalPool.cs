using System.Collections.Generic;
using UnityEngine;

public class ThermalPool : MonoBehaviour
{
    public static ThermalPool Instance;

    [Header("Настройки термиков")]
    public ThermalPoolSettings thermalSettings; // Подключаем настройки

    [Header("Префаб термика")]
    public GameObject thermalPrefab;

    [Header("Префаб облака")]
    public GameObject cloudPrefab;

    private Queue<VerticalLine> thermalPool = new Queue<VerticalLine>();
    private List<VerticalLine> activeThermals = new List<VerticalLine>(); // Список активных термиков
    private Queue<GameObject> cloudPool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public VerticalLine GetThermal(Vector3 basePosition)
    {
        if (thermalPrefab == null || thermalSettings == null || cloudPrefab == null)
        {
            Debug.LogError("ThermalPool: Не заданы thermalPrefab ? cloudPrefab или thermalSettings!");
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
        thermal.cloudBaseObject = cloudBase;

        // ---- Zone Y ----
        thermal.addLiftForce = thermalSettings.addLiftForce;
        thermal.minLiftForceY = thermalSettings.minLiftForceY;
        thermal.liftForceY = thermalSettings.liftForceY;

        // ---- Zone X ----
        if (thermalSettings.zoneForcesX != null && thermalSettings.zoneForcesX.Length != 0)
        {
            thermal.zoneCountX = thermalSettings.zoneCountX;
            thermal.zoneForcesX = thermalSettings.zoneForcesX;
        }

        // ---- Генерация случайных параметров ----
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

        // Получаем или создаем облако
        GameObject cloud;
        if (cloudPool.Count > 0)
        {
            cloud = cloudPool.Dequeue();
            cloud.transform.position = new Vector2(spawnPosition.x + thermal.angle * 2,cloudBase.position.y + thermal.colliderThinkness / 2 + thermalSettings.deltaCloudHighY * thermalSettings.cloudSize.y);
            cloud.transform.localScale = new Vector3(thermal.colliderThinkness * thermalSettings.cloudSize.x, thermal.colliderThinkness * thermalSettings.cloudSize.y, 0);
            cloud.SetActive(true);
        }
        else
        {
            cloud = Instantiate(cloudPrefab, new Vector2(spawnPosition.x + thermal.angle * 2, cloudBase.position.y + thermal.colliderThinkness / 2 + thermalSettings.deltaCloudHighY * thermalSettings.cloudSize.y), Quaternion.identity);
            cloud.transform.localScale = new Vector3(thermal.colliderThinkness * thermalSettings.cloudSize.x, thermal.colliderThinkness * thermalSettings.cloudSize.y, 0);
            cloud.transform.SetParent(transform);
        }

        // Устанавливаем скорость анимации облака
        Animator cloudAnimator = cloud.GetComponent<Animator>();
        if (cloudAnimator != null)
        {
            cloudAnimator.speed = cloudAnimator.runtimeAnimatorController.animationClips[0].length / thermal.thermalLifetime;
            cloudAnimator.Play(0);
        }

        thermal.cloudObject = cloud;

        StartCoroutine(RemoveThermalAfterLifetime(thermal, cloud, thermal.thermalLifetime));

        // Добавляем термик в список активных термиков
        activeThermals.Add(thermal);

        return thermal;
    }

    private System.Collections.IEnumerator RemoveThermalAfterLifetime(VerticalLine thermal, GameObject cloud, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        ReturnThermal(thermal, cloud);
    }

    public void ReturnThermal(VerticalLine thermal, GameObject cloud)
    {
        Debug.Log("!!!!!!!!!!! Возвращаю термик в пул" + thermal.name);
        cloud.SetActive(false);
        thermal.elapsedTime = 0;
        thermal.thermalLifetime = 0;
        thermal.gameObject.SetActive(false);
        activeThermals.Remove(thermal);
        thermalPool.Enqueue(thermal);
        cloudPool.Enqueue(cloud);
    }
}