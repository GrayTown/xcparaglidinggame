using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        public GameObject layerObject; // ������ �� ������ ����
        public ParallaxLayerData config; // ������������ ��� ����
    }

    public Layer[] layers; // ������ �����

    private Vector3 previousCameraPosition;

    private void Start()
    {
        previousCameraPosition = Camera.main.transform.position;
    }

    private void Update()
    {
        MoveLayers();
    }

    private void MoveLayers()
    {
        Vector3 cameraDelta = Camera.main.transform.position - previousCameraPosition;

        foreach (var layer in layers)
        {
            if (layer.layerObject != null && layer.config != null)
            {
                // ������������ ����� ������� ���� � ������ �������� � ��������� �� �������
                Vector3 newPosition = layer.layerObject.transform.position + new Vector3(
                    cameraDelta.x * layer.config.speedX,
                    cameraDelta.y * layer.config.speedY,
                    0f
                );

                // ��������� ��������� ������
                layer.layerObject.transform.position = Vector3.Lerp(
                    layer.layerObject.transform.position,
                    newPosition,
                    layer.config.smoothness
                );
            }
        }

        // ��������� ���������� ������� ������ ��� ���������� �����
        previousCameraPosition = Camera.main.transform.position;
    }
}
