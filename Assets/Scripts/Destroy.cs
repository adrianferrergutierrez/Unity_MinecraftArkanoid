using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameManager gameManager;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota") && gameObject.CompareTag("Destroy"))
        {
            gameManager.destruccion_bola(collision.gameObject);
        }
        else {
            Destroy(collision.gameObject);
        }
    }
}
