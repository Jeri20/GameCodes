using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;
    public float gravityScale = 1f;
    public LayerMask groundLayer;
    public float raycastDistance = 0.1f;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isSticking = false;
    private float stickySurfaceAngle = 0f;
    private bool isAlive = true;
    private bool isGameStarted = false;
    public bool hasFirstJumped = false;
    private int coinCount;
    private int highestScore; // Added variable for highest score

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI highScoreText; // Added reference to high score TextMeshProUGUI

    private Vector2 dragStartPosition;
    private Vector2 dragEndPosition;

    private bool isJumpingLeft = false;
    private bool isJumpingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        coinCount = 0;

        // Load the highest score from PlayerPrefs
        highestScore = PlayerPrefs.GetInt("HighestScore", 0);

        // Update the high score text
        highScoreText.text = "HIGHEST: " + highestScore.ToString();
    }

    void Update()
    {
        if (!isGameStarted)
        {
            // Check for swipe to start the game
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartGame();
                hasFirstJumped= true;
            }
            return;
        }

        if (isAlive)
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
            isSticking = Mathf.Abs(stickySurfaceAngle) > 0f;

            if ((isGrounded || (isSticking && hasFirstJumped)))
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Vector2 touchPosition = Input.GetTouch(0).position;
                    bool isLeftSide = touchPosition.x < Screen.width / 2f;

                    if (isLeftSide)
                    {
                        JumpLeft();
                    }
                    else
                    {
                        JumpRight();
                    }

                    animator.SetBool("IsJumping", true);
                }
            }

            coinText.text = "COINS: " + coinCount.ToString();
        }

        animator.SetBool("IsSticking", isSticking);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("JumpLeft", isJumpingLeft);
        animator.SetBool("JumpRight", isJumpingRight);

        isJumpingLeft = false;
        isJumpingRight = false;
    }

    void FixedUpdate()
    {
        if (isSticking)
        {
            rb.velocity = Quaternion.Euler(0, 0, stickySurfaceAngle) * Vector2.up * rb.velocity.magnitude;
        }
    }

    void Jump(Vector2 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        hasFirstJumped = true;
    }

    public void JumpLeft()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.left * jumpForce, ForceMode2D.Impulse);
        }
        else if (isSticking)
        {
            rb.AddForce(Quaternion.Euler(0, 0, -stickySurfaceAngle) * Vector2.left * jumpForce, ForceMode2D.Impulse);
        }
        isJumpingLeft = true;
    }

    public void JumpRight()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.right * jumpForce, ForceMode2D.Impulse);
        }
        else if (isSticking)
        {
            rb.AddForce(Quaternion.Euler(0, 0, -stickySurfaceAngle) * Vector2.right * jumpForce, ForceMode2D.Impulse);
        }
        isJumpingRight = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("StickySurface"))
        {
            isSticking = true;
            stickySurfaceAngle = Vector2.SignedAngle(Vector2.up, collision.GetContact(0).normal);
        }
        else if (collision.gameObject.CompareTag("spike"))
        {
            if (isAlive)
            {
                isAlive = false;
                Time.timeScale = 0;
                GameOver();
            }
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {
            coinCount++;
            Destroy(collision.gameObject); // Destroy the collected coin
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
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void GameOver()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        Debug.Log("Game Over");

        // Update highest score if needed and save to PlayerPrefs
        if (coinCount > highestScore)
        {
            highestScore = coinCount;
            PlayerPrefs.SetInt("HighestScore", highestScore);
            highScoreText.text = "HIGHEST: " + highestScore.ToString(); // Update high score text
        }
    }
}
