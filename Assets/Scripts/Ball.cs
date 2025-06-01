using UnityEngine;

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
    private bool estaPegada = false; // Estado principal para saber si esta pegada a la pala
    private bool esBolaInicialSinLanzar = false; // Para la primera bola del nivel

    void Awake()
    {
        
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }
    void Start()
    {
        
        pelotaRender = gameObject.GetComponent<Renderer>();
        textura_inicial = pelotaRender.material.mainTexture;
    }

    void Update()
    {
     
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
      
        if ((collision.gameObject.CompareTag("Pared") || collision.gameObject.CompareTag("Bloque_powerup_random") || collision.gameObject.CompareTag("Coral_debuff") || collision.gameObject.CompareTag("Bloque_powerup_especifico") || collision.gameObject.CompareTag("Bloque_nodrop") || collision.gameObject.CompareTag("Wither") || collision.gameObject.CompareTag("Pelota")))
        {
            float velocidad = ultima_velocidad.magnitude;
            Vector3 direccion = Vector3.Reflect(ultima_velocidad.normalized, collision.contacts[0].normal);
            rb.linearVelocity = direccion * Mathf.Max(velocidad, 0f); 

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


    public void LanzarComoNuevaBola(Vector3 direccionDeLanzamiento)
    {
        // Este método ahora solo se usa para lanzar una bola desde la pala
        // o en situaciones donde se necesite una dirección específica.
        transform.parent = null;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direccionDeLanzamiento * launchForce, ForceMode.Impulse);

        estaPegada = false;
        esBolaInicialSinLanzar = false;
    }
    public void LaunchAsDuplicate(Vector3 originalVelocity)
    {
        // Este método es específicamente para las bolas duplicadas.
        transform.parent = null;
        rb.isKinematic = false; // Aseguramos que las físicas estén activas.

        // Para que no sea un clon perfecto, añadimos una pequeña variación a la dirección.
        Vector3 slightVariation = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0).normalized * 1.5f;

        // Asignamos la velocidad de la bola original + nuestra pequeña variación.
        rb.linearVelocity = originalVelocity + slightVariation;

        // Aseguramos que la nueva bola no tenga una velocidad demasiado baja.
        if (rb.linearVelocity.magnitude < velocidad_minima)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velocidad_minima;
        }

        estaPegada = false;
        esBolaInicialSinLanzar = false;
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

 






