using UnityEngine;

[CreateAssetMenu(fileName = "ThermalPoolSettings", menuName = "XC/Thermal Pool/Thermal Pool Settings")]
public class ThermalPoolSettings : ScriptableObject
{
    [Header("Множитель размера облака")]
    public Vector2 cloudSize = new Vector2(1f,1f);

    [Header("дельта превышения облака над термиком")]
    public float deltaCloudHighY = 20f;

    [Header("Базовый множитель скороподъемности")]
    [Range(2, 100)] public float addLiftForce = 1f;

    [Header("Базовая скороподъемность по Y")]
    [Range(4, 100)] public float liftForceY = 4f; // Подъемная сила по Y

    [Header("Минимальная сила перед исчезновением по Y")]
    [Range(0, 10)] public float minLiftForceY = 0.5f; // Минимальная сила перед исчезновением


    [Header("Ширина термика")]
    [Range(30, 500)] public float thremalMinWidht = 30;
    [Range(50, 500)] public float thremalMaxWidht = 60;

    [Header("Точек перегиба в термике")]
    [Range(2, 100)] public int minPointCount = 2;
    [Range(2, 100)] public int maxPointCount = 60;

    [Header("Угол наклона термика")]
    [Range(-30f, 30f)] public float minAngle = -15f;
    [Range(-30f, 30f)] public float maxAngle = 15f;

    [Header("Амплитуда, частотак колебаний и пр. термика")]
    [Range(0.1f, 50f)] public float minAmplitude = 5f;
    [Range(0.1f, 500f)] public float maxAmplitude = 30f;

    [Range(0f, 0.2f)] public float minFrequency = 0.1f;
    [Range(0.2f, 0.5f)] public float maxFrequency = 0.5f;

    [Range(0.5f, 20f)] public float minSpeed = 0.3f;
    [Range(0.5f, 200f)] public float maxSpeed = 100f;

    [Header("Термик идет синусом")]
    public bool sinWave = false;

    [Header("Время жизни термика")]
    [Range(10f, 120f)] public float minLifetime = 20f;
    [Range(10f, 120f)] public float maxLifetime = 60f;

    [Header("Диапазон случайного смещения от точки генерации")]
    public Vector2 randomOffsetX = new Vector2(-10f, 10f);
    public Vector2 randomOffsetY = new Vector2(-5f, 5f);
    public float minDistanceBetweenThermals = 10f;

    [Header("Вероятность появления термика")]
    [Range(0f, 1f)] public float thermalSpawnChance = 0.5f; // Вероятность появления термика (от 0 до 1)
    [Range(0f, 120f)] public float minRetryTime = 5f; // проверка спавна через
    [Range(0f, 120f)] public float maxRetryTime = 10f; // --------
}


