using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float speed = 10f;
    public float limit = 7f;
    public float fuerza = 10.0f;
    
    public GameObject cristalPrefab;
    public int cantidadBloquesMuro = 15; // Cantidad de bloques para cubrir la línea
    public float espacioEntreBloques = 1.01f; // Ajusta ligeramente para evitar huecos
    public float distanciaDelantePala = 0.5f;


    //cosas powerup redstone
    private bool redstone_powerup;
    private bool oro_powerup;

    public GameObject redstone_powerUpIndicator;

    public GameObject cabeza_pico; //cojo el gameobject yno directamenteelmesh render porque por alguna  razon se borraba
    private Renderer render_diamante_pico; //esto lo hago para coger el render  de solola partede diamante delpico para  cmbiarla de color durante el tiempo quedure el powerup
    private Color color_original;


    //el indice 0 es la nueva, y la 1 la vieja
    public Texture[] powerUpTextures;



    //Powerup cuarzo, el funcionamiento del power up es que se va creando un muro de cuarzo en posiciones donde no se ha creado un muro antes
    private int index_posicion_muro = 0;
    public GameObject cuarzo_prefab;

    private Vector3 initialPaddlePosition;

    private void Start()
    {

        render_diamante_pico = cabeza_pico.GetComponent<Renderer>();
        color_original = render_diamante_pico.material.color;
        initialPaddlePosition = transform.position;
        redstone_powerup = false;

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

        if (oro_powerup) { 
        
        }
        Vector3 newPos = transform.position + new Vector3(move, 0, 0);
        newPos.x = Mathf.Clamp(newPos.x, -limit, limit);
        transform.position = newPos;
        //redstone_powerUpIndicator.transform.position = transform.position + new Vector3(0.0f, -0.5f, 0.0f);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            Vector3 direccion = collision.gameObject.transform.position - transform.position;
            Rigidbody player = collision.gameObject.GetComponent<Rigidbody>();
            player.AddForce(fuerza * direccion, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Manzana"))
        {
            GameManager.instance.ActivateMultiball();
            
        }
        else if (other.gameObject.CompareTag("CristalPowerUp"))
        {
            crearMuro();
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
        else if (other.gameObject.CompareTag("Powerup_magma")) {
            GameManager.instance.powerball_change_state(true);
            StartCoroutine(CountDownSecondsMagma());
        }
        else if (other.gameObject.CompareTag("Powerup_cuarzo"))
        {
            crearCuarzoMuro();
        }


        Destroy(other.gameObject);
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


    private void crearMuro()
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
                    spawnPositionPala.y*1.1f,
                    spawnPositionPala.z + distanciaDelantePala // Ajusta la profundidad si es necesario
                );
                Instantiate(cristalPrefab, posicionBloque, Quaternion.identity);

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