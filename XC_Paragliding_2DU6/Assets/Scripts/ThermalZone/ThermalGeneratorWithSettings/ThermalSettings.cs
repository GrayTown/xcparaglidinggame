using UnityEngine;

[CreateAssetMenu(fileName = "ThermalSettings", menuName = "ScriptableObjects/ThermalSettings", order = 1)]
public class ThermalSettings : ScriptableObject
{
    [Header("Thermal parts prefabs")]
    public GameObject cloudPrefab;
    public GameObject thermalUpPrefab;
    public GameObject thermalDownPrefab;

    [Header("Cloud & cloud base parameters")]
    public Vector2 cloudSize = new Vector2(80f, 30f);
    public float cloudTopShift = 15f;

    [Header("Thermal stack settings")]
    public int stackHeightMin = 150;
    public int stackHeightMax = 250;
    public float verticalSpacing = 0f;
    public Vector2 thermalSize = new Vector2(50f, 2f);
    public float deltaSpacing = 0.5f;

    [Header("Thermal noize settings")]
    public float xOffsetRange = 10f;
    public float frequency = 0.02f;
    public float amplitude = 20.0f;
    public float noiseIntensity = 20.0f;

    [Header("Thermal lift settings")]
    public float liftForceUpMin = 50f;
    public float liftForceUpMax = 79f;
    public float liftForceDownMin = 80f;
    public float liftForceDownMax = 100f;

    [Header("Thermal spawn parameters")]
    public float minXSpawn = -100f;
    public float maxXSpawn = 100f;
    public float spawnXDeltaGenPosCheck = 3f;
    public float spawnIntervalMin = 10f;
    public float spawnIntervalMax = 30f;
    public float spawnChance = 0.7f;
    public float thermalLifeTimeMin = 10f;
    public float thermalLifeTimeMax = 60f;

    [Header("Thermal pool size")]
    public int poolSize = 10;
}
