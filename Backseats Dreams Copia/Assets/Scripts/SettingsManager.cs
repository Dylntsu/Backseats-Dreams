using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Componentes de Audio")]
    public AudioMixer masterMixer;
    public Slider volumeSlider;
    public Slider sfxSlider;
    public Slider ambienceSlider;

    [Header("Componentes de Brillo")]
    public Image brightnessOverlay;
    public Slider brightnessSlider;

    private const string MUSIC_VOLUME_KEY = "MusicaVolume";
    private const float MIN_VOLUME = 0.0001f;
    private const string BRIGHTNESS_KEY = "MasterBrightness";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const float MIN_BRIGHTNESS = 0.0001f; 
    private const string AMBIENCE_VOLUME_KEY = "AmbienceVolume"; 


    void Start()
    {
        // caraga el volumen guardado
        float savedVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        volumeSlider.value = Mathf.Max(savedVolume, 0.0001f);
        SetVolume(volumeSlider.value);

        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        sfxSlider.value = Mathf.Max(savedSFXVolume, MIN_VOLUME);
        SetSFXVolume(sfxSlider.value); 

        float savedBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);

        brightnessSlider.value = Mathf.Max(savedBrightness, MIN_BRIGHTNESS);

        float savedAmbienceVolume = PlayerPrefs.GetFloat(AMBIENCE_VOLUME_KEY, 1f);
        ambienceSlider.value = Mathf.Max(savedAmbienceVolume, MIN_VOLUME);
        SetAmbienceVolume(ambienceSlider.value);

        SetBrightness(brightnessSlider.value);


    }
    public void SetSFXVolume(float volume)
    {
        float safeVolume = Mathf.Max(volume, MIN_VOLUME);
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(safeVolume) * 20);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, safeVolume);
    }
    /// <summary>
    /// Esta función la llamara el Slider cada vez que se mueva
    /// </summary>
    public void SetVolume(float volume)
    {
        float safeVolume = Mathf.Max(volume, MIN_VOLUME);

        // valor seguro nunca sera 0
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(safeVolume) * 20);

        // guarda el valor seguro
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, safeVolume);
    }

    public void SetBrightness(float brightness)
    {
        float safeBrightness = Mathf.Max(brightness, MIN_BRIGHTNESS);

        // invierte el valor para el overlay
        float newAlpha = 1.0f - safeBrightness;

        if (brightnessOverlay != null) 
        {
            brightnessOverlay.color = new Color(0, 0, 0, newAlpha);
        }
        // guarda el valor seguro
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, safeBrightness);
    }
    public void SetAmbienceVolume(float volume)
    {
        float safeVolume = Mathf.Max(volume, MIN_VOLUME);
        masterMixer.SetFloat("AmbienceVolume", Mathf.Log10(safeVolume) * 20);
        PlayerPrefs.SetFloat(AMBIENCE_VOLUME_KEY, safeVolume);
    }
}