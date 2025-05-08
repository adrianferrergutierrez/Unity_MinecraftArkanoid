using UnityEngine;

public class Bloque : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int vidas = 3;
 

    public GameObject[] powerups;
    //power up 0 -> manzana
    //power up 1 -> cristal
    //power up 2-> redstone


    // Probabilidades
    private float probabilidad_powerup_manzana_de_hoja = 0.33f;
    private float probabilidad_powerup_cristal = 0.5f;
    private float probabilidad_powerup_redstone = 1f; // Redstone siempre suelta

    private float probabilidad_drop_bloque = 0.1f; // 10%
    private float probabilidad_drop_cofre = 1.0f;  // 100%

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            vidas--;

            if (vidas == 0)
            {
                float random = Random.value;

                if (CompareTag("Hoja") && random < probabilidad_powerup_manzana_de_hoja)
                {
                    InstanciarPowerUp(0); // manzana
                }
                else if (CompareTag("Cristal") && random < probabilidad_powerup_cristal)
                {
                    InstanciarPowerUp(1); // cristal
                }
                else if (CompareTag("Redstone") && random < probabilidad_powerup_redstone)
                {
                    InstanciarPowerUp(2); // redstone
                }
                else if (CompareTag("Cofre") && Random.value < probabilidad_drop_cofre)
                {
                    InstanciarPowerUpAleatorio();
                }
                else if (CompareTag("Bloque") && Random.value < probabilidad_drop_bloque)
                {
                    InstanciarPowerUpAleatorio();
                }

                Destroy(gameObject);
            }
        }
    }

    private void InstanciarPowerUp(int index)
    {
        if (index >= 0 && index < powerups.Length && powerups[index] != null)
        {
            Instantiate(powerups[index], transform.position, powerups[index].transform.rotation);
        }
    }

    private void InstanciarPowerUpAleatorio()
    {
        if (powerups.Length == 0) return;

        int index = Random.Range(0, powerups.Length);
        InstanciarPowerUp(index);
    }

}
