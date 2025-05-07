using UnityEngine;

public class Bloque : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int vidas = 3;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota")) {
            vidas--;
            if (vidas == 0) Destroy(gameObject);
        }
     
    }
 
}
