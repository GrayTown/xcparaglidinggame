using UnityEngine;
using UnityEngine.Rendering;

public class ParallaxBackground : MonoBehaviour
{
    public ParallaxGlobalSettings globalSettings; // Ссылка на глобальные настройки

    private Transform[] layers; // Массив трансформ слоев
    private Vector3[] startPositions; // Начальные позиции слоев

    private void Start()
    {
        // Инициализация слоев
        layers = new Transform[globalSettings.layers.Length];
        startPositions = new Vector3[globalSettings.layers.Length];

        for (int i = 0; i < globalSettings.layers.Length; i++)
        {
            // Создаем объект для слоя
            GameObject layerObject = GameObject.Find(globalSettings.layers[i].parallaxGameObjectName);
            layerObject.transform.parent = transform;
            if (globalSettings.layers[i].startPositionX != 0 || globalSettings.layers[i].startPositionY != 0)
            {
                layerObject.transform.localPosition = new Vector3(globalSettings.layers[i].startPositionX, globalSettings.layers[i].startPositionY,0);
            }
            else
            {
                layerObject.transform.localPosition = Vector3.zero;
            }
            // Добавляем спрайт (нужно указать спрайт вручную или через код)
            //SpriteRenderer renderer = layerObject.GetComponent<SpriteRenderer>();
            //renderer.sortingOrder = globalSettings.layers[i].sortingOrder;

            SortingGroup sg = layerObject.GetComponent<SortingGroup>();
            sg.sortingOrder = globalSettings.layers[i].sortingOrder;

            layers[i] = layerObject.transform;
            startPositions[i] = layerObject.transform.position;
        }
    }

    private void Update()
    {
        
        // Применяем параллакс-эффект
        for (int i = 0; i < layers.Length; i++)
        {
            Vector3 newPosition = startPositions[i] + new Vector3(
                globalSettings.globalSpeed 
                * globalSettings.layers[i].speedX
                * Camera.main.transform.position.x
                * globalSettings.inversionX
                ,
                globalSettings.globalSpeed 
                * globalSettings.layers[i].speedY 
                * Camera.main.transform.position.y
                * globalSettings.inversionY
                , 0);
            layers[i].position = newPosition;
        }
    }
}
