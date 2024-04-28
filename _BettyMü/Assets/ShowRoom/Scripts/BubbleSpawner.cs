using UnityEngine;
using System.Collections;


public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The object to spawn
    public Vector3 spawnAreaCenter; // Center of the spawn area
    public Vector3 spawnAreaSize; // Size of the spawn area
    public float spawnInterval = 4f; // Time between spawns

    void Start()
    {
        // Start spawning objects
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            // Calculate a random position within the spawn area
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
                Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2),
                Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
            );

            // Spawn the object at the calculated position
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

            // Wait for the specified interval before spawning the next object
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
