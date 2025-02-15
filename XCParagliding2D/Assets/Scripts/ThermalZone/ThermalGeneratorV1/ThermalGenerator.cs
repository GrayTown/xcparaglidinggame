using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThermalGenerator : MonoBehaviour
{
    [Header("Thermal parts prefabs")]
    public GameObject cloudPrefab;
    public GameObject thermalUpPrefab;
    public GameObject thermalDownPrefab;

    [Header("Cloud & cloud base parameters")]
    public Transform cloudBase;
    public Vector2 cloudSize = new Vector2(80f, 30f);
    public float cloudTopShift = 15f;

    [Header("Thermal stack settings")]
    public int stackHeightMin = 150;
    public int stackHeightMax = 250;
    public float verticalSpacing = 0f;
    public Vector2 thermalSize = new Vector2(50f, 2f);
    public float deltaSpacing = 0.5f;

    [Header("Thermal noize settings")]
    public float xOffsetRange = 10f;
    public float frequency = 0.02f; // Уменьшаем частоту для удлинения синусоиды
    public float amplitude = 20.0f; // Увеличиваем амплитуду для большего радиуса смещения
    public float noiseIntensity = 20.0f; // Интенсивность шума

    [Header("Thermal lift settings")]
    public float liftForceUpMin = 50f;
    public float liftForceUpMax = 79f;
    public float liftForceDownMin = 80f;
    public float liftForceDownMax = 100f;
    public float currentLiftForceUp = 0f;
    public float currentLiftForceDown = 0f;

    [Header("Thermal spawn parameters")]
    public float minXSpawn = -100f;
    public float maxXSpawn = 100f;
    public float spawnXDeltaGenPosCheck = 3f; // для увеличения расстояния проверки возможности генерации термика рядом с другим термиком
    public float spawnInterval = 10f;
    public float spawnIntervalMin = 10f;
    public float spawnIntervalMax = 30f;
    public float spawnChance = 0.7f;
    public float thermalLifeTimeMin = 10f;
    public float thermalLifeTimeMax = 60f;
    public float thermalLifeTimeTotal = 0;

    // Глобальный - Общий список всех термиков в сцене
    private static List<GameObject> allActiveThermals = new List<GameObject>();

    private List<GameObject> activeThermals = new List<GameObject>();
    private List<GameObject> activeClouds = new List<GameObject>();
    private int stackHeight = 250;

    private void Start()
    {
        StartCoroutine(ThermalSpawnRoutine());
    }

    private IEnumerator ThermalSpawnRoutine()
    {
        spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (Random.value > spawnChance)
                continue;

            float centerX = transform.position.x;
            float posXShift = Random.Range(minXSpawn, maxXSpawn);
            float spawnX = centerX + posXShift; // Смещение от центра

            if (IsObjectInRange(allActiveThermals, spawnX, thermalSize.x * spawnXDeltaGenPosCheck) || IsObjectInRange(activeClouds, spawnX, cloudSize.x * spawnXDeltaGenPosCheck))
            {
                Debug.Log("Объекты наложились - пропуск генерации");
                continue;
            }

            GenerateThermal(spawnX);
        }
    }


    private bool IsObjectInRange(List<GameObject> objects, float x, float range)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                float objX = obj.transform.position.x;
                if (Mathf.Abs(objX - x) < range) // Проверка наложения
                    return true;
            }
        }
        return false;
    }

    private float GetDampedLiftForce(float timeElapsed, float maxForce)
    {
        float lambda = Mathf.Log(2) / thermalLifeTimeTotal;
        return maxForce * Mathf.Exp(-lambda * timeElapsed);
    }

    private void GenerateThermal(float spawnX)
    {
        float baseHeight = cloudBase.position.y;
        float currentY = baseHeight - cloudSize.y;
        float phaseShift = Random.Range(0, Mathf.PI * 2); // Случайный старт синусоиды
        float direction = Random.value > 0.5f ? 1f : -1f; // Случайное направление влево или вправо
        thermalLifeTimeTotal = Random.Range(thermalLifeTimeMin, thermalLifeTimeMax);

        // Создаём облако
        GameObject cloud = Instantiate(cloudPrefab, new Vector2(spawnX, baseHeight - cloudTopShift), Quaternion.identity, transform);
        cloud.transform.localScale = cloudSize;

        Animator cloudAnimator = cloud.GetComponent<Animator>();
        AnimationClip clip = cloudAnimator.runtimeAnimatorController.animationClips[0]; // Получаем первый клип (если их несколько)
        float duration = clip.length; // Длительность анимации в секундах
        
        if (cloudAnimator != null)
        {
            cloudAnimator.speed = duration / thermalLifeTimeTotal; // Анимация будет проигрываться за время жизни термика
        }
        activeClouds.Add(cloud);

        GameObject thermalContainer = new GameObject("Thermal");
        thermalContainer.transform.position = new Vector2(spawnX, currentY);
        thermalContainer.transform.parent = transform;

        // Сила подъема и спуска
        thermalDownPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(liftForceDownMin, liftForceDownMax);
        thermalUpPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(liftForceUpMin, liftForceUpMax);
        currentLiftForceDown = thermalDownPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(liftForceDownMin, liftForceDownMax);
        currentLiftForceUp = thermalUpPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(liftForceUpMin, liftForceUpMax);

        stackHeight = Random.Range(stackHeightMin,stackHeightMax);

        for (int i = 0; i < stackHeight; i++)
        {
            float noise = Random.Range(-noiseIntensity, noiseIntensity);
            float xOffset = Mathf.Sin(i * frequency + phaseShift) * amplitude * direction + noise;
            float xPos = spawnX + xOffset;

            float thermalWidth = thermalSize.x;
            float thermalHeight = thermalSize.y;
            float downWidth = thermalWidth / 4f;
            float blockWidth = downWidth * 2 + thermalWidth;

            GameObject leftDown = Instantiate(thermalDownPrefab, new Vector2(xPos - blockWidth / 2 + downWidth / 2, currentY), Quaternion.identity, thermalContainer.transform);
            leftDown.transform.localScale = new Vector2(downWidth, thermalHeight);

            GameObject upZone = Instantiate(thermalUpPrefab, new Vector2(xPos, currentY), Quaternion.identity, thermalContainer.transform);
            upZone.transform.localScale = thermalSize;

            GameObject rightDown = Instantiate(thermalDownPrefab, new Vector2(xPos + blockWidth / 2 - downWidth / 2, currentY), Quaternion.identity, thermalContainer.transform);
            rightDown.transform.localScale = new Vector2(downWidth, thermalHeight);

            GameObject block = new GameObject("ThermalBlock");
            block.transform.position = new Vector2(xPos, currentY);
            block.transform.parent = thermalContainer.transform;
            BoxCollider2D collider = block.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(blockWidth, thermalHeight);
            collider.isTrigger = true;

            currentY -= thermalHeight + verticalSpacing;
        }

        activeThermals.Add(thermalContainer);
        allActiveThermals.Add(thermalContainer);
        StartCoroutine(DestroyThermalAfterTime(thermalContainer, cloud, thermalLifeTimeTotal));
    }

    private IEnumerator DestroyThermalAfterTime(GameObject thermal, GameObject cloud, float lifetime)
    {
        //yield return new WaitForSeconds(lifetime);
        float elapsedTime = 0;
        AreaEffector2D[] effectors = thermal.GetComponentsInChildren<AreaEffector2D>();

        while (elapsedTime < lifetime)
        {
            elapsedTime += Time.deltaTime;
            foreach (var effector in effectors)
            {
                if (currentLiftForceDown > currentLiftForceUp)
                {
                    effector.forceVariation = GetDampedLiftForce(elapsedTime, currentLiftForceUp);
                }
                else 
                {
                    effector.forceVariation = GetDampedLiftForce(elapsedTime, currentLiftForceDown);
                }
            }
            yield return null;
        }
        allActiveThermals.Remove(thermal);
        activeThermals.Remove(thermal);
        activeClouds.Remove(cloud);
        if (thermal != null) Destroy(thermal);
        if (cloud != null) Destroy(cloud);
    }

    // Добавляем Gizmos для отрисовки термиков в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;  // Цвет Gizmos для термиков

        Gizmos.DrawWireSphere(gameObject.transform.position, 30);

        foreach (var thermal in activeThermals)
        {
            if (thermal != null)
            {
                Gizmos.DrawWireSphere(thermal.transform.position, thermalSize.x / 2); // Отображаем сферу для термика
            }
        }
    }
}