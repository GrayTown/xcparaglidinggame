using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThermalGenWithPool : MonoBehaviour
{
    public ThermalSettings thermalSettings;
    public Transform cloudBase;

    private float currentLiftForceUp = 0f;
    private float currentLiftForceDown = 0f;
    private float thermalLifeTimeTotal = 0;

    private static List<GameObject> allActiveThermals = new List<GameObject>();
    private List<GameObject> activeThermals = new List<GameObject>();
    private List<GameObject> activeClouds = new List<GameObject>();
    private int stackHeight = 250;

    // Используем пул объектов для термальных блоков
    private Queue<GameObject> thermalBlockPool = new Queue<GameObject>();
    private Queue<GameObject> cloudPool = new Queue<GameObject>();

    private void Start()
    {
        StartCoroutine(ThermalSpawnRoutine());
    }

    private IEnumerator ThermalSpawnRoutine()
    {
        float spawnInterval = Random.Range(thermalSettings.spawnIntervalMin, thermalSettings.spawnIntervalMax);
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (Random.value > thermalSettings.spawnChance)
                continue;

            float centerX = transform.position.x;
            float posXShift = Random.Range(thermalSettings.minXSpawn, thermalSettings.maxXSpawn);
            float spawnX = centerX + posXShift;

            // Пытаемся использовать пул объектов, чтобы не создавать новые
            if (IsObjectInRange(allActiveThermals, spawnX, thermalSettings.thermalSize.x * thermalSettings.spawnXDeltaGenPosCheck) ||
                IsObjectInRange(activeClouds, spawnX, thermalSettings.cloudSize.x * thermalSettings.spawnXDeltaGenPosCheck))
            {
                Debug.Log("Объекты наложились - пропуск генерации");
                continue;
            }

            GenerateThermal(spawnX);
        }
    }

    private bool IsObjectInRange(List<GameObject> objects, float x, float range)
    {
        // Для улучшения производительности можно сохранить объекты, которые близки к текущей позиции
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                float objX = obj.transform.position.x;
                if (Mathf.Abs(objX - x) < range)
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
        float currentY = baseHeight - thermalSettings.cloudSize.y;
        float phaseShift = Random.Range(0, Mathf.PI * 2);
        float direction = Random.value > 0.5f ? 1f : -1f;
        thermalLifeTimeTotal = Random.Range(thermalSettings.thermalLifeTimeMin, thermalSettings.thermalLifeTimeMax);

        // Используем пул облаков
        GameObject cloud = GetFromPool(cloudPool, thermalSettings.cloudPrefab, new Vector2(spawnX, baseHeight - thermalSettings.cloudTopShift));
        cloud.transform.localScale = thermalSettings.cloudSize;

        Animator cloudAnimator = cloud.GetComponent<Animator>();
        if (cloudAnimator != null)
        {
            AnimationClip clip = cloudAnimator.runtimeAnimatorController.animationClips[0];
            float duration = clip.length;
            cloudAnimator.speed = duration / thermalLifeTimeTotal;
        }
        activeClouds.Add(cloud);

        GameObject thermalContainer = new GameObject("Thermal");
        thermalContainer.transform.position = new Vector2(spawnX, currentY);
        thermalContainer.transform.parent = transform;

        thermalSettings.thermalDownPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(thermalSettings.liftForceDownMin, thermalSettings.liftForceDownMax);
        thermalSettings.thermalUpPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(thermalSettings.liftForceUpMin, thermalSettings.liftForceUpMax);
        currentLiftForceDown = thermalSettings.thermalDownPrefab.GetComponent<AreaEffector2D>().forceVariation;
        currentLiftForceUp = thermalSettings.thermalUpPrefab.GetComponent<AreaEffector2D>().forceVariation;

        stackHeight = Random.Range(thermalSettings.stackHeightMin, thermalSettings.stackHeightMax);

        // Используем пул термальных блоков
        for (int i = 0; i < stackHeight; i++)
        {
            float noise = Random.Range(-thermalSettings.noiseIntensity, thermalSettings.noiseIntensity);
            float xOffset = Mathf.Sin(i * thermalSettings.frequency + phaseShift) * thermalSettings.amplitude * direction + noise;
            float xPos = spawnX + xOffset;

            float thermalWidth = thermalSettings.thermalSize.x;
            float thermalHeight = thermalSettings.thermalSize.y;
            float downWidth = thermalWidth / 4f;
            float blockWidth = downWidth * 2 + thermalWidth;

            GameObject leftDown = GetFromPool(thermalBlockPool, thermalSettings.thermalDownPrefab, new Vector2(xPos - blockWidth / 2 + downWidth / 2, currentY));
            leftDown.transform.localScale = new Vector2(downWidth, thermalHeight);

            GameObject upZone = GetFromPool(thermalBlockPool, thermalSettings.thermalUpPrefab, new Vector2(xPos, currentY));
            upZone.transform.localScale = thermalSettings.thermalSize;

            GameObject rightDown = GetFromPool(thermalBlockPool, thermalSettings.thermalDownPrefab, new Vector2(xPos + blockWidth / 2 - downWidth / 2, currentY));
            rightDown.transform.localScale = new Vector2(downWidth, thermalHeight);

            GameObject block = new GameObject("ThermalBlock");
            block.transform.position = new Vector2(xPos, currentY);
            block.transform.parent = thermalContainer.transform;
            BoxCollider2D collider = block.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(blockWidth, thermalHeight);
            collider.isTrigger = true;

            currentY -= thermalHeight + thermalSettings.verticalSpacing;
        }

        activeThermals.Add(thermalContainer);
        allActiveThermals.Add(thermalContainer);
        StartCoroutine(DestroyThermalAfterTime(thermalContainer, cloud, thermalLifeTimeTotal));
    }

    private GameObject GetFromPool(Queue<GameObject> pool, GameObject prefab, Vector2 position)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    private IEnumerator DestroyThermalAfterTime(GameObject thermal, GameObject cloud, float lifetime)
    {
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
        ReturnToPool(cloudPool, cloud);
        ReturnToPool(thermalBlockPool, thermal);
    }

    private void ReturnToPool(Queue<GameObject> pool, GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, 30);
        foreach (var thermal in activeThermals)
        {
            if (thermal != null)
            {
                Gizmos.DrawWireSphere(thermal.transform.position, thermalSettings.thermalSize.x / 2);
            }
        }
    }
}

