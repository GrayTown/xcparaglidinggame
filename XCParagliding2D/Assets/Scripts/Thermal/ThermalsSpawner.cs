using System.Collections.Generic;
using UnityEngine;

public class ThermalsSpawner : MonoBehaviour
{
    public GameObject thermalPrefab;
    public float spawnRange = 100f;
    public float spawnInterval = 2f;
    public float maxThermalWidth = 10f;
    public GameObject cloudBase;

    private float lastSpawnTime;
    private List<float> activeThermals = new List<float>();
    private float paragliderPosition = 0;
    private float direction = 1; // 1 - вправо, -1 - влево
    private float thermalYSize = 0;

    void Update()
    {
        // Проверяем, прошел ли интервал для генерации нового термика
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            SpawnThermal();
            lastSpawnTime = Time.time;
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<float>("ThermalDestroy", OnThermalDestroyed);
        EventManager.Instance.Subscribe<bool>("ParagliderDirection", OnChangeSpawnDirection);
        EventManager.Instance.Subscribe<float>("ParagliderPosition", OnChangeSpawnPosition);
        EventManager.Instance.Subscribe<float>("ThermalSizeY", ThermalSizeYPosition);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<float>("ThermalDestroy", OnThermalDestroyed);
        EventManager.Instance.Unsubscribe<bool>("ParagliderDirection", OnChangeSpawnDirection);
        EventManager.Instance.Unsubscribe<float>("ParagliderPosition", OnChangeSpawnPosition);
        EventManager.Instance.Unsubscribe<float>("ThermalSizeY", ThermalSizeYPosition);
    }

    private void ThermalSizeYPosition(float thermalSizeY)
    {
        thermalYSize = thermalSizeY;
    }

    private void SpawnThermal()
    {
        float spawnPosition = GetSpawnPosition();
        float newPos = SetYSpawnPosition();

        if (!IsThermalTooClose(spawnPosition))
        {
            GameObject thermal = Instantiate(thermalPrefab, new Vector3(spawnPosition, newPos, 0), Quaternion.identity);
            activeThermals.Add(spawnPosition);
        }
    }

    private float GetSpawnPosition()
    {
        float basePosition = paragliderPosition + (direction * spawnRange);
        return Random.Range(basePosition, basePosition + (direction * maxThermalWidth));
    }

    private float SetYSpawnPosition()
    {
        float yPos = thermalYSize - cloudBase.transform.position.y;
        return yPos;
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

    private void OnChangeSpawnPosition(float getPosition)
    {
        paragliderPosition = getPosition;
    }

    private void OnThermalDestroyed(float position)
    {
        activeThermals.Remove(position);
        SpawnThermal();
    }
}


