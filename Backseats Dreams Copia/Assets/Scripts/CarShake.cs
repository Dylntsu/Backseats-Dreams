using UnityEngine;

public class CarFrameShake : MonoBehaviour
{
    [Header("Configuración de Vibración")]
    [Tooltip("Amplitud")]
    public float shakeStrength = 10f;

    [Tooltip("Frecuencia")]
    public float shakeSpeed = 2.0f;

    [Header("Variación Aleatoria")]
    [Tooltip("si es verdadero, usa el ruido para simular baches irregulares en lugar de una onda perfecta")]
    public bool useRandomNoise = false;

    private RectTransform rectTransform;
    private Vector2 originalPos;
    private float randomOffset;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();


        if (rectTransform != null)
        {
            originalPos = rectTransform.anchoredPosition;
        }
        //semilla aleatoria 
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (rectTransform == null) return;

        float yOffset = 0f;

        if (useRandomNoise)
        {
            // perlin noise para movimiento irregular
            float noiseVal = Mathf.PerlinNoise((Time.time * shakeSpeed) + randomOffset, 0f);
            yOffset = (noiseVal - 0.5f) * 2f * shakeStrength;
        }
        else
        {
            // onda senoidal para movimiento regular
            yOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeStrength;
        }

        //aplicar desplazamiento vertical
        rectTransform.anchoredPosition = new Vector2(originalPos.x, originalPos.y + yOffset);
    }
}