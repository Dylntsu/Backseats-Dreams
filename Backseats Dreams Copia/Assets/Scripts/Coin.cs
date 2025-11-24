using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    private GameManager gameManager;
    public ParticleSystem collectParticles;
    public AudioClip collectSound;
    public float magnetSpeed = 15f;
    public float magnetRange = 7f;

    private playerController playerController;
    private bool isCollected = false;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerController = GameObject.FindWithTag("Player").GetComponent<playerController>();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            if (gameManager != null)
            {
                Debug.Log("Añadiendo moneda al contador.");
                gameManager.AddCoin(coinValue);
            }
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        Debug.Log("¡Moneda recogida!");

        // oculta la moneda
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // desvincula las particulas para que no se destruyan con la moneda
        collectParticles.transform.SetParent(null);

        collectParticles.Play();

        //tiempo de vida de las partículas
        float particleLifeTime = collectParticles.main.startLifetime.constantMax;

        Destroy(collectParticles.gameObject, particleLifeTime);

        Destroy(gameObject);
    }
    private void Update()
    {
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