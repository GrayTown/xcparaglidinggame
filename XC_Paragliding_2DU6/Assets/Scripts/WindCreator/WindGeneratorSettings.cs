using UnityEngine;

[CreateAssetMenu(fileName = "WindGeneratorSettings", menuName = "XC/Wind/Wind Generator Settings")]
public class WindGeneratorSettings : ScriptableObject
{
    [Header("Настройки ветров")]
    public int windZonesCount = 4; // Количество зон по X
    public float[] windZonesHeight; // Массив сил для зон
    public float windSpeed = 1f;
    public float minWindSpeed = -8f;
    public float maxWindSpeed = 8f;
    public float windChangeSpeed = 0.3f; // Скорость изменения ветра
    public float windUpdateInterval = 30f; // Интервал обновления ветра в секундах
}
