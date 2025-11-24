using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Magnet, Shield, DoubleCoins }

    public PowerUpType type;
    public AudioClip collectPowerUpSound;

    // Claves de NIVELES
    private const string MAGNET_LEVEL_KEY = "Magnet_Level";
    private const string SHIELD_LEVEL_KEY = "Shield_Level";
    private const string DOUBLE_LEVEL_KEY = "DoubleCoins_Level";

    // Configuración de Tiempos
    [Header("Configuración por Tipo")]
    public float magnetBaseTime = 5f;
    public float magnetBonusPerLevel = 2.5f;

    public float shieldBaseTime = 5f;
    public float shieldBonusPerLevel = 1.5f;

    public float doubleBaseTime = 5f;
    public float doubleBonusPerLevel = 2.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerController controller = other.GetComponent<playerController>();
            AudioSource playerAudio = other.GetComponent<AudioSource>();

            if (controller != null)
            {
                if (playerAudio != null && collectPowerUpSound != null)
                {
                    playerAudio.PlayOneShot(collectPowerUpSound);
                }

                float finalDuration = 0f;
                int level = 0;

                if (type == PowerUpType.Magnet)
                {
                    level = PlayerPrefs.GetInt(MAGNET_LEVEL_KEY, 0);
                    finalDuration = magnetBaseTime + (level * magnetBonusPerLevel);
                    controller.ActivateMagnet(finalDuration);
                }
                else if (type == PowerUpType.Shield)
                {
                    level = PlayerPrefs.GetInt(SHIELD_LEVEL_KEY, 0);
                    finalDuration = shieldBaseTime + (level * shieldBonusPerLevel);
                    controller.ActivateShield(finalDuration);
                }
                else if (type == PowerUpType.DoubleCoins)
                {
                    level = PlayerPrefs.GetInt(DOUBLE_LEVEL_KEY, 0);
                    finalDuration = doubleBaseTime + (level * doubleBonusPerLevel);
                    controller.ActivateDoubleCoins(finalDuration);
                }

                Debug.Log($"PowerUp {type} recogido. Nivel: {level}. Duración: {finalDuration}s");
            }

            gameObject.SetActive(false);
        }
    }
}