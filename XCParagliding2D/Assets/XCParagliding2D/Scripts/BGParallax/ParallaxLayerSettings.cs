using UnityEngine;

[CreateAssetMenu(fileName = "ParallaxLayerSettings", menuName = "XCParagliding/Parallax/LayerSettings")]
public class ParallaxLayerSettings : ScriptableObject
{
    public string layerName; // ��� ����
    public int sortingOrder; // ���� ����������
    [Range(0f, 1f)]
    public float speedX; // �������� �� ��� X
    [Range(0f, 1f)]
    public float speedY; // �������� �� ��� Y
    public string parallaxGameObjectName; // ��� ������ ��������
    [Range(-20f, 20f)]
    public float startPositionX; // �������� �� ��� X
    [Range(-20f, 20f)]
    public float startPositionY; // �������� �� ��� Y
}
