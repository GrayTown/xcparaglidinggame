using UnityEngine;

[CreateAssetMenu(fileName = "ParaplanSettings", menuName = "Scriptable Objects/ParaplanSettings")]
public class ParaplanSettings : ScriptableObject
{
    public float forwardSpeed = 5f; // Скорость движения вперед
    public float verticalDescendRate = 2f; // Скорость вертикального снижения в метрах в секунду
}
