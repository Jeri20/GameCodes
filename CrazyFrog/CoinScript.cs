using UnityEngine;

public class CoinScript : MonoBehaviour
{   
    public CoinGenerator coinGenerator;
    
    void Update()
    {    
        transform.Translate(Vector2.left * coinGenerator.currentSpeed * Time.deltaTime);
    }                                  
    
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.gameObject.CompareTag("nextLine"))
        {    
            coinGenerator.GenerateNextCoinWithGap();
        }
        else if(collision.gameObject.CompareTag("Finish"))
        {    
            Destroy(this.gameObject);
        }
    }                                   
}
