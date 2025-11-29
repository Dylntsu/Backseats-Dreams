using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Configuración de Fade")]
    public float fadeDuration = 0.5f;
    private bool isFirstLoad = true;

    // --- Variables de Panel ---
    private CanvasGroup fadeScreen;
    private CanvasGroup storePanel;
    private CanvasGroup mainMenuPanel;
    private CanvasGroup optionsPanel;

    public static UIController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private CanvasGroup FindCanvasGroupByNameTag(string tagName, Canvas rootCanvas)
    {
        CanvasGroup[] groups = rootCanvas.GetComponentsInChildren<CanvasGroup>(true);
        return groups.FirstOrDefault(g => g.CompareTag(tagName));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            return;// Si no se encuentra un Canvas, salir de la función
        }

        fadeScreen = FindCanvasGroupByNameTag("FadeScreen", canvas);
        mainMenuPanel = FindCanvasGroupByNameTag("MainMenuPanel", canvas);
        storePanel = FindCanvasGroupByNameTag("StorePanel", canvas);
        optionsPanel = FindCanvasGroupByNameTag("OptionsPanel", canvas);

        GameObject exitBtnObj = GameObject.FindGameObjectWithTag("ExitButton");

        if (exitBtnObj != null)
        {
            Button btn = exitBtnObj.GetComponent<Button>();

            btn.onClick.RemoveAllListeners();

            btn.onClick.AddListener(QuitGame);
        }

        InitializePanelState(optionsPanel);

        if (mainMenuPanel != null)
        {
            mainMenuPanel.alpha = 1f;
            mainMenuPanel.interactable = true;
            mainMenuPanel.blocksRaycasts = true;
            mainMenuPanel.gameObject.SetActive(true);
        }

        if (fadeScreen != null)
        {
            if (isFirstLoad)
            {
                isFirstLoad = false;
                fadeScreen.alpha = 0f;
                fadeScreen.blocksRaycasts = false;
            }
            else
            {
                StartCoroutine(FadeIn());
            }
        }
    }

    private void InitializePanelState(CanvasGroup panel)
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(true);
            panel.alpha = 0f;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    // --- FUNCIONES DE PANELES ---
    public void ShowStorePanel()
    {
        if (mainMenuPanel != null) StartCoroutine(FadeOutPanel(mainMenuPanel));
        if (storePanel != null) StartCoroutine(FadeInPanel(storePanel));
    }
    public void HideStorePanel()
    {
        if (storePanel != null) StartCoroutine(FadeOutPanel(storePanel));
        if (mainMenuPanel != null) StartCoroutine(FadeInPanel(mainMenuPanel));
    }

    public void ShowOptionsPanel()
    {
        if (mainMenuPanel != null) StartCoroutine(FadeOutPanel(mainMenuPanel));
        if (optionsPanel != null) StartCoroutine(FadeInPanel(optionsPanel));
    }

    public void HideOptionsPanel()
    {
        if (optionsPanel != null) StartCoroutine(FadeOutPanel(optionsPanel));
        if (mainMenuPanel != null) StartCoroutine(FadeInPanel(mainMenuPanel));
    }

    public void ShowPanel(CanvasGroup panelToShow)
    {
        StartCoroutine(FadeInPanel(panelToShow));
    }
    public void HidePanel(CanvasGroup panelToHide)
    {
        StartCoroutine(FadeOutPanel(panelToHide));
    }


    // --- CORRUTINAS ---

    private IEnumerator FadeIn()
    {
        if (fadeScreen == null) yield break;
        fadeScreen.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeScreen.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeScreen.alpha = 0f;
        fadeScreen.blocksRaycasts = false;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (fadeScreen == null)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
            yield break;
        }
        fadeScreen.blocksRaycasts = true;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeScreen.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeScreen.alpha = 1f;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // --- CORRUTINAS DE PANELES ---
    private IEnumerator FadeInPanel(CanvasGroup canvasGroup)
    {

        if (!canvasGroup.gameObject.activeSelf)
        {
            canvasGroup.alpha = 0f;
        }

        canvasGroup.gameObject.SetActive(true);

        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOutPanel(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (canvasGroup == storePanel)
        {
            canvasGroup.gameObject.SetActive(false);
        }
        //
    }
    public void QuitGame()
    {
        Debug.Log("saliendo del juego");

        Application.Quit();

        // detener el modo de reproducción en el editor de Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}