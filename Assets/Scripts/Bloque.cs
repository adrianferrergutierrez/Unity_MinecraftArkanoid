using UnityEngine;

public class Bloque : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int vidas = 3;
 

    public GameObject[] powerups;
    //power up 0 -> manzana
    //power up 1 -> cristal
    //power up 2-> redstone aunque realmente o hace falta hacerlo asi, si solo dropea un item se le puede poner en el item 0 y ya, lo he pensado  tarde

    public AudioClip[] clips;
    //sistema de particulas
    public GameObject sistema_particulas;
    public Transform upperStructureParent;



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
                if (CompareTag("BloqueCentralNether"))
                {
                    Debug.Log("Bloque Central Nether destruido. Liberando estructura superior.");

                    // Verifica si tenemos la referencia al padre de la estructura superior
                    if (upperStructureParent != null)
                    {
                        // Itera a través de TODOS los GameObjects hijos del padre "Parte de Arriba".
                        // Iterar hacia atrás es CRUCIAL al desvincular para no perder referencias.
                        for (int i = upperStructureParent.childCount - 1; i >= 0; i--)
                        {
                            Transform child = upperStructureParent.GetChild(i);

                            // 1. Desvincula el hijo de su padre "Parte de Arriba"
                            child.parent = null; // Establece el padre a null

                            // 2. Asegúrate de que la física se active en este hijo ahora desvinculado
                            Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                            if (childRigidbody != null)
                            {
                                // Si el Rigidbody estaba marcado como Kinematic (para mantenerlo fijo)
                                if (childRigidbody.isKinematic)
                                {
                                    childRigidbody.isKinematic = false; // Desactiva Kinematic para que la física lo controle
                                    Debug.Log("Liberando física en: " + child.name);

                                    // Opcional: Añade una pequeña fuerza para que se separen un poco
                                    // childRigidbody.AddExplosionForce(50f, transform.position, 5f);
                                    childRigidbody.AddForce(Random.insideUnitSphere * 1f, ForceMode.Impulse); // Pequeño empujón aleatorio
                                }
                                // Asegúrate de que la gravedad está activada
                                childRigidbody.useGravity = true;
                            }
                            else
                            {
                                Debug.LogWarning("¡El bloque hijo " + child.name + " no tiene un componente Rigidbody! No podrá caer.");
                            }

                            // Opcional: Habilitar otros scripts en los hijos si es necesario al liberarse
                        }
                        Destroy(upperStructureParent.gameObject);
                        Destroy(gameObject);
                    }
                }

                else
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

                    return;

                    GameObject particlesInstance = Instantiate(sistema_particulas, transform.position, Quaternion.identity);
                    AudioSource audio = particlesInstance.GetComponent<AudioSource>();
                    ParticleSystem particulas = particlesInstance.GetComponent<ParticleSystem>();
                    particulas.Play();

                    //haremos que se escuche el sonido

                    AudioClip clip_que_sonara;
                    int randomIndex = Random.Range(0, clips.Length);
                    clip_que_sonara = clips[randomIndex];
                    audio.PlayOneShot(clip_que_sonara);
                    Destroy(particlesInstance, particulas.main.duration);
                    Destroy(gameObject);
                }
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
