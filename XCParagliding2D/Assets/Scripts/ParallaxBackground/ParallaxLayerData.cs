using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewParallaxLayer", menuName = "XC/Parallax/Layer Data")]
public class ParallaxLayerData : ScriptableObject
{
    public string layerName = "NewLayer";
    public float speedX = 1; // �������� ������ �� X
    public float speedY = 0; // �������� ������ �� Y
    public float smoothness = 1; // ��������� ������
    public float startPositionX = 0;
    public float startPositionY = 0;
}

