using UnityEngine;

public class MobileInputManager : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [SerializeField] private float swipeThreshold = 50f;

    public playerController player;

    void Update()
    {
        DetectSwipe();
    }

    void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            CalculateSwipe();
        }
    }

    void CalculateSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        //verificamos si la distancia del swipe es mayor al umbral
        if (swipeDelta.magnitude < swipeThreshold)
        {
            return; 
        }

        if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
        {
            if (swipeDelta.y > 0)
            {
                // salto
                player.OnSwipeUp();
            }
            else
            {
                //slide
                player.OnSwipeDown();
            }
        }
    }
}