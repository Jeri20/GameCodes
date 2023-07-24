using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;
    public float gravityScale = 1f;
    public float raycastDistance = 0.1f;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isSticking = false;
    private float stickySurfaceAngle = 0f;
    private bool isAlive = true;
    private bool isGameStarted = false;
    public bool hasFirstJumped = false;
    private int coinCount;

    public TextMeshProUGUI coinText;

    private Vector2 dragStartPosition;
    private Vector2 dragEndPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        score = 0;
        coinCount = 0;
    }

    void Update()
    {
        if (!isGameStarted)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartGame();
            }
            return;
        }

        if (isAlive)
        {
            isSticking = Mathf.Abs(stickySurfaceAngle) > 0f;

            if (isSticking || (hasFirstJumped && rb.velocity.y == 0))
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        dragStartPosition = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        dragEndPosition = touch.position;
                        CalculateJump();
                    }
                }
            }

            score += Time.deltaTime * 4;
            scoreText.text = "SCORE: " + score.ToString("F");
            coinText.text = "COINS: " + coinCount.ToString();
        }

        animator.SetBool("IsSticking", isSticking);
        animator.SetBool("IsGrounded", rb.velocity.y == 0);
    }

    void FixedUpdate()
    {
        if (isSticking)
        {
            rb.velocity = Quaternion.Euler(0, 0, stickySurfaceAngle) * Vector2.up * rb.velocity.magnitude;
        }
    }

    void CalculateJump()
    {
        Vector2 swipeVector = dragEndPosition - dragStartPosition;
        float swipeDistance = swipeVector.magnitude;
        float calculatedJumpForce = swipeDistance * jumpForce;
        Vector2 jumpDirection = swipeVector.normalized;
        Jump(jumpDirection, calculatedJumpForce);
    }

    void Jump(Vector2 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        hasFirstJumped = true;
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
        else if (collision.gameObject.CompareTag("coin"))
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
    }
}
