using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour
{
    // --- DEFINICION DE ESTADOS ---
    public enum PlayerState
    {
        Running,        // Corriendo en el suelo
        Jumping,        // En el aire 
        Crouching,      // Deslizándose en el suelo
        FastFalling,    // Cayendo rápido
        Hurt,           // Recibiendo daño
        Dead,           // Game Over por golpe
        FallingSewer    // Game Over por alcantarilla
    }

    [Header("Estado Actual (Solo lectura)")]
    public PlayerState currentState;

    [Header("Efectos de Cámara")]
    public CameraShake cameraShaker;
    [Tooltip("Duración del temblor al chocar")]
    public float hitShakeDuration = 0.4f;
    [Tooltip("Fuerza del temblor (0.1 es suave, 0.5 es fuerte)")]
    public float hitShakeMagnitude = 0.2f;

    [Header("Configuración General")]
    public int attempts = 3;

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

    [Header("Controles Móviles")]
    public float swipeThreshold = 50f; // distancia minima
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [Header("Componentes y Referencias")]
    public SpriteRenderer playerSprite;
    public GameManager gameManager;
    public GameObject shieldVisual;

    [Header("Efectos Visuales")]
    public GameObject shieldHitSfxPrefab;
    public GameObject shieldEndSfxPrefab;
    public ParticleSystem runEffectParticles;

    [Header("Configuración de Agacharse")]
    public Vector2 crouchColliderSize;
    public Vector2 crouchColliderOffset;
    public float crouchDuration = 0.8f;

    [Header("Skins")]
    public AnimatorOverrideController skinNaranja;

    // --- ESTADO DE POTENCIADORES ---
    public bool isMagnetActive { get; private set; } = false;
    public bool isShieldActive { get; private set; } = false;
    public bool isDoubleCoinsActive { get; private set; } = false;

    // --- VARIABLES PRIVADAS ---
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

        if (gameManager != null) startingGameSpeed = gameManager.baseSpeed;
        else startingGameSpeed = 10f;

        ChangeState(PlayerState.Running);
    }

    void Update()
    {
        CheckMobileInput();

        switch (currentState)
        {
            case PlayerState.Running:
                HandleRunningState();
                break;
            case PlayerState.Jumping:
                HandleJumpingState();
                break;
            case PlayerState.Crouching:
                break;
        }

        if (currentState != PlayerState.Dead && currentState != PlayerState.FallingSewer)
        {
            UpdateAnimationSpeed();
            HandleRunParticles();
        }
        else
        {
            if (runEffectParticles != null && runEffectParticles.isEmitting) runEffectParticles.Stop();
        }
    }

    // ==================================================================
    // --- LÓGICA DE INPUT MÓVIL Y TECLADO ---
    // ==================================================================

    private void CheckMobileInput()
    {
        // detectar inicio del toque
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        // detectar fin del toque
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        if (swipeDelta.magnitude >= swipeThreshold)
        {
            if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
            {
                if (swipeDelta.y > 0)
                {
                    // Swipe hacia arriba
                    AttemptJump();
                }
                else
                {
                    // Swipe hacia abajo
                    AttemptCrouchOrFall();
                }
            }
        }
    }

    //conexion de inputs con estados
    private void AttemptJump()
    {
        if (currentState == PlayerState.Running)
        {
            ChangeState(PlayerState.Jumping);
        }
    }

    private void AttemptCrouchOrFall()
    {
        if (currentState == PlayerState.Running)
        {
            ChangeState(PlayerState.Crouching);
        }
        else if (currentState == PlayerState.Jumping)
        {
            ChangeState(PlayerState.FastFalling);
        }
    }

    public void OnSwipeUp()
    {
        if (currentState == PlayerState.Running)
        {
            ChangeState(PlayerState.Jumping);
        }
    }

    public void OnSwipeDown()
    {
        if (currentState == PlayerState.Running)
        {
            ChangeState(PlayerState.Crouching);
        }
        else if (currentState == PlayerState.Jumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10f);
        }
    }

    // ==================================================================
    // --- COMPORTAMIENTO POR ESTADO
    // ==================================================================

    void HandleRunningState()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            AttemptJump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AttemptCrouchOrFall();
        }
    }

    void HandleJumpingState()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AttemptCrouchOrFall();
        }
    }

    // ==================================================================
    // --- MÁQUINA DE ESTADOS
    // ==================================================================
    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        ExitCurrentState();

        currentState = newState;

        switch (currentState)
        {
            case PlayerState.Running:
                anim.SetBool("isGrounded", true);
                break;

            case PlayerState.Jumping:
                anim.SetBool("isGrounded", false);
                anim.SetTrigger("Jump");
                if (jumpSound) audioSource.PlayOneShot(jumpSound);
                rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
                break;

            case PlayerState.Crouching:
                StartCoroutine(CrouchRoutine());
                break;

            case PlayerState.FastFalling:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.down * fastFallForce, ForceMode2D.Impulse);
                break;

            case PlayerState.Hurt:
                StartCoroutine(HurtRoutine());
                break;

            case PlayerState.Dead:
                anim.SetBool("isDead", true);
                if (hurtSound) audioSource.PlayOneShot(hurtSound);
                StartCoroutine(GameOverRoutine());
                break;

            case PlayerState.FallingSewer:
                StartCoroutine(SewerFallRoutine());
                break;
        }
    }

    private void ExitCurrentState()
    {
        switch (currentState)
        {
            case PlayerState.Crouching:
                StopCoroutine("CrouchRoutine");
                collider2d.size = standColliderSize;
                collider2d.offset = standColliderOffset;
                anim.SetBool("isCrounching", false);
                break;

            case PlayerState.Hurt:
                StopCoroutine("HurtRoutine");
                playerSprite.color = Color.white;
                break;
        }
    }

    // ==================================================================
    // --- FÍSICAS Y COLISIONES ---
    // ==================================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            if (currentState == PlayerState.Jumping || currentState == PlayerState.FastFalling)
            {
                ChangeState(PlayerState.Running);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == PlayerState.Dead || currentState == PlayerState.FallingSewer) return;

        // Caída a alcantarilla
        if (collision.gameObject.CompareTag("sewer"))
        {
            if (audioSource && fallSound) audioSource.PlayOneShot(fallSound);
            ChangeState(PlayerState.FallingSewer);
        }
        // Choque con obstáculo
        else if (collision.gameObject.CompareTag("hObstacle") || collision.gameObject.CompareTag("obstacle"))
        {
            // Si el escudo está activo, destruimos el obstáculo sin daño ni temblor
            if (isShieldActive)
            {
                if (destroyObstacle) audioSource.PlayOneShot(destroyObstacle);
                if (shieldHitSfxPrefab) Instantiate(shieldHitSfxPrefab, collision.transform.position, Quaternion.identity);

                collision.gameObject.SetActive(false);
                return;
            }

            // --- AQUI ACTIVAMOS EL TEMBLOR DE CÁMARA ---
            if (cameraShaker != null)
            {
                cameraShaker.TriggerShake(hitShakeDuration, hitShakeMagnitude);
            }

            // Lógica de Vidas
            if (attempts > 0)
            {
                attempts--;
                if (crashSound) audioSource.PlayOneShot(crashSound);
                if (gameManager) gameManager.RemoveLifeIcon();

                if (collision.gameObject.CompareTag("hObstacle"))
                {
                    anim.SetTrigger("Hurt");
                }

                ChangeState(PlayerState.Hurt);
            }
            else
            {
                ChangeState(PlayerState.Dead);
            }
        }
    }

    // ==================================================================
    // --- CORRUTINAS ---
    // ==================================================================

    private IEnumerator CrouchRoutine()
    {
        anim.SetBool("isCrounching", true);
        collider2d.size = crouchColliderSize;
        collider2d.offset = crouchColliderOffset;

        yield return new WaitForSeconds(crouchDuration);

        if (currentState == PlayerState.Crouching)
        {
            ChangeState(PlayerState.Running);
        }
    }

    private IEnumerator HurtRoutine()
    {
        playerSprite.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        playerSprite.color = Color.white;

        if (currentState == PlayerState.Hurt)
        {
            ChangeState(PlayerState.Running);
        }
    }

    private IEnumerator SewerFallRoutine()
    {
        collider2d.enabled = false;
        anim.SetBool("isFalling", true);

        yield return new WaitForSeconds(1f);
        rb.gravityScale = 20f;

        if (gameManager) gameManager.GameOverScreen();
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        if (gameManager) gameManager.GameOverScreen();
    }

    // ==================================================================
    // --- AUXILIARES (Partículas, Skins, PowerUps) ---
    // ==================================================================

    private void HandleRunParticles()
    {
        if (runEffectParticles == null) return;

        if (currentState == PlayerState.Running)
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
        anim.SetFloat("runSpeedMultiplier", currentWorldSpeed / startingGameSpeed);
    }

    // --- POWER UPS ---
    public void ActivateMagnet(float duration)
    {
        StopCoroutine("MagnetCoroutine");
        StartCoroutine(MagnetCoroutine(duration));
        if (gameManager?.uiManager) gameManager.uiManager.ActivatePowerUpIndicator(PowerUp.PowerUpType.Magnet, duration);
    }
    private IEnumerator MagnetCoroutine(float duration) { isMagnetActive = true; yield return new WaitForSeconds(duration); isMagnetActive = false; }

    public void ActivateShield(float duration)
    {
        StopCoroutine("ShieldCoroutine");
        StartCoroutine(ShieldCoroutine(duration));
        if (gameManager?.uiManager) gameManager.uiManager.ActivatePowerUpIndicator(PowerUp.PowerUpType.Shield, duration);
    }
    private IEnumerator ShieldCoroutine(float duration)
    {
        isShieldActive = true; shieldVisual.SetActive(true);
        yield return new WaitForSeconds(duration);
        isShieldActive = false; shieldVisual.SetActive(false);
        Instantiate(shieldEndSfxPrefab, transform.position, Quaternion.identity);
    }

    public void ActivateDoubleCoins(float duration)
    {
        StopCoroutine("DoubleCoinsCoroutine");
        StartCoroutine(DoubleCoinsCoroutine(duration));
        if (gameManager?.uiManager) gameManager.uiManager.ActivatePowerUpIndicator(PowerUp.PowerUpType.DoubleCoins, duration);
    }
    private IEnumerator DoubleCoinsCoroutine(float duration)
    {
        isDoubleCoinsActive = true;
        if (gameManager) gameManager.SetCoinMultiplier(2);
        yield return new WaitForSeconds(duration);
        isDoubleCoinsActive = false;
        if (gameManager) gameManager.SetCoinMultiplier(1);
    }

    // Equipamiento de skins
    public void EquiparSkin(AnimatorOverrideController nuevaSkin)
    {
        anim.runtimeAnimatorController = (nuevaSkin != null) ? nuevaSkin : skinOriginal;
    }
}