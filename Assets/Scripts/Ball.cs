using UnityEngine;

//pelota
public class Ball3D : MonoBehaviour
{
    public float launchForce = 500f;
    private Rigidbody rb;
    private bool launched = false;
    private Vector3 ultima_velocidad;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.forward * launchForce);
            launched = true;
        }
        if(Input.GetKeyDown(KeyCode.R)) transform.position = new Vector3(0, 10, 0);
        ultima_velocidad = rb.linearVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Endline"))
        {
            // Reiniciar escena o vida
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else if (collision.gameObject.CompareTag("Pared") ||collision.gameObject.CompareTag("Pala") || collision.gameObject.CompareTag("Bloque")){ 
            float velocidad = ultima_velocidad.magnitude;
            Vector3 direccion = Vector3.Reflect(ultima_velocidad.normalized, collision.contacts[0].normal);
            rb.linearVelocity = direccion * Mathf.Max(velocidad, 0f); //sacado de este video https://www.youtube.com/watch?v=h84Q0P4VjZY&ab_channel=DevNoob minuto 1:06
        }
    }
    
}
