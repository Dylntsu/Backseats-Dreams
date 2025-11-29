using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public ParticleSystem collectParticles;
    public AudioClip collectSound;
    public float magnetSpeed = 15f;
    public float magnetRange = 7f;

    // --- REFERENCIAS CACHEADAS ---
    private GameManager gameManager;
    private playerController playerController;
    private SpriteRenderer spriteRenderer;
    private Collider2D coinCollider;
    private PoolReturn poolReturn; // El script que devuelve la moneda al Pool

    private bool isCollected = false;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coinCollider = GetComponent<Collider2D>();
        poolReturn = GetComponent<PoolReturn>();

        gameManager = FindFirstObjectByType<GameManager>();
        playerController = FindFirstObjectByType<playerController>();
    }

    private void OnEnable()
    {
        // Reiniciamos el estado cada vez que la moneda sale del Pool
        isCollected = false;
        // Restauramos los componentes que se desactivan al recogerla
        spriteRenderer.enabled = true;
        coinCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        if (gameManager != null)
        {
            gameManager.AddCoin(coinValue);
        }

        spriteRenderer.enabled = false;
        coinCollider.enabled = false;

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        if (collectParticles != null)
        {
            collectParticles.Play();
        }

        if (poolReturn != null)
        {
            poolReturn.ReturnToPoolAfterCollision();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // La lógica de magnetismo no genera GC y es eficiente.
        if (isCollected) return;

        if (playerController != null && playerController.isMagnetActive)
        {
            //calcula la distancia al jugador
            float distanceToPlayer = Vector2.Distance(transform.position, playerController.transform.position);

            if (distanceToPlayer <= magnetRange)
            {
                //mueve la moneda hacia el jugador
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    playerController.transform.position,
                    magnetSpeed * Time.deltaTime
                );
            }
        }
    }
}