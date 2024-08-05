using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string name; // Unique name for each pool
        public GameObject prefab; // Prefab to pool
        public int size; // Number of objects in the pool
    }

    [SerializeField] private Pool[] pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.name, objectPool);
        }
    }

    public GameObject GetObject(string poolName)
    {
        if (poolDictionary.ContainsKey(poolName))
        {
            GameObject objectToReuse = poolDictionary[poolName].Dequeue();
            objectToReuse.SetActive(true);
            return objectToReuse;
        }

        Debug.LogWarning($"Pool with name {poolName} doesn't exist.");
        return null;
    }

    public void ReturnObject(string poolName, GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.ContainsKey(poolName))
        {
            poolDictionary[poolName].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Pool with name {poolName} doesn't exist.");
        }
    }
}
