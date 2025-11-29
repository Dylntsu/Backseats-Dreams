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

    private PlayerData currentData;

    void OnEnable()
    {
        currentData = SaveSystem.Load();

        UpdateCoinText();

        Debug.Log($"Tienda abierta. Cargando datos frescos. Monedas: {currentData.coins}");
    }

    private void UpdateCoinText()
    {
        if (totalCoinsText != null)
        {
            totalCoinsText.text = "COINS: " + currentData.coins.ToString();
            totalCoinsText.ForceMeshUpdate();
        }
        else
        {
            Debug.LogError("ERROR CRÍTICO: No has asignado el TotalCoinsText en el Inspector del ShopManager");
        }
    }

    public void BuyShieldUpgrade()
    {
        if (currentData.shieldLevel >= maxLevel) return;

        if (currentData.coins >= baseCost)
        {
            currentData.coins -= baseCost;
            currentData.shieldLevel++;

            SaveSystem.Save(currentData); // Guardar

            UpdateCoinText();
            Debug.Log("Escudo mejorado al Nivel: " + currentData.shieldLevel);
        }
    }

    public void BuyDoubleCoinsUpgrade()
    {
        if (currentData.doubleCoinsLevel >= maxLevel) return;

        if (currentData.coins >= baseCost)
        {
            currentData.coins -= baseCost;
            currentData.doubleCoinsLevel++;

            SaveSystem.Save(currentData); // Guardar

            UpdateCoinText();
            Debug.Log("Doble Monedas mejorado al Nivel: " + currentData.doubleCoinsLevel);
        }
    }
    public void RefreshShopUI()
    {
        currentData = SaveSystem.Load();
        UpdateCoinText();
        Debug.Log("UI Forzada a actualizarse.");
    }
}