using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ParallaxLayer", menuName = "XC/Parallax/Layer Data")]
public class ParallaxLayerData : ScriptableObject
{
    public string layerName = "NewLayer";
    public float speedX = 1; // Скорость сдвига по X
    public float speedY = 0; // Скорость сдвига по Y
    public float smoothness = 1; // Плавность сдвига
    public float startPositionX = 0;
    public float startPositionY = 0;
}

