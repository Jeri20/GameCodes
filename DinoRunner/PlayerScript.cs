using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public float JumpForce;
    private float score;

    [SerializeField]
    private bool isGrounded = false;
    private bool isAlive = true;
    private bool isGameEnded = false;

    private Rigidbody2D RB;
    public Text ScoreTxt;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        score = 0;
    }

    private void Update()
    {
        if (isGameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Restart the game
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return; // Don't execute further code if the game has ended
        }

        if (isAlive)
        {
            // Check for jump input on Android
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began && isGrounded)
                {
                    RB.AddForce(Vector2.up * JumpForce);
                    isGrounded = false;
                }
            }

            score += Time.deltaTime * 4;
            ScoreTxt.text = "SCORE: " + score.ToString("F");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            if (!isGrounded)
            {
                isGrounded = true;
            }
        }
        else if (collision.gameObject.CompareTag("spike"))
        {
            if (!isGameEnded)
            {
                isAlive = false;
                Time.timeScale = 0;
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        // Disable player movement
        RB.velocity = Vector2.zero;
        RB.gravityScale = 0;

        // Disable player input or show game-over UI
        // Implement your game-over logic here
        Debug.Log("Game Over");

        // Set game-ended flag
        isGameEnded = true;
    }
}
