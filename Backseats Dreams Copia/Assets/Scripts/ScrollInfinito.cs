using UnityEngine;

public class ScrollInfinito : MonoBehaviour
{
    [Header("Configuración de Parallax")]
    public float parallaxMultiplier = 1f; 

    [Header("Referencias")]
    private float anchoSprite;
    private GameManager gameManager; 

    void Start()
    {
        //ancho sprite
        anchoSprite = GetComponent<SpriteRenderer>().bounds.size.x;

        gameManager = Object.FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("ScrollInfinito no pudo encontrar el GameManager!");
        }
    }

    void Update()
    {
        if (gameManager == null) return;
        // Obtiene la velocidad global del juego
        float globalSpeed = gameManager.GetCurrentSpeed();
        // Calcula su propia velocidad basada en el multiplicador
        float mySpeed = globalSpeed * parallaxMultiplier;
        // Mueve el sprite hacia la izquierda
        transform.Translate(Vector3.left * mySpeed * Time.deltaTime);
        // Si se salió completamente de la pantalla, lo reposiciona a la derecha
        if (transform.position.x < -anchoSprite)
        {
            transform.position += new Vector3(anchoSprite * 2, 0, 0);
        }
    }
}