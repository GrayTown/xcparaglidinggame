using System.Collections.Generic;
using UnityEngine;

public class ThermalsSpawner : MonoBehaviour
{
    public GameObject thermalPrefab;
    public float spawnRange = 100f;
    public float spawnInterval = 2f;
    public float maxThermalWidth = 10f;
    public GameObject cloudBase;

    private float lastSpawnTime = 0;
    private List<float> activeThermals = new List<float>();
    private float paragliderPosition = 0;
    private float direction = 1; // 1 - вправо, -1 - влево
    private float thermalYSize = 0;
    private bool isParagliderPositionSet = false; // Флаг, получены ли координаты параплана

    private void Start()
    {
        // Ждем обновления позиции параплана перед первым спавном
        isParagliderPositionSet = false;
    }

    void Update()
    {
        if (!isParagliderPositionSet) return; // Ждем первую позицию
        // Проверяем, прошел ли интервал для генерации нового термика
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            lastSpawnTime = Time.time;
            SpawnThermal();
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<float>("ThermalDestroy", OnThermalDestroyed);
        EventManager.Instance.Subscribe<bool>("ParagliderDirection", OnChangeSpawnDirection);
        EventManager.Instance.Subscribe<float>("ParagliderPosition", OnChangeSpawnPosition, 0);
        EventManager.Instance.Subscribe<Vector2>("ThermalSizeXY", ThermalSizeXYPosition, 1);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<float>("ThermalDestroy", OnThermalDestroyed);
        EventManager.Instance.Unsubscribe<bool>("ParagliderDirection", OnChangeSpawnDirection);
        EventManager.Instance.Unsubscribe<float>("ParagliderPosition", OnChangeSpawnPosition);
        EventManager.Instance.Unsubscribe<Vector2>("ThermalSizeXY", ThermalSizeXYPosition);
    }

    private void SpawnThermal()
    {
        float spawnPosition = GetSpawnPosition();
        float newPos = SetYSpawnPosition();

        if (!IsThermalTooClose(spawnPosition))
        {
            GameObject thermal = Instantiate(thermalPrefab, new Vector3(spawnPosition, newPos, 0), Quaternion.identity);
            activeThermals.Add(thermal.transform.position.x);
        }
    }

    private float GetSpawnPosition()
    {
        float basePosition = paragliderPosition + (direction * spawnRange);
        return Random.Range(basePosition, basePosition + (direction * maxThermalWidth));
    }

    private float SetYSpawnPosition()
    {
        float yPos = 600 - thermalYSize;
        return yPos;
    }

    private void ThermalSizeXYPosition(Vector2 thermalSizeXY)
    {
        thermalYSize = thermalSizeXY.y;
    }


    private void OnChangeSpawnPosition(float getPosition)
    {
        paragliderPosition = getPosition;
        isParagliderPositionSet = true;

    }

    private bool IsThermalTooClose(float spawnPosition)
    {
        foreach (float pos in activeThermals)
        {
            if (Mathf.Abs(spawnPosition - pos) < maxThermalWidth)
            {
                return true;
            }
        }
        return false;
    }

    private void OnChangeSpawnDirection(bool getDirection) 
    {
        if (getDirection)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
    }

    private void OnThermalDestroyed(float position)
    {
        activeThermals.Remove(position);
        SpawnThermal();
    }
}


