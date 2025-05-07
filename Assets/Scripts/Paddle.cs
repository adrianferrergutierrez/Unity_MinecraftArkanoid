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

    private Vector3 initialPaddlePosition;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        initialPaddlePosition = transform.position;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector3 newPos = transform.position + new Vector3(move, 0, 0);
        newPos.x = Mathf.Clamp(newPos.x, -limit, limit);
        transform.position = newPos;
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
        Destroy(other.gameObject);
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