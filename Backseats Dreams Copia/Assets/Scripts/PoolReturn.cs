using UnityEngine;

public class PoolReturn : MonoBehaviour
{
    // Se ejecuta cuando el objeto sale de la vista de la cámara
    void OnBecameInvisible()
    {
        // En lugar de Destroy(gameObject)
        gameObject.SetActive(false);
    }

    public void ReturnToPoolAfterCollision()
    {
        gameObject.SetActive(false);
    }
}