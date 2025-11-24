using UnityEngine;
using UnityEngine.UI;

public class MenuUILinker : MonoBehaviour
{
    [Header("Botones del Menu Principal")]
    public Button playButton;
    public Button optionsButton;
    public Button storeButton;

    [Header("Botones de Paneles")]
    public Button closeOptionsButton; 
    public Button closeStoreButton;

    void Start()
    {
        UIController controller = UIController.instance;

        if (controller == null)
        {
            Debug.LogError("no se encontro UIController.");
            return;
        }

        // logica de vinculacion de botones
        if (playButton != null)
            playButton.onClick.AddListener(() => controller.LoadSceneWithFade("Nivel1"));

        if (optionsButton != null)
            optionsButton.onClick.AddListener(controller.ShowOptionsPanel);

        if (storeButton != null)
            storeButton.onClick.AddListener(controller.ShowStorePanel);

        if (closeOptionsButton != null)
            closeOptionsButton.onClick.AddListener(controller.HideOptionsPanel);

        if (closeStoreButton != null)
            closeStoreButton.onClick.AddListener(controller.HideStorePanel);
    }
}