using UnityEngine;
using Victor.Tools;

public class ThermalV2 : MonoBehaviour
{
    [SerializeField]
    [VTRangeStep(0f, 10f, 0.25f)]
    private float _thermalVerticalSpeed = 0f;

    public float ThermalVerticalSpeed => _thermalVerticalSpeed;
}
