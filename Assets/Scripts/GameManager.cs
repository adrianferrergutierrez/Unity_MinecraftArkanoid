    using UnityEngine;
    using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
    {
        public GameObject ballPrefab;
        public static GameManager instance;

        private List<GameObject> activeBalls = new List<GameObject>();

        // Referencia a la pala para saber dónde spawnear la primera bola (opcional)
        private GameObject pala;

        //numero de vidas que tenemos, se mostrará en la ui a partir de este numero
        private int vidas_player = 3;

         private Vector3 initialSpawnPosition;

        public int puntuacion = 0;
        private bool estado_oro = false;


    //fog
    private bool fogWasEnabled;
    private Color originalFogColor;
    private float originalFogDensity;


    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    void Awake()
    {
        // Singleton: solo uno en toda la partida
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No se destruye entre escenas
        }
        else
        {
            Destroy(gameObject); // Si ya hay uno, se destruye el duplicado
        }
    }

    public void SumarPuntos(int puntos)
    {
        puntuacion += puntos;
        Debug.Log("Puntuación actual: " + puntuacion);
    }

    public void ReiniciarPuntos()
    {
        puntuacion = 0;
    }

    // ** Método que se ejecuta al inicio del juego/escena **
    void Start()
        {
            pala = GameObject.FindGameObjectWithTag("Pala");
            initialSpawnPosition = pala.transform.position + Vector3.forward * 3.0f + Vector3.up * 1.0f + Vector3.right * 0.25f;
            InstantiateNewBall(initialSpawnPosition); // Llama al método auxiliar para crear y registrar la bola
        }

    //Inicializamos la bola en su sitio otra vez
        void Start2()
          {
        InstantiateNewBall(initialSpawnPosition); 
         }



    
        public void AddBall(GameObject newBall)
        {
            if (newBall != null && !activeBalls.Contains(newBall))
            {
                activeBalls.Add(newBall);
            }
        }

        // Método para quitar una bola de la lista de seguimiento (cuando se destruye) (IGUAL QUE ANTES)
        public void RemoveBall(GameObject ballToRemove)
        {
            if (ballToRemove != null && activeBalls.Contains(ballToRemove))
            {
                activeBalls.Remove(ballToRemove);

                // ** Lógica de Fin de Juego si pierdes la última bola **
                if (activeBalls.Count == 0)
                {
                    Debug.Log("¡Última bola perdida! Fin del juego (implementar lógica aquí).");
                    // Aquí iría tu lógica de Game Over, como mostrar un panel, reiniciar nivel, etc.
                }
            }
        }

        // Método llamado por el script de la manzana para activar la multibola (IGUAL QUE ANTES)
        public void ActivateMultiball()
        {
        if (activeBalls.Count <= 5) { 
            Debug.LogError("¡Holaaa!");

            List<Vector3> positionsToSpawn = new List<Vector3>();
            foreach (GameObject ball in activeBalls)
            {
                if (ball != null)
                {
                    positionsToSpawn.Add(ball.transform.position);
                }
            }

            if (positionsToSpawn.Count == 0)
            {
                Debug.LogWarning("Multibola activada, pero no se encontraron bolas existentes para duplicar.");
                return;
            }

            foreach (Vector3 spawnPos in positionsToSpawn)
            {
                InstantiateNewBall(spawnPos); // Reutilizamos el método auxiliar
            }}
        }

        // Método auxiliar para instanciar una sola bola y añadirla al seguimiento (IGUAL QUE ANTES)
        private void InstantiateNewBall(Vector3 spawnPosition)
        {
            Debug.LogError("¡Holaaa2!");

            if (ballPrefab != null)
            {
                GameObject newBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
                AddBall(newBall);

                Rigidbody rb = newBall.GetComponent<Rigidbody>();
                if (rb != null)
                {
                

                if (activeBalls.Count > 1) newBall.GetComponent<Ball3D>().launch();
                
                }
            }
            else
            {
                Debug.LogError("¡Prefab de Bola no asignado en el GameManager!");
            }
        }
    
    /// <summary>
    /// Funciíon llamada por la bola cuando colisiona/ se detectaque esta fuera del mapa y borra del vector de bolas ese objeto, si era la ultima en ser borrada, se quita 1 vida y se pone la bola en el punto inicial.
    /// </summary>
    /// <param name="bola"> el parametro bola sera el propio gameobject que se pasará a si mismo desde Ball.cs al game manager para poder borrar este elemento y ver si era la ultima o no</param>
    public void destruccion_bola(GameObject bola) {
        activeBalls.Remove(bola); //borramos el objeto
        Destroy(bola);

        //era la ultima
        if (activeBalls.Count == 0) {
            vidas_player--; //lo usaremos para gestionar la ui y estados del juego
            Start2();
        }
    
    
        }


    public void powerball_change_state(bool estado)
    {
        foreach (GameObject ball in activeBalls) {
            Ball3D scriptball = ball.GetComponent<Ball3D>();
            scriptball.change_powerball_state(estado);
        }

}

    //lo ponemos aqui para acceder de fiorma mas facil desde el scrpt de bloques para preguntar si tenemos que sumar mas puntos por tener el powerup del oro
    public bool get_state_oro()
    {
        return estado_oro;

    }

    public void change_oro_state(bool estado)
    {
        estado_oro = estado;


    }



    public void ActivarNiebla()
    {
        // Guardar valores originales
        fogWasEnabled = RenderSettings.fog;
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;

        // Activar niebla negra
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.13f;

        StartCoroutine(DesactivarNieblaTrasTiempo());
    }

    IEnumerator DesactivarNieblaTrasTiempo()
    {
        yield return new WaitForSeconds(5);
        RenderSettings.fog = fogWasEnabled;
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogDensity = originalFogDensity;

    }



}
