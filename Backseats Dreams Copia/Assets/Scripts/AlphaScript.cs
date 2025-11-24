using UnityEngine;
using UnityEngine.UI;

public class AlphaClick : MonoBehaviour
{
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}