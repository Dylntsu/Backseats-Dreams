using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseSettings : MonoBehaviour
{
    [Header("Referencias Generales")]
    public AudioMixer mainMixer;
    // CAMBIO: Ahora pedimos una Image, no un CanvasGroup
    public Image brightnessOverlayImage; 

    [Header("Sliders")]
    public Slider brightnessSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider atmosphereSlider;

    // Nombres de parámetros (Asegúrate de exponerlos en el Mixer)
    private const string VOL_MUSIC = "MusicVolume"; 
    private const string VOL_SFX = "SFXVolume";
    private const string VOL_ATMOS = "AmbienceVolume";
    // IMPORTANTE: Usamos la misma clave que tu script "BrightnessLoader"
    private const string PREF_BRIGHTNESS = "MasterBrightness"; 

    void Start()
    {
        // 1. Cargar valores
        float valMusic = PlayerPrefs.GetFloat(VOL_MUSIC, 1f);
        float valSFX = PlayerPrefs.GetFloat(VOL_SFX, 1f);
        float valAtmos = PlayerPrefs.GetFloat(VOL_ATMOS, 1f);
        // Usamos la misma clave que ya tenías
        float valBright = PlayerPrefs.GetFloat(PREF_BRIGHTNESS, 1f); 

        // 2. Ajustar posición visual de sliders
        if(musicSlider) musicSlider.value = valMusic;
        if(sfxSlider) sfxSlider.value = valSFX;
        if(atmosphereSlider) atmosphereSlider.value = valAtmos;
        if(brightnessSlider) brightnessSlider.value = valBright;

        // 3. Aplicar valores iniciales
        SetMusicVolume(valMusic);
        SetSFXVolume(valSFX);
        SetAtmosphereVolume(valAtmos);
        SetBrightness(valBright); // Esto actualizará tu imagen negra

        // 4. Conectar eventos
        if(musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if(sfxSlider) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if(atmosphereSlider) atmosphereSlider.onValueChanged.AddListener(SetAtmosphereVolume);
        if(brightnessSlider) brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    // --- FUNCIONES DE AUDIO ---
    public void SetMusicVolume(float value) { ApplyVolume(VOL_MUSIC, value); }
    public void SetSFXVolume(float value) { ApplyVolume(VOL_SFX, value); }
    public void SetAtmosphereVolume(float value) { ApplyVolume(VOL_ATMOS, value); }

    private void ApplyVolume(string parameterName, float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        if(mainMixer != null) mainMixer.SetFloat(parameterName, dbValue);
        PlayerPrefs.SetFloat(parameterName, sliderValue);
    }

    // --- FUNCIÓN DE BRILLO ADAPTADA A TU SISTEMA ---
    public void SetBrightness(float value)
    {
        // Lógica: 1.0 (Slider) = Transparente (Juego Brillante)
        // Lógica: 0.0 (Slider) = Negro Opaco (Juego Oscuro)
        float alphaValue = 1f - value; 

        if (brightnessOverlayImage != null)
        {
            // CAMBIO: Modificamos el color directamente, igual que tu BrightnessLoader
            brightnessOverlayImage.color = new Color(0, 0, 0, alphaValue);
        }

        // Guardamos en la misma clave "MasterBrightness"
        PlayerPrefs.SetFloat(PREF_BRIGHTNESS, value);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}