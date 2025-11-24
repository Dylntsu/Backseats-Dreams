using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Obstáculos")]
    public GameObject[] obstaclePrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 2.0f;
    public float startDelay = 1.0f;

    [Header("Potenciadores")]
    public GameObject[] powerUpPrefabs;
    [Range(0f, 1f)]
    public float powerUpSpawnChance = 0.2f;

    [Header("Configuración de PowerUp")]
    public float powerUpFixedY = -1.0f;
    public float powerUpSpawnRangeX = 8.0f;

    [Header("Referencias")]
    public CoinManager coinManager;

    // Alturas Y
    private float highObstacleY = -2.1f;
    private float lowObstacleY = -2.9f;
    private float sewerY = -3.55f;

    // --- VARIABLES DE POOLING ---
    private List<GameObject>[] obstaclePools;
    private List<GameObject>[] powerUpPools;

    void Start()
    {
        if (spawnPoint == null) Debug.LogError("SpawnPoint no está asignado.");

        InitializePools();
    }

    //listas vacias para cada tipo de prefab
    void InitializePools()
    {
        // obstaculos
        obstaclePools = new List<GameObject>[obstaclePrefabs.Length];
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            obstaclePools[i] = new List<GameObject>();
        }
        // potenciadores
        powerUpPools = new List<GameObject>[powerUpPrefabs.Length];
        for (int i = 0; i < powerUpPrefabs.Length; i++)
        {
            powerUpPools[i] = new List<GameObject>();
        }
    }

    // Función genérica para buscar/crear objetos en la piscina correcta
    GameObject GetPooledObject(List<GameObject>[] poolArray, GameObject[] prefabsArray, int index)
    {
        foreach (GameObject obj in poolArray[index])
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefabsArray[index]);
        newObj.SetActive(false); // Nace desactivado
        poolArray[index].Add(newObj);
        return newObj;
    }

    public void StartSpawning()
    {
        InvokeRepeating("SpawnObstacleAndTryPowerUp", startDelay, spawnInterval);
    }

    void SpawnObstacleAndTryPowerUp()
    {

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);

        //logica de obstaculos
        GameObject prefabReference = obstaclePrefabs[randomIndex];

        Vector3 spawnPosition = spawnPoint.position;

        if (prefabReference.name.Contains("alcantarilla"))
        {
            spawnPosition.y = sewerY;
        }
        else if (prefabReference.name.Contains("hObstacle"))
        {
            spawnPosition.y = highObstacleY;
        }
        else if (prefabReference.name.Contains("sObstacle"))
        {
            spawnPosition.y = lowObstacleY;
        }

        // pool de obstaculos
        GameObject obstacle = GetPooledObject(obstaclePools, obstaclePrefabs, randomIndex);
        obstacle.transform.position = spawnPosition;
        obstacle.transform.rotation = spawnPoint.rotation;
        obstacle.SetActive(true);

        //LOGICA DE MONEDAS
        if (coinManager != null)
        {
            coinManager.SpawnCoins();
        }

        // LOGICA DE POWER-UPS
        if (powerUpPrefabs.Length == 0) return;

        if (Random.Range(0f, 1f) <= powerUpSpawnChance)
        {
            SpawnPowerUp();
        }
    }

    void SpawnPowerUp()
    {
        if (coinManager == null) return;

        float baseSpawnX = spawnPoint.position.x;
        float minX = baseSpawnX + 2.0f;
        float maxX = baseSpawnX + powerUpSpawnRangeX;
        float randomX = Random.Range(minX, maxX);

        Vector3 finalSpawnPosition = new Vector3(randomX, powerUpFixedY, spawnPoint.position.z);

        //limpieza de monedas
        coinManager.ClearCoinsInArea(finalSpawnPosition, coinManager.powerUpRadius);

        //instanciar el Powerup usando Pooling
        SpawnPowerUpInstance(finalSpawnPosition);
    }

    void SpawnPowerUpInstance(Vector3 position)
    {
        int randomIndex = Random.Range(0, powerUpPrefabs.Length);

        GameObject powerUp = GetPooledObject(powerUpPools, powerUpPrefabs, randomIndex);
        powerUp.transform.position = position;
        powerUp.transform.rotation = Quaternion.identity;
        powerUp.SetActive(true);
        // se activa en la logica del powerup
    }
}