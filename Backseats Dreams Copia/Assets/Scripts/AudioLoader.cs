using UnityEngine;
using UnityEngine.Audio;

public class AudioLoader : MonoBehaviour
{
    public AudioMixer masterMixer;

    private const string MUSIC_VOLUME_KEY = "MusicaVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string AMBIENCE_VOLUME_KEY = "AmbienceVolume";
    private const float MIN_VOLUME = 0.0001f;

    void Start()
    {
        if (masterMixer == null) return;

        // cargar volumen de la musica
        float savedVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        float safeVolume = Mathf.Max(savedVolume, MIN_VOLUME);
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(safeVolume) * 20);

        // cargar volumen de SFX
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        float safeSFXVolume = Mathf.Max(savedSFXVolume, MIN_VOLUME);
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(safeSFXVolume) * 20);

        // cargar volumen ambiente
        float savedAmbienceVolume = PlayerPrefs.GetFloat(AMBIENCE_VOLUME_KEY, 1f);
        float safeAmbienceVolume = Mathf.Max(savedAmbienceVolume, MIN_VOLUME);
        masterMixer.SetFloat("AmbienceVolume", Mathf.Log10(safeAmbienceVolume) * 20);
    }
}