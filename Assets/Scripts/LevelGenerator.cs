using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Referensi")]
    public Transform player;
    public GameObject emptyChunk;
    public GameObject[] chunkPrefabs;

    [Header("Pengaturan")]
    public float chunkLength = 60f;
    public float startZ = -20f;
    public int chunksAhead = 4;
    public float despawnDistance = 40f;
    public int safeStartChunks = 2;

    readonly Queue<GameObject> activeChunks = new Queue<GameObject>();
    float nextSpawnZ;
    int spawnedCount;
    int lastIndex = -1;

    void Start()
    {
        nextSpawnZ = startZ;
    }

    void Update()
    {
        while (nextSpawnZ < player.position.z + chunksAhead * chunkLength)
            SpawnChunk();

        while (activeChunks.Count > 0
               && activeChunks.Peek().transform.position.z + chunkLength < player.position.z - despawnDistance)
            Destroy(activeChunks.Dequeue());
    }

    void SpawnChunk()
    {
        GameObject prefab;

        if (spawnedCount < safeStartChunks || chunkPrefabs.Length == 0)
        {
            prefab = emptyChunk;
        }
        else
        {
            int index = Random.Range(0, chunkPrefabs.Length);
            if (chunkPrefabs.Length > 1 && index == lastIndex)
                index = (index + 1) % chunkPrefabs.Length;

            lastIndex = index;
            prefab = chunkPrefabs[index];
        }

        GameObject chunk = Instantiate(prefab, new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity);
        activeChunks.Enqueue(chunk);

        nextSpawnZ += chunkLength;
        spawnedCount++;
    }
}