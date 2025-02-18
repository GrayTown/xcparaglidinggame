using UnityEngine;

public class GeneratedThermalData
{
    public int ID { get; set; }
    public GameObject ThermalGameObject { get; set; }
    public string ParentName { get; set; }

    public GeneratedThermalData(int id, GameObject thermalGO, string parentName)
    {
        ID = id;
        ThermalGameObject = thermalGO;
        ParentName = parentName;
    }
}
