using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Panel de Juego (Fade-In)")]
    public CanvasGroup gameUIContainerCanvasGroup;
    public float fadeInDuration = 0.5f;

    public float jumpScale = 1.2f;  
    public float jumpDuration = 0.5f;

    [Header("UI de Partida")]
    public TextMeshProUGUI coinText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    private int coinsThisRun = 0;
    public TextMeshProUGUI finalScoreText;
    public GameObject newRecordText;
    public TextMeshProUGUI countdownText;

    public Transform ScoreContainer;
    public Transform CoinContainer;
    public Transform CoinBackground;

    private float totalScore = 0f;

    [Header("Referencias del Juego")]
    public playerController player;
    public UIManager uiManager;
    public SpawnManager spawnManager;

    [Header("Fade")]
    public CanvasGroup fadeScreen;
    public float fadeDuration = 0.5f;

    private const float ZOOM_DURATION = 0.5f;
    private const float TARGET_SCALE = 300f;

    [Header("UI de Vidas")]
    public GameObject lifeIconPrefab;
    public Transform livesContainer;
    private List<GameObject> lifeIcons = new List<GameObject>();

    [Header("Lógica de Velocidad del Juego")]
    public float baseSpeed = 10f; 

    [Tooltip("Porcentaje de la velocidad normal al estar herido/agachado. 0.6 = 60%")]
    [Range(0.1f, 1f)]
    public float slowSpeedMultiplier = 0.6f;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public AudioClip CountDownSound;

    public float speedIncreaseRate = 0.1f;

    // --- Variables Privadas ---
    private float currentGlobalSpeed;
    private int coinMultiplier = 1;
    private const string TOTAL_COINS_KEY = "TotalPlayerCoins";

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float loadedMusicVolumeDb;
    private const string MUSIC_VOLUME_KEY = "MusicaVolume";
    private const float MIN_VOLUME = 0.0001f;

    private const string HIGHSCORE_KEY = "HighScore";


    void Start()
    {
        //ocultar ui al inici
        if (gameUIContainerCanvasGroup != null)
        {
            gameUIContainerCanvasGroup.alpha = 0f;
        }
        else
        {
            ScoreContainer.gameObject.SetActive(false);
            CoinContainer.gameObject.SetActive(false);
            livesContainer.gameObject.SetActive(false);
            CoinBackground.gameObject.SetActive(false);
        }

        currentGlobalSpeed = 0f;
        Time.timeScale = 1f;

        totalScore = 0f;
        coinsThisRun = 0;
        UpdateCoinText();

        player = FindFirstObjectByType<playerController>();
        spawnManager = FindFirstObjectByType<SpawnManager>();

        AudioSource[] allAudioSources = GetComponents<AudioSource>();
        if (allAudioSources.Length >= 2)
        {
            musicSource = allAudioSources[0].playOnAwake ? allAudioSources[0] : allAudioSources[1];
            sfxSource = allAudioSources[0].playOnAwake ? allAudioSources[1] : allAudioSources[0];
        }

        if (fadeScreen != null)
            fadeScreen.alpha = 0f;
        InitializeLives();

        currentGlobalSpeed = baseSpeed;

        float sliderValue = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1.0f);

        if (sliderValue <= MIN_VOLUME)
        {
            loadedMusicVolumeDb = -80f;
        }
        else
        {
            loadedMusicVolumeDb = Mathf.Log10(sliderValue) * 20;
        }

        StartCoroutine(StartCountdown());
    }

    // ******************************************************************************
    // CORRUTINA DEL ZOOM
    // ******************************************************************************
    private IEnumerator ZoomEffect(Transform textTransform)
    {
        Vector3 startScale = Vector3.one; // Aseguramos que empiece en 1
        Vector3 endScale = new Vector3(TARGET_SCALE, TARGET_SCALE, TARGET_SCALE);
        float time = 0;

        while (time < ZOOM_DURATION)
        {
            time += Time.deltaTime;
            float normalizedTime = time / ZOOM_DURATION;

            float accelerationCurve = normalizedTime * normalizedTime * normalizedTime;

            textTransform.localScale = Vector3.Lerp(startScale, endScale, accelerationCurve);

            yield return null;
        }

        textTransform.localScale = endScale;
    }

    // CORRUTINA DE SALTO 
    private IEnumerator JumpEffect(Transform textTransform)
    {
        Vector3 originalScale = textTransform.localScale;
        Vector3 targetScale = originalScale * jumpScale;

        float halfDuration = jumpDuration / 2f;
        float time = 0f;

        while (time < halfDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / halfDuration;
            textTransform.localScale = Vector3.Lerp(originalScale, targetScale, normalizedTime);
            yield return null;
        }

        time = 0f;
        while (time < halfDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / halfDuration;
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, normalizedTime);
            yield return null;
        }

        textTransform.localScale = originalScale;
    }

    // CORRUTINA DE FADE IN 
    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = 1f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    // ******************************************************************************
    // CORRUTINA DE CUENTA REGRESIVA
    // ******************************************************************************
    private IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        mainMixer.SetFloat("MusicVolume", -80f);
        sfxSource.PlayOneShot(CountDownSound);

        // cuenta regresiva 
        countdownText.text = "3";
        StartCoroutine(JumpEffect(countdownText.transform));
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        StartCoroutine(JumpEffect(countdownText.transform));
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        StartCoroutine(JumpEffect(countdownText.transform));
        yield return new WaitForSeconds(1f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(livesContainer.GetComponent<RectTransform>());

        countdownText.text = "Run!";

        //zoom effect
        countdownText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ZoomEffect(countdownText.transform));

        // inicia la partida
        currentGlobalSpeed = baseSpeed;
        spawnManager.StartSpawning();
        mainMixer.SetFloat("MusicVolume", loadedMusicVolumeDb);

        // HUD con fade-in
        if (gameUIContainerCanvasGroup != null)
        {
            StartCoroutine(FadeInCanvasGroup(gameUIContainerCanvasGroup, fadeInDuration));
        }
              // Oculta el texto después del zoom y de haber iniciado el fade del HUD
        countdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player != null && !player.isDead)
        {
            baseSpeed += speedIncreaseRate * Time.deltaTime;

            if (player.isHurt || player.isCrouching)
            {
                currentGlobalSpeed = baseSpeed * slowSpeedMultiplier;
            }
            else
            {
                currentGlobalSpeed = baseSpeed;
            }

            float scoreThisFrame = GetCurrentSpeed() * Time.deltaTime;
            totalScore += scoreThisFrame;
            UpdateScoreText();
        }
        else if (player != null && player.isDead)
        {
            currentGlobalSpeed = 0f;
        }
    }

    public float GetCurrentSpeed()
    {
        return currentGlobalSpeed;
    }

    public void AddCoin(int coinValue)
    {
        coinsThisRun += coinValue * coinMultiplier;
        UpdateCoinText();
    }

    public void SetCoinMultiplier(int multiplier)
    {
        coinMultiplier = multiplier;
    }
    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = totalScore.ToString("F0") + "m";
    }
    void UpdateCoinText()
    {
        coinText.text = coinsThisRun.ToString();
    }

    void InitializeLives()
    {
        foreach (Transform child in livesContainer)
        {
            Destroy(child.gameObject);
        }
        lifeIcons.Clear();

        if (player == null)
        {
            Debug.LogError("¡GameManager no pudo encontrar al Player en Start()!");
            return;
        }

        for (int i = 0; i < player.attempts; i++)
        {
            GameObject newLifeIcon = Instantiate(lifeIconPrefab, livesContainer);
            newLifeIcon.transform.localPosition = new Vector3(newLifeIcon.transform.localPosition.x, newLifeIcon.transform.localPosition.y, 0);
            lifeIcons.Add(newLifeIcon);
        }
    }

    public void RemoveLifeIcon()
    {
        if (lifeIcons.Count == 0) return;

        int lastIconIndex = lifeIcons.Count - 1;
        GameObject iconToRemove = lifeIcons[lastIconIndex];
        lifeIcons.RemoveAt(lastIconIndex);
        Destroy(iconToRemove);
    }

    public void GameOverScreen()
    {
        Time.timeScale = 0f;

        if (mainMixer != null)
        {
            float currentLinearVol = Mathf.Pow(10, loadedMusicVolumeDb / 20f);
            float targetLinearVol = currentLinearVol * 0.25f;
            float targetDb;

            if (targetLinearVol <= MIN_VOLUME)
            {
                targetDb = -80f;
            }
            else
            {
                targetDb = Mathf.Log10(targetLinearVol) * 20f;
            }

            mainMixer.SetFloat("MusicVolume", targetDb);
        }
        if (uiManager != null)
        {
            uiManager.ShowGameOverPanel();

            if (player.isFell == true)
            {
                gameOverText.text = "looks like you've fallen";
            }
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = totalScore.ToString("F0") + "m";
        }

        float highScore = PlayerPrefs.GetFloat(HIGHSCORE_KEY, 0);

        if (totalScore > highScore)
        {
            Debug.Log("¡Nuevo Récord!");
            if (newRecordText != null)
            {
                newRecordText.SetActive(true);
            }

            PlayerPrefs.SetFloat(HIGHSCORE_KEY, totalScore);
            PlayerPrefs.Save();
        }

        SaveCoinsToBank();
    }
    private void SaveCoinsToBank()
    {
        int totalCoinsInBank = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
        totalCoinsInBank += coinsThisRun;
        PlayerPrefs.SetInt(TOTAL_COINS_KEY, totalCoinsInBank);
        PlayerPrefs.Save();
        Debug.Log($"Partida terminada. Se guardaron {coinsThisRun} monedas. Nuevo total en banco: {totalCoinsInBank}");
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        StartCoroutine(GoToMenuCoroutine());
    }

    private IEnumerator GoToMenuCoroutine()
    {
        if (fadeScreen != null)
        {
            fadeScreen.gameObject.SetActive(true);
            fadeScreen.blocksRaycasts = true;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                fadeScreen.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            fadeScreen.alpha = 1f;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}