using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float speed = 10f;
    public float limit = 7f;
    public float fuerza = 10.0f;
    
    public GameObject cristalPrefab;
    public GameObject cristalMoradoPrefab;
    public GameObject bedrock_Prefab;
    public int cantidadBloquesMuro = 15; // Cantidad de bloques para cubrir la línea
    public float espacioEntreBloques = 1.01f; // Ajusta ligeramente para evitar huecos
    public float distanciaDelantePala = 0.5f;

    private ManagerScene manager_escena;

    //cosas powerup redstone
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

    //Powerup cuarzo, el funcionamiento del power up es que se va creando un muro de cuarzo en posiciones donde no se ha creado un muro antes
    private int index_posicion_muro = 0;
    public GameObject cuarzo_prefab;

    private Vector3 initialPaddlePosition;

    private Ball3D bolaPegada = null; // Referencia a la bola que está pegada
    public Vector3 offsetBolaPegada = new Vector3(0f, 1.35f, -4.3f);


    public float enlargeDuration = 10.0f;

    public float enlargeFactor = 1.5f;
    private bool powerup_hacerse_grande = false;
    private Vector3 EscalaOriginal; //loponemos para el powerup de hacerse grande para pioder volver al tamaño anterior

    private void Start()
    {
        manager_escena = FindFirstObjectByType<ManagerScene>();

        render_diamante_pico = cabeza_pico.GetComponent<Renderer>();
        color_original = render_diamante_pico.material.color;
        initialPaddlePosition = transform.position;
        redstone_powerup = false;
        muro_god_mode = new GameObject[cantidadBloquesMuro];
        EscalaOriginal = transform.localScale;

    }

void Update()
    {
        Debug.Log("Puntuación actual: " + GameManager.instance.puntuacion);

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
        //redstone_powerUpIndicator.transform.position = transform.position + new Vector3(0.0f, -0.5f, 0.0f);

    }
    public void ActivateEnlarge()
    {
        if (!powerup_hacerse_grande) 
        {
            transform.localScale = new Vector3(EscalaOriginal.x * enlargeFactor, EscalaOriginal.y, EscalaOriginal.z);
            powerup_hacerse_grande = true;
            StartCoroutine(RevertPaddleSizeCoroutine());
        }
        else // Si ya está agrandada, quizás solo reiniciamos el temporizador
        {
            StopCoroutine("RevertPaddleSizeCoroutine"); // Detiene la corrutina anterior
            StartCoroutine(RevertPaddleSizeCoroutine()); // Inicia una nueva
        }
    }

    IEnumerator RevertPaddleSizeCoroutine()
    {
        yield return new WaitForSeconds(enlargeDuration);
        transform.localScale = EscalaOriginal; // Vuelve a la escala original
        powerup_hacerse_grande = false;
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
            //el powerup indicator deberiamos ponerlo en true aqui
            render_diamante_pico.material.color = Color.red;

            StartCoroutine(CountDownSeconds());


        }
        else if (other.gameObject.CompareTag("Powerup_oro"))
        {
            oro_powerup = true;

            //ponemos nueva textura
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
            StartCoroutine(CuentaAtrasIman(10.0f)); // Le pasamos la duración
        }
        else if (other.gameObject.CompareTag("Faro_powerup"))
        {
            GameManager.instance.setVidasPlayer(GameManager.instance.GetVidasPlayer() + 1);
        }
        else if (other.gameObject.CompareTag("Experiencia"))
        {
            manager_escena.acabarNivel();
        }
        else if (other.gameObject.CompareTag("Diamante_powerup")) {
            ActivateEnlarge();
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
                    // Llamamos a nuestro nuevo método unificado
                    PegarBola(ballScript);
                }
            }
        }
    }

    // MÉTODO PARA EL SPAWN INICIAL (LLAMADO POR GAMEMANAGER)
    public void AsignarBola(Ball3D bola)
    {
        // Llamamos al mismo método unificado para asegurar consistencia
        PegarBola(bola);
    }

    // NUEVO MÉTODO PRIVADO Y UNIFICADO PARA PEGAR LA BOLA
    private void PegarBola(Ball3D bola)
    {
        if (bola == null || bola.EstaPegada()) return; // Si no hay bola o ya está pegada, no hacemos nada

        bolaPegada = bola; // Guardamos la referencia a nuestra nueva bola pegada

        // Le decimos a la bola que se pegue usando NUESTRO offset
        bola.PegarseALaPala(this.transform, offsetBolaPegada);
    }

    IEnumerator CountDownSeconds()
    {
        yield return new WaitForSeconds(7); //hacemos lo que seria un waitpid o parecido
        redstone_powerup = false; //ponemos a false cuando pasemos los 7 segundos que sera cuando salgamos el wait 
        render_diamante_pico.material.color = Color.white;

        // powerUpIndicator.SetActive(false); //hacemos que el indiicador se ponga en desactivado para no verlo

    }


    IEnumerator CountDownSecondsOro()
    {
        yield return new WaitForSeconds(7); //hacemos lo que seria un waitpid o parecido
        oro_powerup = false; //ponemos a false cuando pasemos los 7 segundos que sera cuando salgamos el wait 
                             //quitamos la textura
        render_diamante_pico.material.mainTexture = powerUpTextures[1];
        GameManager.instance.change_oro_state(false);
        // powerUpIndicator.SetActive(false); //hacemos que el indiicador se ponga en desactivado para no verlo

    }

    IEnumerator CountDownSecondsMagma()
    {
        yield return new WaitForSeconds(5); //hacemos lo que seria un waitpid o parecido
        GameManager.instance.powerball_change_state(false);

        // powerUpIndicator.SetActive(false); //hacemos que el indiicador se ponga en desactivado para no verlo

    }
 


    private void crearMuro(GameObject cristal)
    {
        if (cristalPrefab != null)
        {
            Vector3 spawnPositionPala = initialPaddlePosition;
            Vector3 direccionMuro = Vector3.forward;

            // Calcula la posición inicial del primer bloque del muro
            // Asumimos que queremos cubrir desde -limit hasta +limit en el eje X
            float inicioX = -limit;

            for (int i = 0; i < cantidadBloquesMuro; i++)
            {
                // Calcula la posición de cada bloque a lo largo del eje X
                Vector3 posicionBloque = new Vector3(
                    inicioX + i * espacioEntreBloques,
                    spawnPositionPala.y*1.1f + 1.0f,
                    spawnPositionPala.z + distanciaDelantePala // Ajusta la profundidad si es necesario
                );
                muro_god_mode[i] = Instantiate(cristal, posicionBloque, Quaternion.identity);

                // Incrementa inicioX para el siguiente bloque (esto podría causar superposición)
                // Una mejor manera es calcular la posición directamente usando el índice y el espacio
                // inicioX += espacioEntreBloques; // No es necesario aquí
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

        // Si al acabarse el tiempo todavía hay una bola pegada, la lanzamos
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

            // Calcula la posición inicial del primer bloque del muro
            // Asumimos que queremos cubrir desde -limit hasta +limit en el eje X
            float inicioX = -limit;

            for (int i = 0; i < cantidadBloquesMuro; i++)
            {
                // Calcula la posición de cada bloque a lo largo del eje X
                Vector3 posicionBloque = new Vector3(
                    inicioX + i * espacioEntreBloques,
                    spawnPositionPala.y * 1.1f + 1.0f,
                    spawnPositionPala.z + distanciaDelantePala // Ajusta la profundidad si es necesario
                );
                if (god_mode) muro_god_mode[i] = Instantiate(bedrock_Prefab, posicionBloque, Quaternion.identity);
                else Destroy(muro_god_mode[i]);

                // Incrementa inicioX para el siguiente bloque (esto podría causar superposición)
                // Una mejor manera es calcular la posición directamente usando el índice y el espacio
                // inicioX += espacioEntreBloques; // No es necesario aquí
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