using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public PlayerController playerController;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                float touchX = touch.position.x;
                float screenWidth = Screen.width;

                // Check if the touch is on the left or right half of the screen
                if (touchX < screenWidth / 2f)
                {
                    playerController.JumpLeft();
                }
                else
                {
                    playerController.JumpRight();
                }
            }
        }
    }
}
