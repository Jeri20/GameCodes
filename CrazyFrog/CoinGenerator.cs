using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public GameObject coin;
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;
    public float speedMultiplier;

    private PlayerController playerController;
    private bool hasGeneratedFirstCoin = false;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        currentSpeed = 0f;
    }

    void Update()
    {
        if (playerController.hasFirstJumped && !hasGeneratedFirstCoin)
        {
            currentSpeed = minSpeed;
            GenerateCoin();
            hasGeneratedFirstCoin = true;
        }

        if (currentSpeed < maxSpeed)
        {
            currentSpeed += 0.05f* speedMultiplier * Time.deltaTime;
        }
    }

    public void GenerateNextCoinWithGap()
    {
        float randomWait = Random.Range(0.5f, 1.2f);
        Invoke("GenerateCoin", randomWait);
    }

    void GenerateCoin()
    {
        GameObject coinIns = Instantiate(coin, transform.position, transform.rotation);
        coinIns.GetComponent<CoinScript>().coinGenerator = this;
    }
}
