using UnityEngine;
using System.Collections; // Necesario para Coroutines

public class EndermanGOlpe : MonoBehaviour
{
    public Animator animator;
    public AudioClip clip_golpe; // Un solo AudioClip para el golpe
    public GameObject sistema_particulas_golpe_prefab; // Prefab que DEBE tener ParticleSystem y AudioSource
    public string triggerGolpe = "ActivarGolpe";
    public float duracionDesaparicion = 3.0f; // Cuánto tiempo estará DESAPARECIDO
    public float tiempoEsperaAntesDeDesaparecer = 1.0f; // Tiempo para ver el golpe antes de desaparecer

    // Variables para hacer desaparecer al Enderman
    public SkinnedMeshRenderer meshDelEnderman; // Arrastra aquí el SkinnedMeshRenderer
    public Collider colliderDelEnderman;      // Arrastra aquí el Collider principal

    // Referencia al script que maneja las vidas
    public Bloque bloqueScript; // Asigna esto en el Inspector o se intentará obtener en Start

    // Variables para los efectos de ausencia
    public ParticleSystem particulasDeAusencia;
    public AudioSource audioSourceAusencia;
    public AudioClip sonidoAusenciaLoop;


    private GameObject instanciaEfectosActual = null;
    private AudioSource audioSourceEfectosGolpe = null; // Renombrado para claridad
    private ParticleSystem particulasEfectosGolpe = null; // Renombrado para claridad

    private bool enSecuenciaDeAccion = false;

    void Start()
    {
        // Asumimos que 'animator', 'meshDelEnderman', y 'colliderDelEnderman'
        // serán asignados desde el Inspector de Unity.

        if (bloqueScript == null)
        {
            bloqueScript = GetComponent<Bloque>();
            if (bloqueScript == null)
            {
                Debug.LogError("Script 'Bloque' no encontrado en el Enderman. La lógica de vidas no funcionará.", this);
            }
        }

        // Configurar partículas de ausencia
        if (particulasDeAusencia != null)
        {
            var mainModule = particulasDeAusencia.main;
            mainModule.playOnAwake = false;
            particulasDeAusencia.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            Debug.LogWarning("Sistema 'particulasDeAusencia' no asignado.", this);
        }

        // Configurar AudioSource de ausencia
        if (audioSourceAusencia != null)
        {
            audioSourceAusencia.playOnAwake = false;
            audioSourceAusencia.loop = true;
            if (sonidoAusenciaLoop != null)
            {
                audioSourceAusencia.clip = sonidoAusenciaLoop;
            }
            else
            {
                Debug.LogWarning("No se asignó 'sonidoAusenciaLoop'.", this);
            }
        }
        else
        {
            Debug.LogWarning("'audioSourceAusencia' no asignado.", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pelota"))
        {
            if (enSecuenciaDeAccion) // Si ya hay una secuencia en curso
            {
                Debug.Log("Interrumpiendo secuencia anterior.");
                StopAllCoroutines(); // Detener todas las corrutinas

                // Detener explícitamente los efectos de ausencia
                if (audioSourceAusencia != null) audioSourceAusencia.Stop();
                if (particulasDeAusencia != null) particulasDeAusencia.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                // Destruir efectos de golpe anteriores si existen
                if (instanciaEfectosActual != null)
                {
                    if (audioSourceEfectosGolpe != null) audioSourceEfectosGolpe.Stop();
                    if (particulasEfectosGolpe != null) particulasEfectosGolpe.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    Destroy(instanciaEfectosActual);
                    instanciaEfectosActual = null; // Limpiar referencia
                }
                ReaparecerEnderman(); // Asegurarse de que esté visible
                enSecuenciaDeAccion = false; // Marcar que la secuencia anterior ha terminado
            }

            // Comprobar vidas después de haber manejado una posible interrupción.
            // Asumimos que bloqueScript.get_vidas() devuelve las vidas DESPUÉS de que el script Bloque procese el golpe actual.
            if (bloqueScript != null && bloqueScript.get_vidas() <= 0) // MODIFICADO AQUÍ
            {
                // Si después del golpe tiene 0 o menos vidas (es el golpe mortal),
                // solo activamos la animación de golpe y dejamos que Bloque maneje la muerte.
                animator.SetTrigger(triggerGolpe);
                // Podrías instanciar aquí los efectos de sonido/partículas del golpe si quieres que se vean/escuchen en el golpe final
                // Ejemplo: ActivarSoloEfectosDeGolpeInmediatos();
                return; // No continuar con la secuencia de desaparición
            }

            // Si no es el golpe mortal, iniciar la nueva secuencia de golpe y desaparición
            StartCoroutine(ActivarGolpeYDesaparecer());
        }
    }

    private IEnumerator ActivarGolpeYDesaparecer()
    {
        enSecuenciaDeAccion = true;

        animator.SetTrigger(triggerGolpe);

        instanciaEfectosActual = Instantiate(sistema_particulas_golpe_prefab, transform.position, Quaternion.identity);
        particulasEfectosGolpe = instanciaEfectosActual.GetComponent<ParticleSystem>();
        audioSourceEfectosGolpe = instanciaEfectosActual.GetComponent<AudioSource>();

        if (particulasEfectosGolpe != null) particulasEfectosGolpe.Play();
        if (audioSourceEfectosGolpe != null && clip_golpe != null)
        {
            audioSourceEfectosGolpe.clip = clip_golpe;
            audioSourceEfectosGolpe.Play();
        }

        yield return new WaitForSeconds(tiempoEsperaAntesDeDesaparecer);

        if (this == null || gameObject == null)
        {
            enSecuenciaDeAccion = false;
            if (instanciaEfectosActual != null) Destroy(instanciaEfectosActual);
            yield break;
        }
        DesaparecerEnderman();

        if (particulasDeAusencia != null) particulasDeAusencia.Play();
        if (audioSourceAusencia != null && audioSourceAusencia.clip != null) audioSourceAusencia.Play();

        yield return new WaitForSeconds(duracionDesaparicion);

        if (audioSourceAusencia != null) audioSourceAusencia.Stop();
        if (particulasDeAusencia != null) particulasDeAusencia.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (this == null || gameObject == null)
        {
            enSecuenciaDeAccion = false;
            if (instanciaEfectosActual != null) Destroy(instanciaEfectosActual);
            yield break;
        }
        ReaparecerEnderman();

        if (instanciaEfectosActual != null)
        {
            float tiempoExtraParaDesvanecer = 0f;
            if (particulasEfectosGolpe != null)
            {
                if (particulasEfectosGolpe.main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                    tiempoExtraParaDesvanecer = particulasEfectosGolpe.main.startLifetime.constant;
                else if (particulasEfectosGolpe.main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                    tiempoExtraParaDesvanecer = particulasEfectosGolpe.main.startLifetime.constantMax;
                else
                    tiempoExtraParaDesvanecer = particulasEfectosGolpe.main.duration;
            }

            Destroy(instanciaEfectosActual, tiempoExtraParaDesvanecer + 0.5f);

            instanciaEfectosActual = null;
            audioSourceEfectosGolpe = null;
            particulasEfectosGolpe = null;
        }

        enSecuenciaDeAccion = false;
    }

    void DesaparecerEnderman()
    {
        if (meshDelEnderman != null) meshDelEnderman.enabled = false;
        if (colliderDelEnderman != null) colliderDelEnderman.enabled = false;
    }

    void ReaparecerEnderman()
    {
        if (meshDelEnderman != null) meshDelEnderman.enabled = true;
        if (colliderDelEnderman != null) colliderDelEnderman.enabled = true;
    }
}
