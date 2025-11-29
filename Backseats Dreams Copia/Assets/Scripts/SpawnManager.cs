using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Pooling Settings")]
    public int initialPoolSize = 5;

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

    // === MODIFICACIÓN CLAVE: ORDEN DE INICIALIZACIÓN Y PRE-CALENTAMIENTO ===
    void InitializePools()
    {
        // 1. Inicializar los arrays contenedores (crea el espacio)
        obstaclePools = new List<GameObject>[obstaclePrefabs.Length];
        powerUpPools = new List<GameObject>[powerUpPrefabs.Length];

        // 2. Inicializar las listas DENTRO del array (crea las listas)
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            obstaclePools[i] = new List<GameObject>();
        }
        for (int i = 0; i < powerUpPrefabs.Length; i++)
        {
            powerUpPools[i] = new List<GameObject>();
        }

        // 3. Pre-calentar las piscinas (Ahora las listas ya existen y no son NULL)
        PreWarmPools();
    }

    void PreWarmPools()
    {
        // Pre-calentar Obstáculos
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            for (int j = 0; j < initialPoolSize; j++)
            {
                GetPooledObject(obstaclePools, obstaclePrefabs, i);
            }
        }

        // Pre-calentar Potenciadores 
        for (int i = 0; i < powerUpPrefabs.Length; i++)
        {
            for (int j = 0; j < initialPoolSize; j++)
            {
                GetPooledObject(powerUpPools, powerUpPrefabs, i);
            }
        }
    }
    // =========================================================================

    // Función genérica para buscar/crear objetos en la piscina correcta
    GameObject GetPooledObject(List<GameObject>[] poolArray, GameObject[] prefabsArray, int index)
    {
        // FIX CRÍTICO 1: Si el prefab está vacío en el Inspector, fallamos con un error claro
        if (prefabsArray[index] == null)
        {
            Debug.LogError("ERROR DE POOLING: El prefab en el índice " + index + " está vacío ('None'). Revisa el Inspector.");
            return null;
        }

        // FIX CRÍTICO 2: Asegurar que la lista exista (aunque InitializePools debería hacerlo)
        if (poolArray[index] == null)
        {
            poolArray[index] = new List<GameObject>();
        }

        // Buscar un objeto inactivo
        foreach (GameObject obj in poolArray[index])
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Si el pool se expande: Instanciar y añadir
        GameObject newObj = Instantiate(prefabsArray[index]);
        newObj.SetActive(false);
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

        // Si el objeto devuelto es nulo (porque el prefab estaba vacío), salimos.
        if (obstaclePrefabs.Length == 0 || obstaclePrefabs[randomIndex] == null) return;

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

        // CRÍTICO: Si GetPooledObject falló por NULL, no intentamos usar el objeto
        if (obstacle == null) return;

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
        if (powerUpPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, powerUpPrefabs.Length);

        // CRÍTICO: Si GetPooledObject falló por NULL, no intentamos usar el objeto
        GameObject powerUp = GetPooledObject(powerUpPools, powerUpPrefabs, randomIndex);
        if (powerUp == null) return;

        powerUp.transform.position = position;
        powerUp.transform.rotation = Quaternion.identity;
        powerUp.SetActive(true);
        // se activa en la logica del powerup
    }
}