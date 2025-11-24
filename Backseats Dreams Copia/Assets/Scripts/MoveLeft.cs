using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private GameManager gameManager;
    private float leftBound = -15f; 

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager == null) Debug.LogError("no se encontro el gameManger");
    }

    void Update()
    {
        if (gameManager == null) return;

        float globalSpeed = gameManager.GetCurrentSpeed();
        transform.Translate(Vector3.left * globalSpeed * Time.deltaTime);

        // object pooling screen bounds check
        if (transform.position.x < leftBound)
        {
            gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        if (transform.position.x < 0)
        {
            gameObject.SetActive(false);
        }
    }
}