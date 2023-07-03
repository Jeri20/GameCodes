using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;
    public float gravityScale = 1f;
    public LayerMask groundLayer;
    public float raycastDistance = 0.1f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isSticking = false;
    private float stickySurfaceAngle = 0f;
    private bool isAlive = true;
    private bool isGameStarted = false;
    public bool hasFirstJumped = false;
    private float score;

    public Text scoreText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Disable gravity initially
        rb.constraints = RigidbodyConstraints2D.FreezePosition; // Freeze the player's position
        score = 0;
    }

    void Update()
    {
        if (!isGameStarted)
        {
            // Check for swipe to start the game
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartGame();
            }
            return;
        }

        if (isAlive)
        {
            // Check if the player is grounded or sticking to a surface
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
            isSticking = Mathf.Abs(stickySurfaceAngle) > 0f;

            // Jump only if the player is grounded or has made the first jump
            if ((isGrounded || (isSticking && hasFirstJumped)))
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    // Calculate swipe direction and jump accordingly
                    Vector2 touchDelta = Input.GetTouch(0).deltaPosition;
                    if (Mathf.Abs(touchDelta.x) > Mathf.Abs(touchDelta.y))
                    {
                        if (touchDelta.x > 0)
                            JumpRight();
                        else
                            JumpLeft();
                    }
                }
            }

            score += Time.deltaTime * 4;
            scoreText.text = "SCORE: " + score.ToString("F");
        }
    }

    void FixedUpdate()
    {
        // Apply sticky surface movement
        if (isSticking)
        {
            rb.velocity = Quaternion.Euler(0, 0, stickySurfaceAngle) * Vector2.up * jumpForce;
        }
    }

    void JumpLeft()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.left * jumpForce, ForceMode2D.Impulse);
        }
        else if (isSticking)
        {
            rb.AddForce(Quaternion.Euler(0, 0, -stickySurfaceAngle) * Vector2.left * jumpForce, ForceMode2D.Impulse);
        }
    }

    void JumpRight()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.right * jumpForce, ForceMode2D.Impulse);
        }
        else if (isSticking)
        {
            rb.AddForce(Quaternion.Euler(0, 0, -stickySurfaceAngle) * Vector2.right * jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("StickySurface"))
        {
            isSticking = true;
            stickySurfaceAngle = Vector2.SignedAngle(Vector2.up, collision.GetContact(0).normal);
        }
        else if (collision.gameObject.CompareTag("ground"))
        {
            if (!isGrounded)
            {
                isGrounded = true;
            }
        }
        else if (collision.gameObject.CompareTag("spike"))
        {
            if (!isAlive)
            {
                isAlive = false;
                Time.timeScale = 0;
                GameOver();
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("StickySurface"))
        {
            isSticking = false;
            stickySurfaceAngle = 0f;
        }
    }

    private void StartGame()
    {
        isGameStarted = true;
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.None; // Release the player's position constraints
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze the player's rotation
    }

    private void GameOver()
    {
        // Disable player movement
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        // Disable player input or show game-over UI
        // Implement your game-over logic here
        Debug.Log("Game Over");
    }
}
