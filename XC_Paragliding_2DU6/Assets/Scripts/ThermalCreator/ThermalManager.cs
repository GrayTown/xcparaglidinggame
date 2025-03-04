using UnityEngine;
using System.Collections;

public class ThermalManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    
    private void Update()
    {
        foreach (Transform spawn in spawnPoints)
        {
            // Проверяем время для каждой точки
            ThermalPoint thermalPoint = spawn.GetComponent<ThermalPoint>();
            if (thermalPoint != null)
            {
                thermalPoint.CheckAndSpawnThermal();
            }
        }
    }
}

