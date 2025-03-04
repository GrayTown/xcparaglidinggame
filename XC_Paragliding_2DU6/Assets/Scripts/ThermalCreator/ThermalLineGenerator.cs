using UnityEngine;
using Unity;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class VerticalLine : MonoBehaviour
{
    [Header("Границы термика")]
    public Transform parentObject; // Основание линии
    public Transform cloudObject;  // Вершина линии

    [Header("Точки перегиба, угол наклона, толщина")]
    public int pointCount = 2; // Количество точек линии
    public float angle = 0f; // Угол наклона линии в градусах
    public float colliderThinkness = 0.5f; // Отступ от линии для коллайдера

    // Параметры колебаний
    [Header("Настройки колебаний")]
    public bool useSinWave = true; // Использовать синусоидальные колебания
    public float frequency = 1f; // Частота колебаний
    public float amplitude = 1f; // Амплитуда колебаний
    public float speed = 1f; // Скорость колебаний
    public float smoothness = 0.1f; // Плавность колебаний
    public float transitionSpeed = 0.5f; // Скорость перехода между синусом и шумом

    [Header("Подъемная сила термика")]
    public float addLiftForce = 1f; // Базовая подъемная сила
    public int zoneCountX = 6; // Количество зон по X
    public float[] zoneForcesX; // Массив сил для зон
    public float liftForceY = 4f; // Подъемная сила по Y
    public float minLiftForceY = 0.5f; // Минимальная сила перед исчезновением
    public float currentLiftForce = 0f;

    [Header("Время жизни термика")]
    public float thermalLifetime = 30f; // Время жизни термика (сек)
    public float elapsedTime = 0f; // Счетчик времени жизни

    
    private LineRenderer lineRenderer;
    private PolygonCollider2D polyCollider;
    private float time;
    private float transitionValue;   
    private bool toDestroy = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCount;
        lineRenderer.useWorldSpace = true;
        transitionValue = useSinWave ? 0f : 1f;
        polyCollider = gameObject.GetComponent<PolygonCollider2D>();
        polyCollider.isTrigger = true;
        // Гарантируем, что массив сил соответствует количеству зон
        if (zoneForcesX == null || zoneForcesX.Length != zoneCountX)
        {
            zoneForcesX = new float[zoneCountX];

            // Заполним массив значениями по умолчанию
            for (int i = 0; i < zoneCountX; i++)
            {
                if (i == 0 || i == zoneCountX - 1) zoneForcesX[i] = -1f; // Края
                else if (i == 1 || i == zoneCountX - 2) zoneForcesX[i] = 0f; // Почти края
                else zoneForcesX[i] = 1f; // Центр
            }
        }
    }

    private void Update()
    {
        if (!toDestroy && this.isActiveAndEnabled)
        {
            if (parentObject != null && cloudObject != null)
            {
                time += Time.deltaTime * speed;

                // Плавный переход между синусом и шумом
                transitionValue = Mathf.Lerp(transitionValue, useSinWave ? 0f : 1f, transitionSpeed * Time.deltaTime);

                GenerateLine();
                GenerateEdgeCollider();
            }
            // Отслеживание времени жизни
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= thermalLifetime)
            {
                toDestroy = true; // Удаляем термик по истечении времени
                ThermalPool.Instance.ReturnThermal(this); // Возвращаем в пул
            }
        }
    }

    private void GenerateLine()
    {
        if (!toDestroy && isActiveAndEnabled)
        {
            if (pointCount <= 0)
            {
                Debug.LogWarning("pointCount меньше или равно 0! Установка в 2.");
                pointCount = 2; // Минимальное допустимое значение
            }
            if (lineRenderer.positionCount != pointCount)
            {
                lineRenderer.positionCount = pointCount;
            }

            float angleRad = angle * Mathf.Deg2Rad;
            for (int i = 0; i < pointCount; i++)
            {
                float t = (float)i / (pointCount - 1);
                float baseX = parentObject.position.x;
                float baseY = Mathf.Lerp(parentObject.position.y, cloudObject.position.y, t);
                float offsetX = Mathf.Sin(angleRad) * (baseY - parentObject.position.y);

                // Вычисляем колебания с плавностью
                float wave = transitionValue < 0.5f
                    ? Mathf.Sin(time * frequency + i) * amplitude
                    : Mathf.PerlinNoise(time * frequency + i, 0f) * amplitude * 2f - amplitude;
                wave = Mathf.Lerp(wave, wave * 0.5f, smoothness);
                float x = baseX + offsetX + wave;
                // Проверяем, не превышает ли i допустимые границы
                if (i >= lineRenderer.positionCount)
                {
                    Debug.LogError($"Ошибка: i ({i}) выходит за пределы positionCount ({lineRenderer.positionCount})");
                    return;
                }
                lineRenderer.SetPosition(i, new Vector3(x, baseY, parentObject.position.z));
            }
        }
    }

    private void GenerateEdgeCollider()
    {
        if (!toDestroy && isActiveAndEnabled)
        {
            List<Vector2> colliderPoints = new List<Vector2>();

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 worldPoint = lineRenderer.GetPosition(i);
                Vector2 localPoint = transform.InverseTransformPoint(worldPoint); // Преобразуем в локальные координаты

                if (i == 0)
                {
                    Vector2 colliderPoint = new Vector2(localPoint.x + colliderThinkness, localPoint.y);
                    colliderPoints.Add(colliderPoint);
                }

                if (i < lineRenderer.positionCount)
                {
                    Vector2 colliderPoint = new Vector2(localPoint.x - colliderThinkness, localPoint.y);
                    colliderPoints.Add(colliderPoint);
                }

                if (i == lineRenderer.positionCount - 1)
                {
                    Vector2 colliderPoint = new Vector2(localPoint.x + colliderThinkness, localPoint.y);
                    colliderPoints.Add(colliderPoint);

                    for (int j = i; j >= 0; j--)
                    {
                        Vector3 reverseWorldPoint = lineRenderer.GetPosition(j);
                        Vector2 reverseLocalPoint = transform.InverseTransformPoint(reverseWorldPoint);
                        Vector2 point = new Vector2(reverseLocalPoint.x + colliderThinkness, reverseLocalPoint.y);
                        colliderPoints.Add(point);
                    }
                }
            }
            polyCollider.SetPath(0, colliderPoints);
        }
    }

    private float CalculateLiftForce(Vector2 otherPosition)
    {
        if (!toDestroy && isActiveAndEnabled)
        {
            if (polyCollider == null || polyCollider.pathCount == 0) return 0f;

            // Получаем границы термика
            Vector2[] outerPath = polyCollider.GetPath(0);
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (Vector2 point in outerPath)
            {
                Vector2 worldPoint = transform.TransformPoint(point);
                minX = Mathf.Min(minX, worldPoint.x);
                maxX = Mathf.Max(maxX, worldPoint.x);
                minY = Mathf.Min(minY, worldPoint.y);
                maxY = Mathf.Max(maxY, worldPoint.y);
            }

            // Если объект за пределами термика, возвращаем 0
            if (otherPosition.x < minX || otherPosition.x > maxX || otherPosition.y < minY || otherPosition.y > maxY)
                return 0f;

            // Определяем ширину термика
            float width = maxX - minX;
            float normalizedX = Mathf.InverseLerp(minX, maxX, otherPosition.x);

            // Определяем текущую зону
            int zoneIndex = Mathf.FloorToInt(normalizedX * zoneCountX);
            zoneIndex = Mathf.Clamp(zoneIndex, 0, zoneCountX - 1);

            // Получаем силу из массива
            float forceX = zoneForcesX[zoneIndex];

            // Уменьшаем подъемную силу по Y в зависимости от оставшегося времени
            float lifeFactor = Mathf.Clamp01(1f - (elapsedTime / thermalLifetime));
            float forceY = Mathf.Lerp(minLiftForceY, liftForceY, lifeFactor);

            return addLiftForce * forceX * forceY;
        }
        return 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Paraglider>(out Paraglider paraglider))
        {
            float liftForce = CalculateLiftForce(other.transform.position);
            currentLiftForce = liftForce;
            paraglider._currentVerticalSpeed = liftForce;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<Paraglider>(out Paraglider paraglider))
        {
            float liftForce = CalculateLiftForce(other.transform.position);
            currentLiftForce = liftForce;
            paraglider._currentVerticalSpeed = liftForce;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Paraglider>(out Paraglider paraglider))
        {
            paraglider._currentVerticalSpeed = 0;
        }
    }

    public void ResetThermal()
    {
        elapsedTime = 0f;
        toDestroy = false;
        currentLiftForce = 0f;
    }

    private void OnDrawGizmos()
    {
        if (polyCollider != null && polyCollider.pathCount > 0)
        {
            Gizmos.color = Color.red;

            Vector2[] points = polyCollider.GetPath(0);
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 worldPoint1 = transform.TransformPoint(points[i]);
                Vector2 worldPoint2 = transform.TransformPoint(points[(i + 1) % points.Length]);
                Gizmos.DrawLine(worldPoint1, worldPoint2);
            }
        }
    }
}
