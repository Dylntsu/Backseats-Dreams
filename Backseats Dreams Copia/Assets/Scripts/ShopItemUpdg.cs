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
    public TextMeshProUGUI totalCoinsText;

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

    private const string BANK_KEY = "TotalPlayerCoins";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentLevel = PlayerPrefs.GetInt(upgradeName + "_Level", 0);

        UpdateUI(false);
    }

    public void BuyUpgrade()
    {
        CalculateCurrentCost();
        int totalMoney = PlayerPrefs.GetInt(BANK_KEY, 0);
        int oldMoney = totalMoney;

        if (totalMoney >= currentCost && currentLevel < maxLevel)
        {
            // --- LOGICA ---
            totalMoney -= currentCost;
            PlayerPrefs.SetInt(BANK_KEY, totalMoney);

            currentLevel++;
            PlayerPrefs.SetInt(upgradeName + "_Level", currentLevel);
            PlayerPrefs.Save();

            //animacion boton
            StartCoroutine(ButtonPunchEffect());

            UpdateUI(true, oldMoney, totalMoney);

            // pitch aleatorio entre 0.9 y 1.1
            if (purchaseSoundEffect != null && audioSource != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(purchaseSoundEffect);
            }

            PlayPurchaseEffect();
        }
        else
        {
            // error con pitch bajito
            if (errorSFX != null && audioSource != null)
            {
                audioSource.pitch = 0.8f;
                audioSource.PlayOneShot(errorSFX);
            }
            // efecto de sacudida
            StartCoroutine(ButtonShakeEffect());
        }
    }

    IEnumerator AnimateCoinsCount(int startValue, int endValue)
    {
        float duration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            // Lerp calcula el valor intermedio
            int currentValue = (int)Mathf.Lerp(startValue, endValue, elapsed / duration);

            if (totalCoinsText != null)
                totalCoinsText.text = "Coins: " + currentValue.ToString();

            yield return null;
        }
        // Asegurar que termine en el valor exacto
        if (totalCoinsText != null)
            totalCoinsText.text = "Coins: " + endValue.ToString();
    }

    // Efecto de encoger y estirar el botón
    IEnumerator ButtonPunchEffect()
    {
        Transform btnTransform = buyButton.transform;
        Vector3 originalScale = Vector3.one; 

        // Encoger rápido
        float time = 0;
        while (time < 0.1f)
        {
            time += Time.unscaledDeltaTime;
            btnTransform.localScale = Vector3.Lerp(originalScale, originalScale * 0.9f, time / 0.1f);
            yield return null;
        }

        //vuelve a crecer
        time = 0;
        while (time < 0.1f)
        {
            time += Time.unscaledDeltaTime;
            btnTransform.localScale = Vector3.Lerp(originalScale * 0.9f, originalScale, time / 0.1f);
            yield return null;
        }
        btnTransform.localScale = originalScale;
    }

    IEnumerator ButtonShakeEffect()
    {
        Transform btnTransform = buyButton.transform;
        Vector3 originalPos = btnTransform.localPosition;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-5f, 5f); // Mueve 5 pixeles a los lados
            btnTransform.localPosition = originalPos + new Vector3(x, 0, 0);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        btnTransform.localPosition = originalPos;
    }


    void PlayPurchaseEffect()
    {
        if (purchaseParticlePrefab != null)
        {
            Transform spawnTransform = effectSpawnPoint != null ? effectSpawnPoint : buyButton.transform;
            GameObject vfx = Instantiate(purchaseParticlePrefab, spawnTransform.position, Quaternion.identity);
            Destroy(vfx, 2.0f);
        }
    }

    void UpdateUI(bool animateCoins = false, int oldCoins = 0, int newCoins = 0)
    {
        CalculateCurrentCost();

        // Barras
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
        }

        // Texto de Monedas
        if (totalCoinsText != null)
        {
            if (animateCoins)
            {
                StopAllCoroutines(); // Detener animaciones anteriores para evitar conflictos
                StartCoroutine(ButtonPunchEffect()); // Reiniciar el punch si se detuvo
                StartCoroutine(AnimateCoinsCount(oldCoins, newCoins));
            }
            else
            {
                totalCoinsText.text = "Coins: " + PlayerPrefs.GetInt(BANK_KEY, 0).ToString();
            }
        }
    }

    // Calcula el costo actual basado en el nivel
    void CalculateCurrentCost()
    {
        currentCost = Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }
}