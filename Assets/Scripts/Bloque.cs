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
    
    private ManagerScene manager_escena;

    public Transform upperStructureParent;
    private bool powerball;

    public GameObject powerup_exp;

    // Probabilidades
    public float probabilidad_powerup;

    



    void Start()
    {
        manager_escena = FindFirstObjectByType<ManagerScene>();
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
                bool esBloqueContableParaGanar = GetComponent<Bloque_destruible>() != null;

                if (esBloqueContableParaGanar)
                {
                    if (manager_escena != null)
                    {
                        manager_escena.RegistrarBloqueDestruido();
                        if (GameManager.instance.get_state_oro()) GameManager.instance.SumarPuntos(400);
                        else GameManager.instance.SumarPuntos(100);
                    }
                    // Puedes dar unos puntos base por destruir cualquier bloque contable aquí
                    // GameManager.instance.SumarPuntos(10); 
                }

                //comportamiento especifico de los bloques centrales, donde primero hacen que los hijos dejen de ser sus hijos para no ser eliminados todos juntos
                if (CompareTag("BloqueCentralNether"))
                {
                    //le decimos al manager de la escena que lleva el contador de cuantos bloques se tienen que destruir en esta escena que hemos eliminado 1 

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
               
                        Eliminacion(); 

                        //Destroy(gameObject);
                    }
                }

                else
                {
                    float random = Random.value;
                    //si estamos en los ultimos bloques le subimos la probabilidad de dropear el experiencia powerup, haciendo que pasemos de nivel 
                    if (manager_escena.ratio_acabado_nivel() >= 0.95f) probabilidad_powerup = 0.5f;

                    if (CompareTag("Bloque_powerup_especifico") && random < probabilidad_powerup)
                    {
                        //le decimos al manager de la escena que lleva el contador de cuantos bloques se tienen que destruir en esta escena que hemos eliminado 1 
                        InstanciarPowerUp(0);
                      

                    }
                    else if (CompareTag("Bloque_powerup_random") && random < probabilidad_powerup)
                    {
                        //le decimos al manager de la escena que lleva el contador de cuantos bloques se tienen que destruir en esta escena que hemos eliminado 1 
                        InstanciarPowerUpAleatorio();
                
                    }
                    else if (CompareTag("Wither")) {
                        //aqui no baajmos el numero de bloques destruidos porque es un bloque "malo" que no pasa nada si no se destruye
                        GameManager.instance.SumarPuntos(-500);
                        GameManager.instance.ActivarNiebla(Color.black, 0.13f);
                    
                    }
                    else if (CompareTag("Coral_debuff"))
                    {
                        GameManager.instance.SumarPuntos(-500);
                        GameManager.instance.ActivarNiebla(Color.magenta, 0.08f);
                    }

             

                    Eliminacion();
                    }
                }
            }
        }




    private void InstanciarPowerUp(int index)
    {
        if ((index >= 0 && index < powerups.Length && powerups[index] != null) && manager_escena.ratio_acabado_nivel() < 0.95f)
        {
            Instantiate(powerups[index], transform.position, powerups[index].transform.rotation);
        }
        else Instantiate(powerup_exp, transform.position, powerup_exp.transform.rotation);
    }

    private void InstanciarPowerUpAleatorio()
    {
        if (powerups.Length == 0) return;
        if (manager_escena.ratio_acabado_nivel() >= 0.95f) {
            Instantiate(powerup_exp, transform.position, powerup_exp.transform.rotation);
        }
        else {
            int index = Random.Range(0, powerups.Length);
            InstanciarPowerUp(index); }
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
