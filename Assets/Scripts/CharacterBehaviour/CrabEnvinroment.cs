using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabEnvinroment : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform lairHolder;
    [SerializeField] Transform objectsSpawnHolder;

    [SerializeField] Transform randomPositionHolder;

    [SerializeField] Transform pooledHolder;
    [SerializeField] Transform readyHolder;

    Transform[] randomPositions;
    Transform[] objectsSpawnPositions;
    Transform[] lairs;

    List<Spawnable> pooledObjects;
    List<Spawnable> readyObjects;

    [Header("Settings")]
    [SerializeField] [Range(0, 60)] float spawnTime;
    float spawnElapsedTime = 0;

    public bool objectReady { get { return readyObjects.Count != 0; } }

    private void Awake()
    {
        pooledObjects = new List<Spawnable>(pooledHolder.GetComponentsInChildren<Spawnable>());
        readyObjects = new List<Spawnable>();

        var spawnList = new List<Transform>(objectsSpawnHolder.GetComponentsInChildren<Transform>());
        spawnList.Remove(objectsSpawnHolder);
        objectsSpawnPositions = spawnList.ToArray();

        spawnList = new List<Transform>(lairHolder.GetComponentsInChildren<Transform>());
        spawnList.Remove(lairHolder);
        lairs = spawnList.ToArray();

        spawnList = new List<Transform>(randomPositionHolder.GetComponentsInChildren<Transform>());
        spawnList.Remove(randomPositionHolder);
        randomPositions = spawnList.ToArray();
    }

    public bool IsSpawnableReady(Spawnable spawnable)
    {
        return readyObjects.Contains(spawnable);
    }

    private void Update()
    {
        SpawnUpdate();
    }

    void SpawnUpdate()
    {
        spawnElapsedTime += Time.deltaTime;

        if (spawnElapsedTime > spawnTime)
        {
            SpawnObject();
            spawnElapsedTime = 0;
        }
    }

    void SpawnObject()
    {
        if (pooledObjects.Count == 0) return;

        int index = Random.Range(0, pooledObjects.Count);

        var spawned = pooledObjects[index];
        pooledObjects.RemoveAt(index);

        readyObjects.Add(spawned);
        spawned.transform.parent = readyHolder;


        spawned.Spawn(objectsSpawnPositions[Random.Range(0, objectsSpawnPositions.Length)].position);
    }


    public void RetrieveObject(Spawnable retrievedObject)
    {
        if (readyObjects.Contains(retrievedObject))
        {
            readyObjects.Remove(retrievedObject);
        }
    }

    public void AddObject(Spawnable newObject)
    {
        pooledObjects.Add(newObject);
        newObject.transform.parent = pooledHolder;
    }

    public Spawnable GetRandomObject()
    {
        return readyObjects[Random.Range(0, readyObjects.Count)];
    }

    public Transform GetRandomLair()
    {
        return lairs[Random.Range(0, lairs.Length)];
    }

    public Vector3 GetRandomPosition()
    {
        return randomPositions[Random.Range(0, randomPositions.Length)].position;
    }

}
