using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public PlayerController playerController;
    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStartPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                swipeEndPosition = touch.position;
                DetectSwipeDirection();
            }
        }
    }

    void DetectSwipeDirection()
    {
        Vector2 swipeDirection = swipeEndPosition - swipeStartPosition;

        if (swipeDirection.x > 0)
        {
            playerController.JumpRight();
        }
        else if (swipeDirection.x < 0)
        {
            playerController.JumpLeft();
        }
    }
}
