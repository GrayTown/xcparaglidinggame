using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThermalGeneratorWithPool : MonoBehaviour
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
    private int thermalCounter = 0;

    public Dictionary<int, GameObject> thermalDictionary = new Dictionary<int, GameObject>();

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

            if (IsObjectInRange(allActiveThermals, spawnX, thermalSettings.thermalSize.x * thermalSettings.spawnXDeltaGenPosCheck) || IsObjectInRange(activeClouds, spawnX, thermalSettings.cloudSize.x * thermalSettings.spawnXDeltaGenPosCheck))
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
        int thermalIndex = thermalCounter++;

        float baseHeight = cloudBase.position.y;
        float currentY = baseHeight - thermalSettings.cloudSize.y;
        float phaseShift = Random.Range(0, Mathf.PI * 2);
        float direction = Random.value > 0.5f ? 1f : -1f;
        thermalLifeTimeTotal = Random.Range(thermalSettings.thermalLifeTimeMin, thermalSettings.thermalLifeTimeMax);

        //GameObject cloud = Instantiate(thermalSettings.cloudPrefab, new Vector2(spawnX, baseHeight - thermalSettings.cloudTopShift), Quaternion.identity, transform);
        //cloud.transform.localScale = thermalSettings.cloudSize;
        GameObject cloud = ObjectPoolManager.Instance.GetFromPool("Cloud", new Vector2(spawnX, baseHeight - thermalSettings.cloudTopShift),Quaternion.identity,transform);
        if (cloud != null) 
        {
            cloud.transform.localScale = thermalSettings.cloudSize;
        }

        Animator cloudAnimator = cloud.GetComponent<Animator>();
        if (cloudAnimator != null)
        {
            AnimationClip clip = cloudAnimator.runtimeAnimatorController.animationClips[0];
            float duration = clip.length;
            cloudAnimator.speed = duration / thermalLifeTimeTotal;
        }
        activeClouds.Add(cloud);

        GameObject thermalContainer = new GameObject("Thermal_" + thermalIndex + "_" + gameObject.name);
        thermalContainer.transform.position = new Vector2(spawnX, currentY);
        thermalContainer.transform.parent = transform;
        //GameObject thermalContainer = ObjectPoolManager.Instance.GetFromPool("Thermal", new Vector2(spawnX, currentY), Quaternion.identity, transform);

        thermalSettings.thermalDownPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(thermalSettings.liftForceDownMin, thermalSettings.liftForceDownMax);
        thermalSettings.thermalUpPrefab.GetComponent<AreaEffector2D>().forceVariation = Random.Range(thermalSettings.liftForceUpMin, thermalSettings.liftForceUpMax);
        currentLiftForceDown = thermalSettings.thermalDownPrefab.GetComponent<AreaEffector2D>().forceVariation;
        currentLiftForceUp = thermalSettings.thermalUpPrefab.GetComponent<AreaEffector2D>().forceVariation;
        stackHeight = Random.Range(thermalSettings.stackHeightMin, thermalSettings.stackHeightMax);

        for (int i = 0; i < stackHeight; i++)
        {
            float noise = Random.Range(-thermalSettings.noiseIntensity, thermalSettings.noiseIntensity);
            float xOffset = Mathf.Sin(i * thermalSettings.frequency + phaseShift) * thermalSettings.amplitude * direction + noise;
            float xPos = spawnX + xOffset;

            float thermalWidth = thermalSettings.thermalSize.x;
            float thermalHeight = thermalSettings.thermalSize.y;
            float downWidth = thermalWidth / 4f;
            float blockWidth = downWidth * 2 + thermalWidth;

            //GameObject leftDown = Instantiate(thermalSettings.thermalDownPrefab, new Vector2(xPos - blockWidth / 2 + downWidth / 2, currentY), Quaternion.identity, thermalContainer.transform);
            GameObject leftDown = ObjectPoolManager.Instance.GetFromPool("ThermalDown", new Vector2(xPos - blockWidth / 2 + downWidth / 2, currentY), Quaternion.identity, thermalContainer.transform);
            leftDown.transform.localScale = new Vector2(downWidth, thermalHeight);

            //GameObject upZone = Instantiate(thermalSettings.thermalUpPrefab, new Vector2(xPos, currentY), Quaternion.identity, thermalContainer.transform);
            GameObject upZone = ObjectPoolManager.Instance.GetFromPool("ThermalUp", new Vector2(xPos, currentY), Quaternion.identity, thermalContainer.transform);
            upZone.transform.localScale = thermalSettings.thermalSize;

            //GameObject rightDown = Instantiate(thermalSettings.thermalDownPrefab, new Vector2(xPos + blockWidth / 2 - downWidth / 2, currentY), Quaternion.identity, thermalContainer.transform);
            GameObject rightDown = ObjectPoolManager.Instance.GetFromPool("ThermalDown", new Vector2(xPos + blockWidth / 2 - downWidth / 2, currentY), Quaternion.identity, thermalContainer.transform);
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
        thermalDictionary.Add(thermalIndex, thermalContainer);
        StartCoroutine(DestroyThermalAfterTime(thermalContainer, cloud, thermalLifeTimeTotal));
    }

    private IEnumerator DestroyThermalAfterTime(GameObject thermal, GameObject cloud, float lifetime)
    {
        float elapsedTime = 0;
        AreaEffector2D[] effectors = thermal.GetComponentsInChildren<AreaEffector2D>();

        while (elapsedTime < lifetime)
        {
            elapsedTime += Time.deltaTime;
            //EventManager.Instance.Publish("RiseUp", currentLiftForceUp);
            //EventManager.Instance.Publish("RiseDown", currentLiftForceDown);
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

        if (thermal != null) 
        {
            ObjectPoolManager.Instance.ReturnToPool("Thermal", thermal);
            //Destroy(thermal); 
        }
        if (cloud != null) 
        {
            ObjectPoolManager.Instance.ReturnToPool("Cloud", cloud);
            //Destroy(cloud); 
        }
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

