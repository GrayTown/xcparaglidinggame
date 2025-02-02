using UnityEngine;

[CreateAssetMenu(fileName = "ParallaxLayerSettings", menuName = "XCParagliding/Parallax/LayerSettings")]
public class ParallaxLayerSettings : ScriptableObject
{
    public string layerName; // »м¤ сло¤
    public int sortingOrder; // —лой сортировки
    [Range(0f, 1f)]
    public float speedX; // —корость по оси X
    [Range(0f, 1f)]
    public float speedY; // —корость по оси Y
    public string parallaxGameObjectName; // сам объект движени¤
    [Range(-20f, 20f)]
    public float startPositionX; // —корость по оси X
    [Range(-20f, 20f)]
    public float startPositionY; // —корость по оси Y
}
