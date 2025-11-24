using UnityEngine;

public class LevitationEffect : MonoBehaviour
{
    [Header("Configuración de Levitación")]
    [Tooltip("Velocidad del movimiento")]
    public float speed = 1.5f;

    [Tooltip("Altura maxima del movimiento")]
    public float height = 0.25f;

    private Vector3 startPos;

    void Start()
    {
        //posicion inicial
        startPos = transform.position;
    }

    void Update()
    {
        // se calcula el nuevo desplazamiento vertical
        float newY = Mathf.Sin(Time.time * speed);
        float verticalOffset = newY * height;

        //se modifica solo la posicion Y del objeto
        transform.position = new Vector3(
            transform.position.x,
            startPos.y + verticalOffset,
            transform.position.z
        );
    }
}