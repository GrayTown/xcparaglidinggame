using UnityEngine;

[CreateAssetMenu(fileName = "ParallaxGlobalSettings", menuName = "XCParagliding/Parallax/GlobalSettings")]
public class ParallaxGlobalSettings : ScriptableObject
{
    [Range(0f, 10f)]
    public float globalSpeed = 1f; // ���������� �������� ����������
    public ParallaxLayerSettings[] layers; // ������ �������� ��� ������� ����
    [Range(-1, 1f)]
    public int inversionX = 1;
    [Range(-1, 1)]
    public int inversionY = 1;
}
