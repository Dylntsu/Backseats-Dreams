using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // === Economía ===
    public int coins;
    public int highScore;

    // === Configuración ===
    public float musicVolume;
    public bool isTutorialCompleted;

    // === MEJORAS ===
    public int magnetLevel;
    public int shieldLevel;
    public int doubleCoinsLevel;

    // CONSTRUCTOR
    public PlayerData()
    {
        coins = 0;
        highScore = 0;
        musicVolume = 1.0f;
        isTutorialCompleted = false;

        // Niveles iniciales
        magnetLevel = 0;
        shieldLevel = 0;
        doubleCoinsLevel = 0;
    }
}