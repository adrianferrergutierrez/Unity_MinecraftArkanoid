using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloque : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int vidas = 3;
 

    public GameObject[] powerups;
    //para los objetos que solo tienen un drop ponemos el unico item y hacemos que se dopee el item [0]. Si queremos aleatorio hacemos que se haga random entre el numero de items que tiene el array.

    public AudioClip[] clips;
    //sistema de particulas
    public GameObject sistema_particulas;
    public Transform upperStructureParent;
    private bool powerball;



    // Probabilidades
    public float probabilidad_powerup;

    



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
            Ball3D pelota_script = collision.gameObject.GetComponent<Ball3D>();
            if (pelota_script.get_state_powerball()) vidas = 0;
            else vidas--;

            if (vidas == 0)
            {
                //si tenemos el powerup de oro, sumamos por bloque destruido 400 puntos
                if (GameManager.instance.get_state_oro()) GameManager.instance.SumarPuntos(400);
                else GameManager.instance.SumarPuntos(100);
                //comportamiento especifico de los bloques centrales, donde primero hacen que los hijos dejen de ser sus hijos para no ser eliminados todos juntos
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
                        //Eliminacion(); DESCOMENTAR CUANDO FUNCIONEN LAS PARTICULASS DE DESTRUCCION PARA TODOS
                        Destroy(gameObject);
                    }
                }

                else
                {
                    float random = Random.value;

                    if (CompareTag("Bloque_powerup_especifico") && random < probabilidad_powerup)
                    {
                        InstanciarPowerUp(0);
                    }
                    else if (CompareTag("Bloque_powerup_random") && random < probabilidad_powerup)
                    {
                        InstanciarPowerUpAleatorio();
                    }
                    else if (CompareTag("Wither")) {
                       GameManager.instance.ActivarNiebla();
                    
                    }
                        //destroy game object provisional,esto se quitara cuando todos los bloques tengan ya las particulas y sonidos, si no da error

                        Destroy(gameObject);

                        return;

                        Eliminacion();
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

    //funcion que se usara cuando tengamos todas las particulas funcionando
    private void Eliminacion()
    {
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
