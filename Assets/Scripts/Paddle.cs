using System.Collections;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float speed = 10f;
    public float limit = 7f;
    public float fuerza = 10.0f;
    private GameManager gameManager;
    public GameObject cristalPrefab;
    public int cantidadBloquesMuro = 15; // Cantidad de bloques para cubrir la línea
    public float espacioEntreBloques = 1.01f; // Ajusta ligeramente para evitar huecos
    public float distanciaDelantePala = 0.5f;


    //cosas powerup redstone
    private bool redstone_powerup;
    public GameObject redstone_powerUpIndicator;

    public GameObject cabeza_pico; //cojo el gameobject yno directamenteelmesh render porque por alguna  razon se borraba
    private Renderer render_diamante_pico; //esto lo hago para coger el render  de solola partede diamante delpico para  cmbiarla de color durante el tiempo quedure el powerup
    private Color color_original;



    private Vector3 initialPaddlePosition;

    private void Start()
    {

        gameManager = FindFirstObjectByType<GameManager>();
        render_diamante_pico = cabeza_pico.GetComponent<Renderer>();
        color_original = render_diamante_pico.material.color;
        initialPaddlePosition = transform.position;
        redstone_powerup = false;

}

void Update()
    {
        float move;
        if (redstone_powerup)
        {
            move = Input.GetAxis("Horizontal") * speed * 2 * Time.deltaTime;
        }
        else
        {
            move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
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
            gameManager.ActivateMultiball();
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
        Destroy(other.gameObject);
    }

    IEnumerator CountDownSeconds()
    {
        yield return new WaitForSeconds(7); //hacemos lo que seria un waitpid o parecido
        redstone_powerup = false; //ponemos a false cuando pasemos los 7 segundos que sera cuando salgamos el wait 
        render_diamante_pico.material.color = Color.white;

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
                    spawnPositionPala.y,
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
}