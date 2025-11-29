using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ShopItemUpdg : MonoBehaviour
{
    [Header("Item Config")]

    public string upgradeName = "Magnet";
    public int maxLevel = 5;
    public int baseCost = 500;
    public float costMultiplier = 1.5f;

    [Header("Visual References")]
    public Image[] levelBars;
    public TextMeshProUGUI costText;
    public Button buyButton;
    public TextMeshProUGUI totalCoinsText; // Referencia al texto grande de monedas

    [Header("Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1, 1, 1, 0.3f);

    [Header("SFX")]
    public AudioClip purchaseSoundEffect;
    public AudioClip errorSFX;

    [Header("VFX")]
    public GameObject purchaseParticlePrefab;
    public Transform effectSpawnPoint;

    private AudioSource audioSource;
    private int currentLevel = 0;
    private int currentCost = 0;

    // DATOS LOCALES
    private PlayerData currentData;

    void OnEnable() // Usamos OnEnable para refrescar siempre al abrir
    {
        audioSource = GetComponent<AudioSource>();

        currentData = SaveSystem.Load();

        currentLevel = GetLevelFromData();

        UpdateUI(false);
    }

    private int GetLevelFromData()
    {
        switch (upgradeName)
        {
            case "Magnet": return currentData.magnetLevel;
            case "Shield": return currentData.shieldLevel;
            case "DoubleCoins": return currentData.doubleCoinsLevel;
            default:
                Debug.LogError("Nombre de upgrade incorrecto en Inspector: " + upgradeName);
                return 0;
        }
    }

    private void SetLevelToData(int newLevel)
    {
        switch (upgradeName)
        {
            case "Magnet": currentData.magnetLevel = newLevel; break;
            case "Shield": currentData.shieldLevel = newLevel; break;
            case "DoubleCoins": currentData.doubleCoinsLevel = newLevel; break;
        }
    }
    // -----------------------------------------------------------

    public void BuyUpgrade()
    {
        // Recargar datos por seguridad antes de comprar
        currentData = SaveSystem.Load();
        currentLevel = GetLevelFromData();

        CalculateCurrentCost();

        int totalMoney = currentData.coins; // Leemos del JSON
        int oldMoney = totalMoney;

        if (totalMoney >= currentCost && currentLevel < maxLevel)
        {
            // --- LOGICA DE COMPRA ---
            totalMoney -= currentCost;
            currentData.coins = totalMoney; // Actualizamos dinero en memoria

            currentLevel++;
            SetLevelToData(currentLevel); // Actualizamos nivel en memoria

            // GUARDAMOS EN DISCO
            SaveSystem.Save(currentData);

            // --- FEEDBACK VISUAL ---
            StartCoroutine(ButtonPunchEffect());

            UpdateUI(true, oldMoney, totalMoney);

            // SFX
            if (purchaseSoundEffect != null && audioSource != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(purchaseSoundEffect);
            }

            PlayPurchaseEffect();

            ShopManager sm = FindFirstObjectByType<ShopManager>();
            if (sm != null) sm.RefreshShopUI();
        }
        else
        {
            // ERROR
            if (errorSFX != null && audioSource != null)
            {
                audioSource.pitch = 0.8f;
                audioSource.PlayOneShot(errorSFX);
            }
            StartCoroutine(ButtonShakeEffect());
        }
    }
    void UpdateUI(bool animateCoins = false, int oldCoins = 0, int newCoins = 0)
    {
        CalculateCurrentCost();

        // Barras Visuales
        for (int i = 0; i < levelBars.Length; i++)
        {
            if (i < currentLevel) levelBars[i].color = activeColor;
            else levelBars[i].color = inactiveColor;
        }

        // Botón y Costo
        if (currentLevel >= maxLevel)
        {
            costText.text = "MAX";
            buyButton.interactable = false;
        }
        else
        {
            costText.text = currentCost.ToString();
            buyButton.interactable = true;
        }

        // Texto de Monedas
        if (totalCoinsText != null)
        {
            if (animateCoins)
            {
                StopAllCoroutines();
                StartCoroutine(ButtonPunchEffect());
                StartCoroutine(AnimateCoinsCount(oldCoins, newCoins));
            }
            else
            {
                // Leemos directamente de currentData.coins
                if (currentData != null)
                    totalCoinsText.text = "COINS: " + currentData.coins.ToString();
            }
        }
    }

    IEnumerator AnimateCoinsCount(int startValue, int endValue)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            int currentValue = (int)Mathf.Lerp(startValue, endValue, elapsed / duration);

            if (totalCoinsText != null)
                totalCoinsText.text = "COINS: " + currentValue.ToString();

            yield return null;
        }
        if (totalCoinsText != null)
            totalCoinsText.text = "COINS: " + endValue.ToString();
    }

    IEnumerator ButtonPunchEffect()
    { 
        Transform btnTransform = buyButton.transform;
        Vector3 originalScale = Vector3.one;
        float time = 0;
        while (time < 0.1f) { time += Time.unscaledDeltaTime; btnTransform.localScale = Vector3.Lerp(originalScale, originalScale * 0.9f, time / 0.1f); yield return null; }
        time = 0;
        while (time < 0.1f) { time += Time.unscaledDeltaTime; btnTransform.localScale = Vector3.Lerp(originalScale * 0.9f, originalScale, time / 0.1f); yield return null; }
        btnTransform.localScale = originalScale;
    }

    IEnumerator ButtonShakeEffect()
    { 
        Transform btnTransform = buyButton.transform;
        Vector3 originalPos = btnTransform.localPosition;
        float duration = 0.2f; float elapsed = 0f;
        while (elapsed < duration) { float x = Random.Range(-5f, 5f); btnTransform.localPosition = originalPos + new Vector3(x, 0, 0); elapsed += Time.unscaledDeltaTime; yield return null; }
        btnTransform.localPosition = originalPos;
    }

    void PlayPurchaseEffect()
    { 
        if (purchaseParticlePrefab != null) { Transform spawnTransform = effectSpawnPoint != null ? effectSpawnPoint : buyButton.transform; GameObject vfx = Instantiate(purchaseParticlePrefab, spawnTransform.position, Quaternion.identity); Destroy(vfx, 2.0f); }
    }

    void CalculateCurrentCost()
    {
        currentCost = Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }
}