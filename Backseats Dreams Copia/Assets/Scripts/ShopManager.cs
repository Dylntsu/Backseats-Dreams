using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI Referencias")]
    public TextMeshProUGUI totalCoinsText;

    [Header("Configuración de Niveles")]
    public int maxLevel = 5; 
    public int baseCost = 500;

    //claves de PlayerPrefs
    private const string MAGNET_LEVEL_KEY = "Magnet_Level";
    private const string SHIELD_LEVEL_KEY = "Shield_Level";
    private const string DOUBLE_LEVEL_KEY = "DoubleCoins_Level";
    private const string TOTAL_COINS_KEY = "TotalPlayerCoins";

    private int totalCoinsInBank;

    void Start()
    {

        totalCoinsInBank = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
        UpdateCoinText();
    }

    private void UpdateCoinText()   
    {
        totalCoinsText.text = totalCoinsInBank.ToString();
    }

// Logica de Compra
    public void BuyMagnetUpgrade()
    {
        int currentLevel = PlayerPrefs.GetInt(MAGNET_LEVEL_KEY, 0);

        if (currentLevel >= maxLevel)
        {
            Debug.Log("NIVEL MAXIMO DE IMAN");
            return;
        }

        if (totalCoinsInBank >= baseCost)
        {
            // Pagar
            totalCoinsInBank -= baseCost;
            PlayerPrefs.SetInt(TOTAL_COINS_KEY, totalCoinsInBank);

            // Subir Nivel
            currentLevel++;
            PlayerPrefs.SetInt(MAGNET_LEVEL_KEY, currentLevel);
            PlayerPrefs.Save();

            UpdateCoinText();
            Debug.Log("Imán mejorado al Nivel: " + currentLevel);

        }
        else
        {
            Debug.Log("No tienes suficientes monedas.");
        }
    }

    public void BuyShieldUpgrade()
    {
        int currentLevel = PlayerPrefs.GetInt(SHIELD_LEVEL_KEY, 0);

        if (currentLevel >= maxLevel) return;

        if (totalCoinsInBank >= baseCost)
        {
            totalCoinsInBank -= baseCost;
            PlayerPrefs.SetInt(TOTAL_COINS_KEY, totalCoinsInBank);

            currentLevel++;
            PlayerPrefs.SetInt(SHIELD_LEVEL_KEY, currentLevel);
            PlayerPrefs.Save();

            UpdateCoinText();
            Debug.Log("Escudo mejorado al Nivel: " + currentLevel);
        }
    }

    public void BuyDoubleCoinsUpgrade()
    {
        int currentLevel = PlayerPrefs.GetInt(DOUBLE_LEVEL_KEY, 0);

        if (currentLevel >= maxLevel) return;

        if (totalCoinsInBank >= baseCost)
        {
            totalCoinsInBank -= baseCost;
            PlayerPrefs.SetInt(TOTAL_COINS_KEY, totalCoinsInBank);

            currentLevel++;
            PlayerPrefs.SetInt(DOUBLE_LEVEL_KEY, currentLevel);
            PlayerPrefs.Save();

            UpdateCoinText();
            Debug.Log("Doble Monedas mejorado al Nivel: " + currentLevel);
        }
    }
}