using UnityEngine;

public class MainMenuInput : MonoBehaviour
{
    [Header("UI Salida")]
    public GameObject quitConfirmationPanel; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (quitConfirmationPanel.activeSelf)
            {
                // si el panel ya esta abierto se cierra
                CerrarPanelSalida();
            }
            else
            {
                // si el panel no esta abierto se abre
                AbrirPanelSalida();
            }
        }
    }

    public void AbrirPanelSalida()
    {
        quitConfirmationPanel.SetActive(true);
    }

    public void CerrarPanelSalida()
    {
        quitConfirmationPanel.SetActive(false);
    }

}