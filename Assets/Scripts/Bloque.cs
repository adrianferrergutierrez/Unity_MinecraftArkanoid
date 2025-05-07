using UnityEngine;

public class Bloque : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int vidas = 3;
    public GameObject manzanaPowerUp;//lo enlazamos por unity
    public GameObject cristalPowerUp;
    private float probabilidad_powerup_manzana_de_hoja = 0.33f;
    private float probabilidad_powerup_cristal = 0.5f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota")) {
            vidas--;
            if (vidas == 0)
            {
                float random = Random.value;

                if (gameObject.CompareTag("Hoja"))
                {
                    if (random < probabilidad_powerup_manzana_de_hoja)
                    {
                        Instantiate(manzanaPowerUp, transform.position, manzanaPowerUp.transform.rotation);
                    }

                }
                else if (gameObject.CompareTag("Cristal")) {
                    if (random < probabilidad_powerup_cristal)
                    {
                        Instantiate(cristalPowerUp, transform.position, cristalPowerUp.transform.rotation);
                    }
                }
               // else if (gameObject.CompareTag("Cofre"))//para el cofre  pondremos que de un item 100 x 100, pero uno aleatorio
                //else if(gameObject.CompareTag("Bloque"))//para el resto de bloques que no tienen un powerup especifico

                Destroy(gameObject);
            }
            
        }
     
    }
 
}
