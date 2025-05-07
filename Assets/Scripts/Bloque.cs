using UnityEngine;

public class Bloque : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int vidas = 3;
    public GameObject manzanaPowerUp;//lo enlazamos por unity
    private float probabilidad_powerup_manzana_de_hoja = 0.33f;
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
                if (gameObject.CompareTag("Hoja")) {
                    float random = Random.value;
                    if (random < probabilidad_powerup_manzana_de_hoja) {
                        Instantiate(manzanaPowerUp, transform.position, manzanaPowerUp.transform.rotation);
                     }

                }
                Destroy(gameObject);
            }
            
        }
     
    }
 
}
