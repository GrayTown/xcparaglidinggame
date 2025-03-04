using UnityEngine;

[CreateAssetMenu(fileName = "ParaplanSettings", menuName = "XC/Paraplan Settings")]
public class ParaplanSettings : ScriptableObject
{
    public float _verticalSpeed = -1;
    public float _horizontalSpeed = 10;
    public float _smoothDeltaSpeed = 2;
    public bool _currentDirection = false;
}
