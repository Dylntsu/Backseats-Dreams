using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private Vector3 originalLocalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        instance = this;
        originalLocalPosition = transform.localPosition;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        // Si ya está temblando, reseteamos para aplicar el nuevo impacto
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originalLocalPosition;
        }
        shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Aplicar la sacudida a la posición local de la cámara
            transform.localPosition = new Vector3(originalLocalPosition.x + x, originalLocalPosition.y + y, originalLocalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        //regresa la camara a su posicion original
        transform.localPosition = originalLocalPosition;
        shakeCoroutine = null;
    }
}