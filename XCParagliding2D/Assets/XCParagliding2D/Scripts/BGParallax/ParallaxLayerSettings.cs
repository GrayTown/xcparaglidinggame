using UnityEngine;

[CreateAssetMenu(fileName = "ParallaxLayerSettings", menuName = "XCParagliding/Parallax/LayerSettings")]
public class ParallaxLayerSettings : ScriptableObject
{
    public string layerName; // Имя слоя
    public int sortingOrder; // Слой сортировки
    [Range(0f, 1f)]
    public float speedX; // Скорость по оси X
    [Range(0f, 1f)]
    public float speedY; // Скорость по оси Y
    public string parallaxGameObjectName; // сам объект движения
    [Range(-20f, 20f)]
    public float startPositionX; // Скорость по оси X
    [Range(-20f, 20f)]
    public float startPositionY; // Скорость по оси Y
}
