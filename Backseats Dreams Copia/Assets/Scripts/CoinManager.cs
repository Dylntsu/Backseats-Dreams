using UnityEngine;

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

    [Header("capas de coleccionables")]
    public LayerMask obstacleLayer; 
    public LayerMask coinLayer;

    public float powerUpRadius = 0.75f;

    public void SpawnCoins()
    {
        int coinsToSpawn = Random.Range(minCoinsToSpawn, maxCoinsToSpawn + 1);

        int patternType = Random.Range(0, 2); // 0 = Linea, 1 = Arco

        for (int i = 0; i < coinsToSpawn; i++)
        {
            //calcula la posicion X
            float xPos = spawnReferencePoint.position.x + (i * coinSpacing);
            float yPos;

            // decide el patron de spawn
            if (patternType == 0)
            {
                yPos = spawnReferencePoint.position.y;
            }
            else
            {
                // patron de arco
                float yOffset = Mathf.Abs(Mathf.Sin(i * arcFrequency)) * arcHeight;
                yPos = spawnReferencePoint.position.y + yOffset;
            }

            Vector3 spawnPosition = new Vector3(xPos, yPos, spawnReferencePoint.position.z);

            GameObject newCoin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

            // Verifica si la moneda colisiona con un obstaculo
            Collider2D obstacleCheck = Physics2D.OverlapCircle(newCoin.transform.position, coinRadius, obstacleLayer);

            if (obstacleCheck != null)
            {

                Destroy(newCoin);
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
                Destroy(hit.gameObject);
            }
        }
    }
}