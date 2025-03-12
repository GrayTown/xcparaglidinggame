using UnityEngine;

[CreateAssetMenu(fileName = "ParaplanSettings", menuName = "XC/Paraplan Settings")]
public class ParaplanSettings : ScriptableObject
{
    [Header("Установки глайда")]
    public float VerticalSpeed = -1;
    public float HorizontalSpeed = 10;
    public float turnSpeed = 2f;

    [Header("Начальное направление (вправо)")]
    public bool CurrentDirection = false;
}
