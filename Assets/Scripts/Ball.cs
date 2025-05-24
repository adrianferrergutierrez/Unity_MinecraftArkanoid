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



    private bool estaPegada = false; // Estado principal para saber si est� pegada a la pala
    private bool esBolaInicialSinLanzar = false; // Para la primera bola del nivel

    void Awake()
    {
        // Awake() se llama inmediatamente cuando se crea el objeto, ANTES que Start().
        // Este es el lugar perfecto para obtener referencias a otros componentes del mismo objeto.
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }
    void Start()
    {
        
        pelotaRender = gameObject.GetComponent<Renderer>();
        //guardamos la texutra inicial 
        textura_inicial = pelotaRender.material.mainTexture;
    }

    void Update()
    {
        // IMPORTANTE: Se elimina el Input.GetKeyDown(KeyCode.Space) de aqu�.
        // La pala se encargar� ahora de la l�gica de lanzamiento.
        if (!estaPegada)
        {
            ultima_velocidad = rb.linearVelocity;
            velocidad_actual = ultima_velocidad.magnitude;
        }
    }

    public void PegarseALaPala(Transform palaTransform, Vector3 offsetLocal)
    {
        if (estaPegada) return;

        transform.parent = palaTransform;
        rb.isKinematic = true;
        transform.localPosition = offsetLocal; // Posición LOCAL respecto a la pala
        estaPegada = true;
    }

    public void LanzarDesdePala(Vector3 direccionDeLanzamiento)
    {
        if (!estaPegada) return; // No se puede lanzar si no está pegada

        transform.parent = null; // La bola deja de ser hija
        rb.isKinematic = false; // Reactivamos las físicas
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direccionDeLanzamiento * launchForce);

        estaPegada = false;
        esBolaInicialSinLanzar = false;
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
       
        
    }
    public bool EstaPegada()
    {
        return estaPegada;
    }



    public bool get_state_powerball() {
        return power_ball;
    
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

    public void ConfigurarEstadoInicial(bool esInicial)
    {
        esBolaInicialSinLanzar = esInicial;
        if (esBolaInicialSinLanzar)
        {
            estaPegada = true;
            rb.isKinematic = true; // La bola inicial empieza sin físicas, esperando el lanzamiento
        }
    }
}

 






