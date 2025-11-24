using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BrightnessLoader : MonoBehaviour
{
    private Image overlayImage;

    private const string BRIGHTNESS_KEY = "MasterBrightness";

    void Start() // carga el brillo guardado al iniciar
    {
        overlayImage = GetComponent<Image>();

        float savedBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);

        ApplyBrightness(savedBrightness);
    }

    private void ApplyBrightness(float brightness)
    {
    
        float newAlpha = 1.0f - brightness;

        //panel negro
        overlayImage.color = new Color(0, 0, 0, newAlpha);
    }
}