using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
        public Transform defaultParent; // Опциональный родитель по умолчанию
    }

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Создаем пулы для всех объектов
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.defaultParent);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectQueue);
        }
    }

    // Метод получения объекта из пула
    public GameObject GetFromPool(string tag, Vector2 position, Quaternion rotation, Transform customParent = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Пул с тегом {tag} не найден!");
            return null;
        }

        GameObject pooledObject;

        if (poolDictionary[tag].Count == 0)
        {
            Debug.Log($"Пул {tag} пуст, создаем новый объект.");
            Pool pool = pools.Find(p => p.tag == tag);
            pooledObject = Instantiate(pool.prefab, customParent ? customParent : pool.defaultParent);
        }
        else
        {
            pooledObject = poolDictionary[tag].Dequeue();
            pooledObject.transform.SetParent(customParent ? customParent : pools.Find(p => p.tag == tag).defaultParent);
        }

        pooledObject.SetActive(true);
        pooledObject.transform.SetPositionAndRotation(position, rotation);
        return pooledObject;
    }

    // Метод возврата объекта в пул
    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Пул с тегом {tag} не найден!");
            Destroy(obj); // Если объекта нет в пуле, уничтожаем
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}
