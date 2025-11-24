using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour
{
    [Header("Estado del Jugador")]
    public bool isGrounded;
    public bool isDead = false;
    public int attempts = 3;
    public bool isFell = false;
    public bool isCrouching = false;
    public bool isHurt = false;

    [Header("Sonidos")]
    public AudioClip jumpSound;
    public AudioClip crashSound;
    public AudioClip fallSound;
    public AudioClip hurtSound;
    public AudioClip destroyObstacle;

    [Header("Movimiento")]
    public float speed = 10.0f;
    public float jumpForce = 5.0f;
    public float fastFallForce = 15.0f;

    [Header("Componentes y Referencias")]
    public SpriteRenderer playerSprite;
    public GameManager gameManager;
    public GameObject shieldVisual;

    [Header("Efectos de Partículas")]
    public GameObject shieldHitSfxPrefab;
    public GameObject shieldEndSfxPrefab;
    public ParticleSystem runEffectParticles;

    [Header("Configuración de Agacharse")]
    public Vector2 crouchColliderSize;
    public Vector2 crouchColliderOffset;
    public float crouchDuration = 0.8f;

    [Header("Skins")]
    public AnimatorOverrideController skinNaranja;

    //potenciadores
    public bool isMagnetActive { get; private set; } = false;
    public bool isShieldActive { get; private set; } = false;
    public bool isDoubleCoinsActive { get; private set; } = false;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D collider2d;
    private RuntimeAnimatorController skinOriginal;
    private Vector2 standColliderSize;
    private Vector2 standColliderOffset;
    private AudioSource audioSource;

    private float startingGameSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<CapsuleCollider2D>();
        gameManager = FindFirstObjectByType<GameManager>();
        audioSource = GetComponent<AudioSource>();

        if (shieldVisual != null) shieldVisual.SetActive(false);

        standColliderSize = collider2d.size;
        standColliderOffset = collider2d.offset;

        skinOriginal = anim.runtimeAnimatorController;
        EquiparSkin(skinNaranja);

        if (gameManager != null)
        {
            startingGameSpeed = gameManager.baseSpeed;
        }
        else
        {
            startingGameSpeed = 10f;
            Debug.LogWarning("PlayerController no pudo encontrar GameManager.");
        }
    }

    void Update()
    {
        if (!isDead)
        {
            PlayerJump();
            PlayerCrouch();
            UpdateAnimationSpeed();
            HandleRunParticles();
        }
        else
        {
            if (runEffectParticles != null && runEffectParticles.isEmitting)
            {
                runEffectParticles.Stop();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (isDead) return;

        if (collision.gameObject.CompareTag("sewer"))
        {
            StartCoroutine(FallSewer());
            audioSource.PlayOneShot(fallSound);
        }
        else if (collision.gameObject.CompareTag("hObstacle") || collision.gameObject.CompareTag("obstacle"))
        {
            // logica escudo
            if (isShieldActive)
            {
                audioSource.PlayOneShot(destroyObstacle);

                if (shieldHitSfxPrefab != null)
                {
                    Instantiate(shieldHitSfxPrefab, collision.transform.position, Quaternion.identity);
                }

                
                collision.gameObject.SetActive(false);

                return;
            }

            // si el escudo no esta activo, procesa el daño
            if (attempts > 0)
            {
                attempts--;

                if (crashSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(crashSound);
                }

                if (gameManager != null)
                {
                    gameManager.RemoveLifeIcon();
                }

                StartCoroutine(FlashRed());
                Debug.Log("Golpe. Quedan " + attempts + " intentos.");

                if (collision.gameObject.CompareTag("hObstacle"))
                {
                    anim.SetTrigger("Hurt");
                }

            }
            else // attempts = 0
            {
                StartCoroutine(HandleGameOver());
            }
        }
    }


    private void HandleRunParticles()
    {
        if (runEffectParticles == null) return;

        bool isRunning = isGrounded && !isCrouching && !isHurt && !isDead;

        if (isRunning)
        {
            if (!runEffectParticles.isEmitting) runEffectParticles.Play();
        }
        else
        {
            if (runEffectParticles.isEmitting) runEffectParticles.Stop();
        }
    }

    private void UpdateAnimationSpeed()
    {
        if (gameManager == null || startingGameSpeed <= 0) return;

        float currentWorldSpeed = gameManager.GetCurrentSpeed();
        float speedRatio = currentWorldSpeed / startingGameSpeed;
        anim.SetFloat("runSpeedMultiplier", speedRatio);
    }

    private void PlayerJump()
    {
        if (isCrouching) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("isGrounded", false);
            anim.SetTrigger("Jump");

            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    private void PlayerCrouch()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isGrounded && !isCrouching)
            {
                StartCoroutine(CrouchSequence());
            }
            else if (!isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.down * fastFallForce, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator CrouchSequence()
    {
        isCrouching = true;
        anim.SetBool("isCrounching", true);
        collider2d.size = crouchColliderSize;
        collider2d.offset = crouchColliderOffset;

        yield return new WaitForSeconds(crouchDuration);

        if (!isDead)
        {
            collider2d.size = standColliderSize;
            collider2d.offset = standColliderOffset;
            anim.SetBool("isCrounching", false);
            isCrouching = false;
        }
    }

    private IEnumerator HandleGameOver()
    {
        Debug.Log("Game Over");
        isDead = true;
        anim.SetBool("isDead", true);

        audioSource.PlayOneShot(hurtSound);

        yield return new WaitForSeconds(1.0f);

        if (gameManager != null)
        {
            gameManager.GameOverScreen();
        }
    }

    // --- IMAN ---
    public void ActivateMagnet(float duration)
    {
        StopCoroutine("MagnetCoroutine"); 
        StartCoroutine(MagnetCoroutine(duration));

        if (gameManager != null && gameManager.uiManager != null)
        {
            gameManager.uiManager.ActivatePowerUpIndicator(PowerUp.PowerUpType.Magnet, duration);
        }
    }
    private IEnumerator MagnetCoroutine(float duration)
    {
        isMagnetActive = true;
        yield return new WaitForSeconds(duration);
        isMagnetActive = false;
    }

    // --- ESCUDO ---
    public void ActivateShield(float duration)
    {
        StopCoroutine("ShieldCoroutine");
        StartCoroutine(ShieldCoroutine(duration));

        if (gameManager != null && gameManager.uiManager != null)
        {
            gameManager.uiManager.ActivatePowerUpIndicator(PowerUp.PowerUpType.Shield, duration);
        }
    }
    private IEnumerator ShieldCoroutine(float duration)
    {
        isShieldActive = true;
        shieldVisual.SetActive(true);

        yield return new WaitForSeconds(duration);

        isShieldActive = false;
        shieldVisual.SetActive(false);
        Instantiate(shieldEndSfxPrefab, transform.position, Quaternion.identity);
    }

    // --- MONEDAS DOBLES ---
    public void ActivateDoubleCoins(float duration)
    {
        StopCoroutine("DoubleCoinsCoroutine");
        StartCoroutine(DoubleCoinsCoroutine(duration));

        if (gameManager != null && gameManager.uiManager != null)
        {
            gameManager.uiManager.ActivatePowerUpIndicator(PowerUp.PowerUpType.DoubleCoins, duration);
        }
    }
    private IEnumerator DoubleCoinsCoroutine(float duration)
    {
        isDoubleCoinsActive = true;

        if (gameManager != null)
        {
            gameManager.SetCoinMultiplier(2);
        }

        yield return new WaitForSeconds(duration);

        isDoubleCoinsActive = false;

        if (gameManager != null)
        {
            gameManager.SetCoinMultiplier(1);
        }
    }

    public IEnumerator FlashRed()
    {
        isHurt = true;
        playerSprite.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        playerSprite.color = Color.white;
        isHurt = false;
    }

    public IEnumerator FallSewer()
    {
        if (isDead) yield break;

        isDead = true;
        isFell = true;
        collider2d.enabled = false;

        anim.SetBool("isFalling", true);

        yield return new WaitForSeconds(1f);

        rb.gravityScale = 20f;

        if (gameManager != null)
        {
            gameManager.GameOverScreen();
        }
    }

    public void EquiparSkin(AnimatorOverrideController nuevaSkin)
    {
        if (nuevaSkin == null)
        {
            anim.runtimeAnimatorController = skinOriginal;
        }
        else
        {
            anim.runtimeAnimatorController = nuevaSkin;
        }
    }
}