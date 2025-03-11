using UnityEngine;

[CreateAssetMenu(fileName = "ParaplanSettings", menuName = "XC/Paraplan Settings")]
public class ParaplanSettings : ScriptableObject
{
    public float VerticalSpeed = -1;
    public float HorizontalSpeed = 10;
    public float SmoothDeltaSpeed = 2;
    public bool CurrentDirection = false;
    public float ModifierVerticalSpeed = 10f;
}
