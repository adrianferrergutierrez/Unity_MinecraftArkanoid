using UnityEngine;

//pelota
public class Ball3D : MonoBehaviour
{
    public float launchForce = 500f;
    private Rigidbody rb;
    private bool launched = false;
    private Vector3 ultima_velocidad;
    public float velocidad_actual;
    private float velocidad_minima = 20.0f;
    private AudioSource audio;
    public AudioClip clip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
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
        velocidad_actual = ultima_velocidad.magnitude;
    }

    public void launch()
    {
        rb.AddForce(Vector3.forward * launchForce);

    }

    private void OnCollisionEnter(Collision collision)
    {
       // if (collision.gameObject.CompareTag("Endline"))
        //{
            // Reiniciar escena o vida
          //  UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        //}
       if (collision.gameObject.CompareTag("Pared") || collision.gameObject.CompareTag("Bloque_powerup_random")|| collision.gameObject.CompareTag("Bloque_powerup_especifico")|| collision.gameObject.CompareTag("GlassNoDrop") || collision.gameObject.CompareTag("Pelota"))
        {
            float velocidad = ultima_velocidad.magnitude;
            Vector3 direccion = Vector3.Reflect(ultima_velocidad.normalized, collision.contacts[0].normal);
            rb.linearVelocity = direccion * Mathf.Max(velocidad, 0f); //sacado de este video https://www.youtube.com/watch?v=h84Q0P4VjZY&ab_channel=DevNoob minuto 1:06

            //forma para mantener una velocidad minima, puesta aqui de seguro por si se frena por alguna razon (cuidado con los rigidbody se frena ahi)
            if (rb.linearVelocity.magnitude < velocidad_minima)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * velocidad_minima;
            }
            audio.PlayOneShot(clip);
        }


        
    }

  

  

}
