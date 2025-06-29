using System.Collections;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float speed = 20f;
    public float limit = 7f;
    public float fuerza = 10.0f;
    public GameObject cristalPrefab;
    public GameObject cristalMoradoPrefab;
    public GameObject bedrock_Prefab;
    public int cantidadBloquesMuro = 15; // Cantidad de bloques para cubrir la l�nea
    public float espacioEntreBloques = 1.01f; // Ajusta ligeramente para evitar huecos
    public float distanciaDelantePala = 0.5f;
    private ManagerScene manager_escena;
    private bool redstone_powerup;
    private bool oro_powerup;
    public GameObject redstone_powerUpIndicator;
    public GameObject cabeza_pico; //cojo el gameobject yno directamenteelmesh render porque por alguna  razon se borraba
    private Renderer render_diamante_pico; //esto lo hago para coger el render  de solola partede diamante delpico para  cmbiarla de color durante el tiempo quedure el powerup
    private Color color_original;
    //el indice 0 es la nueva, y la 1 la vieja
    public Texture[] powerUpTextures;
    private bool god_mode = false;
    private GameObject[] muro_god_mode;
    private int index_posicion_muro = 0;
    public GameObject cuarzo_prefab;
    private Vector3 initialPaddlePosition;
    private Ball3D bolaPegada = null; 
    public Vector3 offsetBolaPegada = new Vector3(0f, 1.35f, -4.3f);
    public Transform visualesPala; 
    public float enlargeDuration = 10.0f;
    public float enlargeFactor = 1.5f;
    public float shrinkFactor = 0.5f;
    private bool powerup_hacerse_grande = false;
    private bool powerup_hacerse_pequena = false;
    private Vector3 escalaVisualOriginal;
    private Vector3 tamanoColliderOriginal;
    private BoxCollider paddleCollider;




    private void Start()
    {
        manager_escena = FindFirstObjectByType<ManagerScene>();

        render_diamante_pico = cabeza_pico.GetComponent<Renderer>();
        color_original = render_diamante_pico.material.color;
        initialPaddlePosition = transform.position;
        redstone_powerup = false;
        muro_god_mode = new GameObject[cantidadBloquesMuro];
        if (visualesPala != null)
        {
            escalaVisualOriginal = visualesPala.localScale;
        }
        paddleCollider = GetComponent<BoxCollider>();
        if (paddleCollider != null)
        {
            tamanoColliderOriginal = paddleCollider.size;
        }
    }

void Update()
    {
        Debug.Log("Puntuaci�n actual: " + GameManager.instance.puntuacion);

        float move;
        if (redstone_powerup)
        {
            move = Input.GetAxis("Horizontal") * speed * 2 * Time.deltaTime;
        }
        else
        {
            move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            crearMuroGodMode();
            god_mode = !god_mode;
        }
        if (bolaPegada != null && Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.MarcarPrimeraBolaLanzada(); // Avisamos al GM
            bolaPegada.LanzarDesdePala(Vector3.forward); // Lanzamos hacia adelante
            bolaPegada = null; // Ya no hay bola pegada
        }

        Vector3 newPos = transform.position + new Vector3(move, 0, 0);
        newPos.x = Mathf.Clamp(newPos.x, -limit, limit);
        transform.position = newPos;

    }
    public void ActivateEnlarge()
    {
        if (visualesPala == null || paddleCollider == null)
        {
            Debug.LogError("No se han asignado las referencias de 'visualesPala' o el BoxCollider.");
            return;
        }

        // Si estaba encogida, desactivamos ese efecto
        if (powerup_hacerse_pequena)
        {
            StopCoroutine("RevertShrinkCoroutine");
            powerup_hacerse_pequena = false;
        }

        // Aplicamos directamente el tama�o grande
        visualesPala.localScale = new Vector3(escalaVisualOriginal.x * enlargeFactor, escalaVisualOriginal.y, escalaVisualOriginal.z);
        paddleCollider.size = new Vector3(tamanoColliderOriginal.x * enlargeFactor, tamanoColliderOriginal.y, tamanoColliderOriginal.z);
        powerup_hacerse_grande = true;

        StopCoroutine("RevertPaddleSizeCoroutine");
        StartCoroutine("RevertPaddleSizeCoroutine");
    }

    public void ActivateShrink()
    {
        if (visualesPala == null || paddleCollider == null)
        {
            Debug.LogError("No se han asignado las referencias de 'visualesPala' o el BoxCollider.");
            return;
        }

        // Si estaba agrandada, desactivamos ese efecto
        if (powerup_hacerse_grande)
        {
            StopCoroutine("RevertPaddleSizeCoroutine");
            powerup_hacerse_grande = false;
        }

        // Aplicamos directamente el tama�o peque�o
        visualesPala.localScale = new Vector3(escalaVisualOriginal.x * shrinkFactor, escalaVisualOriginal.y, escalaVisualOriginal.z);
        paddleCollider.size = new Vector3(tamanoColliderOriginal.x * shrinkFactor, tamanoColliderOriginal.y, tamanoColliderOriginal.z);
        powerup_hacerse_pequena = true;

        StopCoroutine("RevertPaddleSizeCoroutine");
        StartCoroutine("RevertPaddleSizeCoroutine");
    }

    IEnumerator RevertPaddleSizeCoroutine()
    {
        yield return new WaitForSeconds(enlargeDuration);

        // Volvemos a los tama�os y escalas originales
        if (visualesPala != null)
        {
            visualesPala.localScale = escalaVisualOriginal;
        }
        if (paddleCollider != null)
        {
            paddleCollider.size = tamanoColliderOriginal;
        }

        powerup_hacerse_grande = false;
        powerup_hacerse_pequena = false; // Aseguramos que ambos estados se resetean
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Manzana"))
        {
            Destroy(other.gameObject);

            GameManager.instance.ActivateMultiball();


        }
        else if (other.gameObject.CompareTag("CristalPowerUp"))
        {
            crearMuro(cristalPrefab);
        }
        else if (other.gameObject.CompareTag("Redstonepowerup"))
        {
            redstone_powerup = true;
            render_diamante_pico.material.color = Color.red;
            StartCoroutine(CountDownSeconds());
        }
        else if (other.gameObject.CompareTag("Powerup_oro"))
        {
            oro_powerup = true;
            //nueva textura
            render_diamante_pico.material.mainTexture = powerUpTextures[0];
            GameManager.instance.change_oro_state(true);
            StartCoroutine(CountDownSecondsOro());

        }
        else if (other.gameObject.CompareTag("Powerup_magma"))
        {
            GameManager.instance.powerball_change_state(true);
            StartCoroutine(CountDownSecondsMagma());
        }
        else if (other.gameObject.CompareTag("Powerup_cuarzo"))
        {
            crearCuarzoMuro();
        }
        else if (other.gameObject.CompareTag("Purple_glass_powerup"))
        {
            crearMuro(cristalMoradoPrefab);
        }
        else if (other.gameObject.CompareTag("Hierro_powerup"))
        {
            GameManager.instance.ActivarPowerUpIman(true);
            render_diamante_pico.material.mainTexture = powerUpTextures[2];
            StartCoroutine(CuentaAtrasIman(10.0f)); // Le pasamos la duraci�n
        }
        else if (other.gameObject.CompareTag("Faro_powerup"))
        {
            GameManager.instance.setVidasPlayer(GameManager.instance.GetVidasPlayer() + 1);
        }
        else if (other.gameObject.CompareTag("Experiencia"))
        {
            manager_escena.acabarNivel();
        }
        else if (other.gameObject.CompareTag("Diamante_powerup"))
        {
            ActivateEnlarge();
        }
        else if (other.gameObject.CompareTag("Lapis")) { 
            for (int i = 0; i < 5;++i) manager_escena.RegistrarBloqueDestruido(); //sumamos en 5 el numero de bloques desturidos, mejorando la experiencia!!
        }
        else if (other.gameObject.CompareTag("Powerup_shrink"))
        {
            ActivateShrink();
        }


        Destroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.instance.EstaImanActivo() && collision.gameObject.CompareTag("Pelota"))
        {
            // Solo intentamos pegar una bola si no tenemos ya una.
            if (bolaPegada == null)
            {
                Ball3D ballScript = collision.gameObject.GetComponent<Ball3D>();
                if (ballScript != null)
                {
                    // Llamamos a nuestro nuevo m�todo unificado
                    PegarBola(ballScript);
                }
            }
        }
    }

    // M�TODO PARA EL SPAWN INICIAL (LLAMADO POR GAMEMANAGER)
    public void AsignarBola(Ball3D bola)
    {
        // Llamamos al mismo m�todo unificado para asegurar consistencia
        PegarBola(bola);
    }

    // NUEVO M�TODO PRIVADO Y UNIFICADO PARA PEGAR LA BOLA
    private void PegarBola(Ball3D bola)
    {
        if (bola == null || bola.EstaPegada()) return; // Si no hay bola o ya est� pegada, no hacemos nada
        bolaPegada = bola; // Guardamos la referencia a nuestra nueva bola pegada
        // Le decimos a la bola que se pegue usando NUESTRO offset
        bola.PegarseALaPala(this.transform, offsetBolaPegada);
    }

    IEnumerator CountDownSeconds()
    {
        yield return new WaitForSeconds(7); //hacemos lo que seria un waitpid o parecido
        redstone_powerup = false; //ponemos a false cuando pasemos los 7 segundos que sera cuando salgamos el wait 
        render_diamante_pico.material.color = Color.white;
    }


    IEnumerator CountDownSecondsOro()
    {
        yield return new WaitForSeconds(7); 
        oro_powerup = false;
        render_diamante_pico.material.mainTexture = powerUpTextures[1];
        GameManager.instance.change_oro_state(false);

    }

    IEnumerator CountDownSecondsMagma()
    {
        yield return new WaitForSeconds(5); //hacemos lo que seria un waitpid o parecido
        GameManager.instance.powerball_change_state(false);
    }
 


    private void crearMuro(GameObject cristal)
    {
        if (cristalPrefab != null)
        {
            Vector3 spawnPositionPala = initialPaddlePosition;
            Vector3 direccionMuro = Vector3.forward;

            // Calcula la posici�n inicial del primer bloque del muro
            // Asumimos que queremos cubrir desde -limit hasta +limit en el eje X
            float inicioX = -limit;

            for (int i = 0; i < cantidadBloquesMuro; i++)
            {
                // Calcula la posici�n de cada bloque a lo largo del eje X
                Vector3 posicionBloque = new Vector3(
                    inicioX + i * espacioEntreBloques,
                    spawnPositionPala.y*1.1f + 1.0f,
                    spawnPositionPala.z + distanciaDelantePala // Ajusta la profundidad si es necesario
                );
                muro_god_mode[i] = Instantiate(cristal, posicionBloque, Quaternion.identity);

                // Incrementa inicioX para el siguiente bloque (esto podr�a causar superposici�n)
                // Una mejor manera es calcular la posici�n directamente usando el �ndice y el espacio
                // inicioX += espacioEntreBloques; // No es necesario aqu�
            }
        }
        else
        {
            Debug.LogError("No se ha asignado el Prefab del bloque de cristal al Paddle.");
        }
    }

    IEnumerator CuentaAtrasIman(float duracion)
    {
        yield return new WaitForSeconds(duracion);

        GameManager.instance.ActivarPowerUpIman(false);
        render_diamante_pico.material.mainTexture = powerUpTextures[1]; // o la textura original

        // Si al acabarse el tiempo todav�a hay una bola pegada, la lanzamos
        if (bolaPegada != null)
        {
            bolaPegada.LanzarDesdePala(transform.forward);
            bolaPegada = null;
        }
    }


    private void crearMuroGodMode()
    {
        if (bedrock_Prefab != null)
        {
            Vector3 spawnPositionPala = initialPaddlePosition;
            Vector3 direccionMuro = Vector3.forward;

            // Calcula la posici�n inicial del primer bloque del muro
            // Asumimos que queremos cubrir desde -limit hasta +limit en el eje X
            float inicioX = -limit;

            for (int i = 0; i < cantidadBloquesMuro; i++)
            {
                // Calcula la posici�n de cada bloque a lo largo del eje X
                Vector3 posicionBloque = new Vector3(
                    inicioX + i * espacioEntreBloques,
                    spawnPositionPala.y * 1.1f + 1.0f,
                    spawnPositionPala.z + distanciaDelantePala // Ajusta la profundidad si es necesario
                );
                if (god_mode) muro_god_mode[i] = Instantiate(bedrock_Prefab, posicionBloque, Quaternion.identity);
                else Destroy(muro_god_mode[i]);

            }
        }
        else
        {
            Debug.LogError("No se ha asignado el Prefab del bloque de cristal al Paddle.");
        }
    }



    private void crearCuarzoMuro()
    {
       
            if (cuarzo_prefab != null)
            {
                Vector3 spawnPositionPala = initialPaddlePosition;
                Vector3 direccionMuro = Vector3.forward;
                float inicioX = -limit;
                Vector3 posicionBloque = new Vector3(
                        inicioX + index_posicion_muro * espacioEntreBloques,
                        spawnPositionPala.y + 1.1f,
                        spawnPositionPala.z + distanciaDelantePala // Ajusta la profundidad si es necesario
                    );
                Instantiate(cuarzo_prefab, posicionBloque, Quaternion.identity);
                ++index_posicion_muro;
            if (index_posicion_muro >= cantidadBloquesMuro) index_posicion_muro = 0; //reiniciamos la sequencia

                 }
            
 
    }
}