using UnityEngine;

public class EndermanGOlpe : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;            
    public AudioClip[] clips_golpe;
    public GameObject sistema_particulas_golpe;
    public Vector3[] posiciones_teletransporte;
    public string nombreAnimacionIdle = "Armature|Idle"; 
    public string nombreAnimacionGolpe = "Armature|Yell"; 
    public string triggerGolpe = "ActivarGolpe"; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota")) {
            ActivarGolpe();
            posiciones_teletransporte_func();
        }
    }

    private void ActivarGolpe()
    {

        animator.SetTrigger(triggerGolpe);
        GameObject particlesInstance = Instantiate(sistema_particulas_golpe, transform.position, Quaternion.identity);
        ParticleSystem particulas = particlesInstance.GetComponent<ParticleSystem>();
        particulas.Play();
        AudioSource audio = particlesInstance.GetComponent<AudioSource>();

        AudioClip clip_que_sonara;
        int randomIndex = Random.Range(0, clips_golpe.Length);
        clip_que_sonara = clips_golpe[randomIndex];
        audio.PlayOneShot(clip_que_sonara);
        Destroy(particlesInstance, particulas.main.duration);



    }

    private void posiciones_teletransporte_func()
    {
        int randomIndex = Random.Range(0, posiciones_teletransporte.Length);
        transform.Translate(posiciones_teletransporte[randomIndex]);
    }
}
