using UnityEngine;

public class Destroy : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota") && gameObject.CompareTag("Destroy"))
        {
            GameManager.instance.HandleBallLost(collision.gameObject);
        }
      
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }



}
