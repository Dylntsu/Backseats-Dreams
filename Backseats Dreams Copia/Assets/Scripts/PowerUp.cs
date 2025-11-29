using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Magnet, Shield, DoubleCoins }

    public PowerUpType type;
    public AudioClip collectPowerUpSound;

    [Header("Configuración de Tiempos")]
    public float magnetBaseTime = 5f;
    public float magnetBonusPerLevel = 2.5f;

    public float shieldBaseTime = 5f;
    public float shieldBonusPerLevel = 1.5f;

    public float doubleBaseTime = 5f;
    public float doubleBonusPerLevel = 2.5f;

    private GameManager gameManager;

    private void Start()
    {

        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("CRÍTICO: El PowerUp no encuentra el GameManager. Asegúrate de que existe en la escena.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Verificación de seguridad
            if (gameManager == null) return;

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
                    level = gameManager.currentData.magnetLevel; // <--- LECTURA DIRECTA DE MEMORIA
                    finalDuration = magnetBaseTime + (level * magnetBonusPerLevel);
                    controller.ActivateMagnet(finalDuration);
                }
                else if (type == PowerUpType.Shield)
                {
                    level = gameManager.currentData.shieldLevel; // <--- LECTURA DIRECTA DE MEMORIA
                    finalDuration = shieldBaseTime + (level * shieldBonusPerLevel);
                    controller.ActivateShield(finalDuration);
                }
                else if (type == PowerUpType.DoubleCoins)
                {
                    level = gameManager.currentData.doubleCoinsLevel; // <--- LECTURA DIRECTA DE MEMORIA
                    finalDuration = doubleBaseTime + (level * doubleBonusPerLevel);
                    controller.ActivateDoubleCoins(finalDuration);
                }

                Debug.Log($"PowerUp {type} recogido. Nivel obtenido del JSON: {level}. Duración: {finalDuration}s");
            }

            gameObject.SetActive(false);
        }
    }
}