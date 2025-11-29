using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    [Header("Configuracion de la Moneda")]
    public GameObject coinPrefab;
    public float coinRadius = 0.5f;

    [Header("Configuracion de Spawn")]
    public int minCoinsToSpawn = 5;
    public int maxCoinsToSpawn = 10;
    public Transform spawnReferencePoint;
    public float coinSpacing = 1.5f;

    [Header("Configuracion de Arco")]
    public float arcHeight = 2.0f;
    public float arcFrequency = 0.5f;

    [Header("Capas de Coleccionables")]
    public LayerMask obstacleLayer;
    public LayerMask coinLayer;

    public float powerUpRadius = 0.75f;

    // === NUEVOS PARÁMETROS DE POOLING ===
    [Header("Configuración de Pooling")]
    public int initialPoolSize = 30;
    private List<GameObject> coinPool;
    // ======================================

    void Start()
    {
        if (spawnReferencePoint == null) Debug.LogError("Spawn Reference Point no está asignado.");

        InitializeCoinPool();
    }

    // Inicializa la piscina (Pre-Warm)
    void InitializeCoinPool()
    {
        coinPool = new List<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            // Instanciar, desactivar y añadir a la piscina
            GameObject newCoin = Instantiate(coinPrefab);
            newCoin.SetActive(false);
            coinPool.Add(newCoin);
        }
    }

    // Función para obtener un objeto de la piscina o crear uno nuevo
    GameObject GetPooledCoin()
    {
        // reutilizar moneda
        foreach (GameObject coin in coinPool)
        {
            if (!coin.activeInHierarchy)
            {
                return coin;
            }
        }

        // 2. Si el pool está lleno, lo expandimos
        GameObject newCoin = Instantiate(coinPrefab);
        newCoin.SetActive(false); // Nace inactiva
        coinPool.Add(newCoin);
        return newCoin;
    }

    public void SpawnCoins()
    {
        int coinsToSpawn = Random.Range(minCoinsToSpawn, maxCoinsToSpawn + 1);
        int patternType = Random.Range(0, 2); // 0 = Linea, 1 = Arco

        for (int i = 0; i < coinsToSpawn; i++)
        {
            // Calcula la posición Y
            float yPos;
            if (patternType == 0)
            {
                yPos = spawnReferencePoint.position.y;
            }
            else
            {
                float yOffset = Mathf.Abs(Mathf.Sin(i * arcFrequency)) * arcHeight;
                yPos = spawnReferencePoint.position.y + yOffset;
            }

            float xPos = spawnReferencePoint.position.x + (i * coinSpacing);
            Vector3 spawnPosition = new Vector3(xPos, yPos, spawnReferencePoint.position.z);

            // === USAR POOLING EN LUGAR DE INSTANTIATE ===
            GameObject newCoin = GetPooledCoin();
            newCoin.transform.position = spawnPosition;
            newCoin.transform.rotation = Quaternion.identity;
            newCoin.SetActive(true); // La activamos para usarla

            // Verifica si la moneda colisiona con un obstaculo
            Collider2D obstacleCheck = Physics2D.OverlapCircle(newCoin.transform.position, coinRadius, obstacleLayer);

            if (obstacleCheck != null)
            {

                newCoin.SetActive(false);
            }
        }
    }

    public void ClearCoinsInArea(Vector3 position, float radius)
    {
        // overlap circle para encontrar monedas en el area
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, radius, coinLayer);

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("coin"))
            {
                hit.gameObject.SetActive(false);
            }
        }
    }
}