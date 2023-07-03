using UnityEngine;

public class SpikeGenerator : MonoBehaviour
{
    public GameObject spike;
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;
    public float speedMultiplier;

    private PlayerController playerController;
    private bool hasGeneratedFirstSpike = false;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        currentSpeed = 0f;
    }

    void Update()
    {
        if (playerController.hasFirstJumped && !hasGeneratedFirstSpike)
        {
            currentSpeed = minSpeed;
            GenerateSpike();
            hasGeneratedFirstSpike = true;
        }

        if (currentSpeed < maxSpeed)
        {
            currentSpeed += 0.05f* speedMultiplier * Time.deltaTime;
        }
    }

    public void GenerateNextSpikeWithGap()
    {
        float randomWait = Random.Range(0.5f, 1.2f);
        Invoke("GenerateSpike", randomWait);
    }

    void GenerateSpike()
    {
        GameObject spikeIns = Instantiate(spike, transform.position, transform.rotation);
        spikeIns.GetComponent<SpikeScript>().spikeGenerator = this;
    }
}
