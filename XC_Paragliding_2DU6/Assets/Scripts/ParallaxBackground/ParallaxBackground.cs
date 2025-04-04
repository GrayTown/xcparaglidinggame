using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        public GameObject layerObject; // Ссылка на объект слоя
        public ParallaxLayerData config; // Конфигурация для слоя
    }

    public Layer[] layers; // Массив слоев

    private Vector3 _previousCameraPosition;

    private void Start()
    {
        _previousCameraPosition = Camera.main.transform.position;
    }

    private void Update()
    {
        MoveLayers();
    }

    private void MoveLayers()
    {
        Vector3 cameraDelta = Camera.main.transform.position - _previousCameraPosition;

        foreach (var layer in layers)
        {
            if (layer.layerObject != null && layer.config != null)
            {
                // Рассчитываем сдвиг каждого слоя с учетом скорости и плавности из конфига
                Vector3 newPosition = layer.layerObject.transform.position + new Vector3(
                    cameraDelta.x * layer.config.speedX,
                    cameraDelta.y * layer.config.speedY,
                    0f
                );

                // Применяем плавность сдвига
                layer.layerObject.transform.position = Vector3.Lerp(
                    layer.layerObject.transform.position,
                    newPosition,
                    layer.config.smoothness
                );
            }
        }

        // Обновляем предыдущую позицию камеры для следующего кадра
        _previousCameraPosition = Camera.main.transform.position;
    }
}
