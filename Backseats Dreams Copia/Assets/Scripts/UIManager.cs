using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Paneles Principales")]
    public GameObject gameUIPanel; 

    [Header("Componentes del Game Over")]
    public CanvasGroup gameOverCanvasGroup;
    public float fadeDuration = 1.0f;

    [Header("Indicador de PowerUp")]
    public GameObject powerUpIndicatorContainer;
    public Image powerUpIcon;
    public Image cooldownImage;    

    [Header("Sprites de PowerUps")]
    public Sprite magnetSprite;
    public Sprite shieldSprite;
    public Sprite doubleCoinsSprite;

    private Coroutine activeTimerCoroutine;

    void Start()
    {
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0f;
            gameOverCanvasGroup.interactable = false;
        }
    }

    public void ActivatePowerUpIndicator(PowerUp.PowerUpType type, float duration)
    {
        // Mostrar el Contenedor del Indicador
        Sprite iconToShow = null;
        switch (type)
        {
            case PowerUp.PowerUpType.Magnet:
                iconToShow = magnetSprite;
                break;
            case PowerUp.PowerUpType.Shield:
                iconToShow = shieldSprite;
                break;
            case PowerUp.PowerUpType.DoubleCoins:
                iconToShow = doubleCoinsSprite;
                break;
        }

        if (iconToShow != null)
        {
            powerUpIcon.sprite = iconToShow;
        }

        powerUpIndicatorContainer.SetActive(true);

        // verificación de corrutina activa
        if (activeTimerCoroutine != null) StopCoroutine(activeTimerCoroutine);
        activeTimerCoroutine = StartCoroutine(PowerUpTimerRoutine(duration));
    }

    private IEnumerator PowerUpTimerRoutine(float duration)
    {
        float timer = duration;

        // Animación de "pop" al aparecer
        powerUpIndicatorContainer.transform.localScale = Vector3.zero;
        float popTime = 0f;
        while (popTime < 0.2f)
        {
            popTime += Time.deltaTime;
            powerUpIndicatorContainer.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, popTime / 0.2f);
            yield return null;
        }
        powerUpIndicatorContainer.transform.localScale = Vector3.one;

        // cuentta  regresiva
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = timer / duration;
            }

            yield return null;
        }

        powerUpIndicatorContainer.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        // Oculta el panel de UI del juego
        if (gameUIPanel != null)
        {
            gameUIPanel.SetActive(false);
        }

        StartCoroutine(FadeInPanel(gameOverCanvasGroup));
    }

    private IEnumerator FadeInPanel(CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(true);

        float elapsedTime = 0f;

        // Bucle que se ejecuta hasta que se completa la duración
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            canvasGroup.alpha = newAlpha;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Asegura que el alpha sea 1 al final
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}