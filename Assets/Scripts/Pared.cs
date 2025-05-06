using UnityEngine;

public class Pared : MonoBehaviour
{
    public float fuerza = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 direccion = collision.gameObject.transform.position - transform.position;

            Rigidbody player = collision.gameObject.GetComponent<Rigidbody>();
            player.AddForce(fuerza * direccion, ForceMode.Impulse); //impulso para hacer la fuerza de forma inmediata.
        }*/
    }
}
