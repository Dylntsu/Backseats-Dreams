using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Sistema de Guardado (JSON)")]
    public PlayerData currentData; // <--- TUS DATOS VIVEN AQUI AHORA

    [Header("Sistema de Pausa")]
    public GameObject pausePanel;
    public static bool isPaused = false;

    [Header("Panel de Juego")]
    public CanvasGroup gameUIContainerCanvasGroup;
    public float fadeInDuration = 0.5f;

    [Header("Animaciones UI")]
    public float jumpScale = 1.2f;
    public float jumpDuration = 0.5f;

    [Header("UI de Partida")]
    public TextMeshProUGUI coinText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public GameObject newRecordText;
    public TextMeshProUGUI countdownText;

    public Transform ScoreContainer;
    public Transform CoinContainer;
    public Transform CoinBackground;

    [Header("Referencias del Juego")]
    public playerController player;
    public UIManager uiManager;
    public SpawnManager spawnManager;

    [Header("Fade")]
    public CanvasGroup fadeScreen;
    public float fadeDuration = 0.5f;

    [Header("UI de Vidas")]
    public GameObject lifeIconPrefab;
    public Transform livesContainer;

    [Header("Lógica de Velocidad")]
    public float baseSpeed = 10f;
    [Range(0.1f, 1f)]
    public float slowSpeedMultiplier = 0.6f;
    public float speedIncreaseRate = 0.1f;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public AudioClip CountDownSound;

    private float currentGlobalSpeed;
    private int coinMultiplier = 1;
    private int coinsThisRun = 0;
    private float totalScore = 0f;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private float loadedMusicVolumeDb;
    private List<GameObject> lifeIcons = new List<GameObject>();

    private const float MIN_VOLUME = 0.0001f;
    private const float ZOOM_DURATION = 0.5f;
    private const float TARGET_SCALE = 300f;

    void Start()
    {
        currentData = SaveSystem.Load();

        // Configuración inicial
        isPaused = false;
        Time.timeScale = 1f;
        Application.targetFrameRate = 60;

        // Setup UI Inicial
        if (gameUIContainerCanvasGroup != null)
        {
            gameUIContainerCanvasGroup.alpha = 0f;
        }
        else
        {
            if (ScoreContainer) ScoreContainer.gameObject.SetActive(false);
            if (CoinContainer) CoinContainer.gameObject.SetActive(false);
            if (livesContainer) livesContainer.gameObject.SetActive(false);
            if (CoinBackground) CoinBackground.gameObject.SetActive(false);
        }

        currentGlobalSpeed = 0f;
        totalScore = 0f;
        coinsThisRun = 0;
        UpdateCoinText();

        player = FindFirstObjectByType<playerController>();
        spawnManager = FindFirstObjectByType<SpawnManager>();

        // Audio Setup
        AudioSource[] allAudioSources = GetComponents<AudioSource>();
        if (allAudioSources.Length >= 2)
        {
            musicSource = allAudioSources[0].playOnAwake ? allAudioSources[0] : allAudioSources[1];
            sfxSource = allAudioSources[0].playOnAwake ? allAudioSources[1] : allAudioSources[0];
        }

        if (fadeScreen != null) fadeScreen.alpha = 0f;

        InitializeLives();

        currentGlobalSpeed = baseSpeed;

        float sliderValue = currentData.musicVolume;
        loadedMusicVolumeDb = sliderValue <= MIN_VOLUME ? -80f : Mathf.Log10(sliderValue) * 20;
        // Aplicamos el volumen al mixer inmediatamente
        if (mainMixer != null) mainMixer.SetFloat("MusicVolume", loadedMusicVolumeDb);


        if (pausePanel != null) pausePanel.SetActive(false);

        StartCoroutine(StartCountdown());
    }

    // === GUARDADO AL MINIMIZAR ===
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // El usuario minimizó la app o recibió una llamada, guardar
            SaveSystem.Save(currentData);
        }
    }

    // Respaldo por si se cierra
    void OnApplicationQuit()
    {
        SaveSystem.Save(currentData);
    }
    // ===================================================

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameOverPanel != null && !gameOverPanel.activeSelf && !countdownText.gameObject.activeSelf)
            {
                TogglePause();
            }
        }

        if (player != null &&
            player.currentState != playerController.PlayerState.Dead &&
            player.currentState != playerController.PlayerState.FallingSewer)
        {
            if (!isPaused)
            {
                baseSpeed += speedIncreaseRate * Time.deltaTime;

                if (player.currentState == playerController.PlayerState.Hurt ||
                    player.currentState == playerController.PlayerState.Crouching)
                {
                    currentGlobalSpeed = baseSpeed * slowSpeedMultiplier;
                }
                else
                {
                    currentGlobalSpeed = baseSpeed;
                }

                totalScore += GetCurrentSpeed() * Time.deltaTime;
                UpdateScoreText();
            }
        }
        else if (player != null)
        {
            currentGlobalSpeed = 0f;
        }
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // GUARDA AL PAUSAR
        SaveSystem.Save(currentData);

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            CanvasGroup cg = pausePanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
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
        if (scoreText != null) scoreText.text = totalScore.ToString("F0") + "m";
    }

    void UpdateCoinText()
    {
        coinText.text = coinsThisRun.ToString();
    }

    void InitializeLives()
    {
        foreach (Transform child in livesContainer) Destroy(child.gameObject);
        lifeIcons.Clear();

        if (player == null) return;

        for (int i = 0; i < player.attempts; i++)
        {
            GameObject newLifeIcon = Instantiate(lifeIconPrefab, livesContainer);
            Vector3 pos = newLifeIcon.transform.localPosition;
            newLifeIcon.transform.localPosition = new Vector3(pos.x, pos.y, 0);
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
            float targetDb = targetLinearVol <= MIN_VOLUME ? -80f : Mathf.Log10(targetLinearVol) * 20f;
            mainMixer.SetFloat("MusicVolume", targetDb);
        }

        if (uiManager != null)
        {
            uiManager.ShowGameOverPanel();
            if (player.currentState == playerController.PlayerState.FallingSewer)
            {
                gameOverText.text = "looks like you've fallen";
            }
        }

        if (finalScoreText != null) finalScoreText.text = totalScore.ToString("F0") + "m";

        // Sumar monedas al total global
        currentData.coins += coinsThisRun;

        // Revisar HighScore
        if (totalScore > currentData.highScore)
        {
            if (newRecordText != null) newRecordText.SetActive(true);
            currentData.highScore = (int)totalScore; // Casteo a int
        }

        // GUARDAR EN DISCO
        SaveSystem.Save(currentData);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
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

    private IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        mainMixer.SetFloat("MusicVolume", -80f);
        sfxSource.PlayOneShot(CountDownSound);

        string[] count = { "3", "2", "1" };
        foreach (string c in count)
        {
            countdownText.text = c;
            StartCoroutine(JumpEffect(countdownText.transform));
            yield return new WaitForSeconds(1f);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(livesContainer.GetComponent<RectTransform>());

        countdownText.text = "Run!";
        countdownText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ZoomEffect(countdownText.transform));

        currentGlobalSpeed = baseSpeed;
        spawnManager.StartSpawning();
        mainMixer.SetFloat("MusicVolume", loadedMusicVolumeDb);

        if (gameUIContainerCanvasGroup != null)
            StartCoroutine(FadeInCanvasGroup(gameUIContainerCanvasGroup, fadeInDuration));

        countdownText.gameObject.SetActive(false);
    }

    private IEnumerator ZoomEffect(Transform textTransform)
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(TARGET_SCALE, TARGET_SCALE, TARGET_SCALE);
        float time = 0;
        while (time < ZOOM_DURATION)
        {
            time += Time.deltaTime;
            textTransform.localScale = Vector3.Lerp(startScale, endScale, Mathf.Pow(time / ZOOM_DURATION, 3));
            yield return null;
        }
        textTransform.localScale = endScale;
    }

    private IEnumerator JumpEffect(Transform textTransform)
    {
        Vector3 originalScale = textTransform.localScale;
        Vector3 targetScale = originalScale * jumpScale;
        float halfDuration = jumpDuration / 2f;
        float time = 0f;

        while (time < halfDuration)
        {
            time += Time.deltaTime;
            textTransform.localScale = Vector3.Lerp(originalScale, targetScale, time / halfDuration);
            yield return null;
        }
        time = 0f;
        while (time < halfDuration)
        {
            time += Time.deltaTime;
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, time / halfDuration);
            yield return null;
        }
        textTransform.localScale = originalScale;
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, time / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}