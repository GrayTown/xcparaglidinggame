using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Thermal : MonoBehaviour
{
    [SerializeField] private float liftStrength = 0f; // Начальная сила подъема термика
    [SerializeField] private float lifeTime = 10f; // Время жизни термика в секундах
    [SerializeField] private float maxLiftStrength = 2f; // Максимальная сила подъема
    [SerializeField] private float minLiftStrength = 0f; // Минимальная сила подъема
    [SerializeField] private float cloudOffsetY = 0f; // Высота облака над термиком

    // коробка термика
    [SerializeField] private Vector2 minSizeDelta = new Vector2(2f, 2f);
    [SerializeField] private Vector2 maxSizeDelta = new Vector2(2f, 2f);

    private float elapsedTime = 0f; // Время, прошедшее с начала жизни термика
    private bool isThermalActive = true;
    private bool inThermalNow = false;
    private ThermalResizer thermalResizer;
    private Cloud cloud; // Ссылка на облако

    public float LiftStrength => liftStrength;

    private void Start()
    {
        // Убедимся, что термик начинает с максимальной силы подъема
        liftStrength = maxLiftStrength;
        thermalResizer = GetComponent<ThermalResizer>();
        RandomThermalParameters();
        RandomThermalResize();
        cloud = GetComponentInChildren<Cloud>();
        if (cloud != null)
        {
            // Смещаем облако вверх (используем Vector2)
            cloud.transform.position = (Vector2)transform.position + new Vector2(0, transform.position.y + thermalResizer.size.y/2 - cloudOffsetY - 300);
            cloud.SetLifetime(lifeTime); // Передаем время жизни в облако перед стартом
        }
    }

    private void FixedUpdate()
    {
        UpdateLiftStrenght();
    }

    private void RandomThermalResize() 
    {
        if (thermalResizer != null)
        {
            float x = UnityEngine.Random.Range(thermalResizer.size.x / minSizeDelta.x, thermalResizer.size.x * maxSizeDelta.x);
            float y = UnityEngine.Random.Range(thermalResizer.size.y / minSizeDelta.y, thermalResizer.size.y * maxSizeDelta.y);
            thermalResizer.size = new Vector2(x, y); 
            thermalResizer.OnValidate();
            EventManager.Instance.Publish<Vector2>("ThermalSizeXY", thermalResizer.size);
        }
    }

    private void RandomThermalParameters() 
    {
        lifeTime = UnityEngine.Random.Range(10f, 50f); // Время жизни термика в секундах
        maxLiftStrength = UnityEngine.Random.Range(30f, 50f); ; // Максимальная сила подъема
        minLiftStrength = UnityEngine.Random.Range(0f, 30f); ; // Минимальная сила подъема 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Paraglider")
        {
            isThermalActive = true;
            inThermalNow = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Paraglider")
        {
            liftStrength = 0f;
            inThermalNow = false;
            EventManager.Instance.Publish<float>("ThermalLift", liftStrength);
            Debug.Log("Вышел из термика, сила = " + liftStrength);
        }
    }

    private void UpdateLiftStrenght() 
    {
        // Как только термик появился - время пошло!
        elapsedTime += Time.deltaTime;
        // Применяем распределение Гаусса для уменьшения силы подъема
        float normalizedTime = elapsedTime / lifeTime; // Нормализуем время от 0 до 1
        liftStrength = Mathf.Lerp(maxLiftStrength, minLiftStrength, Mathf.Pow(normalizedTime - 0.5f, 2f) * 4); // Используем Гауссово распределение
        if (isThermalActive)
        {
            if (inThermalNow)
            {
                EventManager.Instance.Publish<float>("ThermalLift", liftStrength);
                Debug.Log("Попал в термик!!!, сила = " + liftStrength);               
            }

            if (elapsedTime >= lifeTime)
            {
                liftStrength = 0f;
                isThermalActive = false;
                inThermalNow = false;
                EventManager.Instance.Publish<float>("ThermalDestroy", gameObject.transform.position.x);
                Debug.Log("Термик сдох, сила = " + liftStrength);
                Destroy(gameObject);
            }
        }
    }
}


