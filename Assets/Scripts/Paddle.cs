using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float speed = 10f;
    public float limit = 7f;
    public float fuerza = 10.0f;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

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
        if (collision.gameObject.CompareTag("Pelota")) {
            Vector3 direccion = collision.gameObject.transform.position - transform.position;

            Rigidbody player = collision.gameObject.GetComponent<Rigidbody>();
            player.AddForce(fuerza * direccion, ForceMode.Impulse); //impulso para hacer la fuerza de forma inmediata.
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //aplicamos powerup manzana
        if (other.gameObject.CompareTag("Manzana"))
        {
            gameManager.ActivateMultiball();
        }
        Destroy(other.gameObject);
    }

    //las  corutinas seran para los powerups que requieran de un tiempo , este de multibola es instantaneo
    /*  IEnumerator CountDownSecondsManzana()
      {
          yield return new WaitForSeconds(7); //hacemos una espera de x segundos
          tienep = false; //ponemos a false cuando pasemos los 7 segundos que sera cuando salgamos el wait 
          powerUpIndicator.SetActive(false); //hacemos que el indiicador se ponga en desactivado para no verlo

      }
    */
}
