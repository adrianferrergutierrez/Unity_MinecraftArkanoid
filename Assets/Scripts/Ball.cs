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


    private bool power_ball = false;
    public Texture textura_powerball;
    private Texture textura_inicial;
    private Renderer pelotaRender; //esto lo hago para coger el render  de solola partede diamante delpico para  cmbiarla de color durante el tiempo quedure el powerup
    private bool Iman = false;
    private bool Inicio = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();

        pelotaRender = gameObject.GetComponent<Renderer>();
        //guardamos la texutra inicial 
        textura_inicial = pelotaRender.material.mainTexture;
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Space) && (Iman||Inicio))
        {
            rb.AddForce(Vector3.forward * launchForce);
            launched = true;
        }



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
        if ((collision.gameObject.CompareTag("Pared") || collision.gameObject.CompareTag("Bloque_powerup_random") || collision.gameObject.CompareTag("Coral_debuff") || collision.gameObject.CompareTag("Bloque_powerup_especifico") || collision.gameObject.CompareTag("Bloque_nodrop") || collision.gameObject.CompareTag("Wither") || collision.gameObject.CompareTag("Pelota")))
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
        else if (collision.gameObject.CompareTag("Pala") && Iman) {
            transform.position = collision.gameObject.transform.position + new Vector3(0.0f,1.0f,2.5f);
        }


        
    }
    

    public bool get_state_powerball() {
        return power_ball;
    
    }

    public void Inicio_state(bool estado)
    {
        Inicio = estado;
    }

    public void change_powerball_state(bool estado) {
        power_ball = estado;
        if (estado) {
            pelotaRender.material.mainTexture = textura_powerball;
            pelotaRender.material.color = Color.white;
        }

        else
        {
            pelotaRender.material.mainTexture = textura_inicial;
            pelotaRender.material.color = Color.red;

        }

    }

    public void toggleIman(bool iman)
    {
        Iman = iman;
    }
    }

 






