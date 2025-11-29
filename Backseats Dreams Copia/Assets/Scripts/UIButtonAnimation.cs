using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración Hover")]
    public float hoverScale = 1.1f; 
    public float hoverDuration = 0.1f; 
    public AudioClip hoverSound;

    [Header("Configuración Botón Jugar")]
    public bool isPlayButton = false;
    public string sceneToLoad = "GameScene"; 
    public float transitionDuration = 0.5f; 
    public float finalScale = 50f;

    private Vector3 originalScale;
    private AudioSource audioSource;
    private Button btn;
    private Canvas canvas;

    void Start()
    {
        originalScale = transform.localScale;
        btn = GetComponent<Button>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        canvas = GetComponentInParent<Canvas>();
    }

    // --- LoGICA DE HOVER  ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        //verificacion de seguridad
        if (btn == null) btn = GetComponent<Button>();

        if (btn == null || !btn.interactable) return;

        StopAllCoroutines();
        StartCoroutine(AnimateScale(originalScale * hoverScale, hoverDuration));

        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    // --- L0GICA DE SALIDA ---
    public void OnPointerExit(PointerEventData eventData)
    {
        //verificacion de seguridad
        if (btn == null) btn = GetComponent<Button>();

        if (btn == null || !btn.interactable) return;

        StopAllCoroutines();
        StartCoroutine(AnimateScale(originalScale, hoverDuration));
    }

    public void PlayGameTransition()
    {
        if (isPlayButton)
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    // --- CORRUTINAS ---
    IEnumerator AnimateScale(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            yield return null;
        }
        transform.localScale = targetScale;
    }

    // Corrutina para la transición al juego
    IEnumerator TransitionRoutine()
    {
        btn.interactable = false;


        transform.SetAsLastSibling();

        //Animación de Zoom
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(finalScale, finalScale, 1f);

        float time = 0;
        while (time < transitionDuration)
        {
            float t = time / transitionDuration;
            t = t * t * t; // Cubic Ease In

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            time += Time.unscaledDeltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}