using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
    {
        public GameObject ballPrefab;
        public static GameManager instance;
        private List<GameObject> activeBalls = new List<GameObject>();

        // Referencia a la pala para saber dónde spawnear la primera bola (opcional)
        private GameObject pala;
        private Vector3 palaInitialSpawnOffset = new Vector3(0.5f, 1.0f, -3.0f); // Para calcular la posición de spawn de la bola


    //numero de vidas que tenemos, se mostrará en la ui a partir de este numero
    private int vidas_player = 3;
        private Vector3 initialSpawnPosition;
        public int puntuacion = 0;
        private bool estado_oro = false;




    //fog
    private bool fogWasEnabled;
        private Color originalFogColor;
        private float originalFogDensity;


        //eventos
        public delegate void GameStateChanged(); // Un tipo de delegado para eventos simples
        public event GameStateChanged OnScoreChanged;
        public event GameStateChanged OnLivesChanged;

    //escenas
    private string[] levelSceneNames = { "Scene1", "Scene2", "Scene3", "Scene4", "Scene5" };
    private string mainMenuSceneName = "SceneMainMenu";
    private string gameOverSceneName = "SceneGameOver";
    private string winSceneName = "SceneWin";

    private int bloquesRestantesEnNivel;
    private int currentLevelIndex = 0; // Para rastrear el nivel actual
    private bool primeraBolaLanzadaDelNivel = false; // Para la lógica del primer lanzamiento



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

    
    void OnEnable()
    {
        // Le estamos diciendo al SceneManager: "Oye, cada vez que anuncies que una escena ha terminado de cargarse(sceneLoaded), por favor, ejecuta también mi método llamado OnSceneLoaded.

        SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al evento de carga de escena
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name);
        //buscamios la pala para spawnear la bola delante de la pala
        pala = GameObject.FindGameObjectWithTag("Pala"); // Busca la pala en la nueva escena

        if (pala == null && IsCurrentSceneLevel())
        {
            Debug.LogError("¡ADVERTENCIA! No se encontró la pala ('Pala' tag) en la escena de nivel: " + scene.name);
        }

        // Lógica para cuando se carga una escena de nivel
        if (IsCurrentSceneLevel(scene.name))
        {
            activeBalls.Clear(); // Limpia bolas de niveles anteriores
            primeraBolaLanzadaDelNivel = false;
            // Configurar el nivel (ej: contar bloques, posicionar elementos)
            // SetupLevel(); // Podrías tener un método para esto

            if (vidas_player > 0)
            {
                RespawnBall(true); // True indica que es el spawn inicial del nivel
            }
            else
            {
                // Si por alguna razón llegamos a un nivel sin vidas, vamos a Game Over
                HandleGameOver();
            }
        }
        else
        {
            // Si no es una escena de nivel (menú, game over, etc.), limpiamos las bolas activas
            ClearAllBalls();
        }
        // Asegurar que la UI se actualice al cargar cualquier escena (si el SceneUIManager está presente)
        OnScoreChanged?.Invoke();
        OnLivesChanged?.Invoke();
    }
    public void StartGameFromMenu()
    {
        puntuacion = 0;
        vidas_player = 3; // O tu valor inicial de vidas
        currentLevelIndex = 0;
        primeraBolaLanzadaDelNivel = false;
        OnScoreChanged?.Invoke();
        OnLivesChanged?.Invoke();
        if (levelSceneNames != null && levelSceneNames.Length > 0)
        {
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
        }
        else Debug.LogError("No hay escenas de nivel configuradas!");
    }
    public void RetryCurrentLevel()
    {
        // Opcional: Resetea vidas si quieres que cada reintento cueste el total de vidas del nivel
        // vidas_player = 3; 
        // OnLivesChanged?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena actual
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HandleGameOver()
    {
        Debug.Log("GAME OVER");
        SceneManager.LoadScene(gameOverSceneName);

   
    }

    private void HandleWinGame()
    {
        Debug.Log("¡HAS GANADO!");
        SceneManager.LoadScene(winSceneName);
    }

    public void GoToNextLevel()
    {
        currentLevelIndex++;
        if (levelSceneNames != null && currentLevelIndex < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
        }
        else
        {
            HandleWinGame();
        }
    }
    private bool IsCurrentSceneLevel(string sceneName = null)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        if (levelSceneNames == null || levelSceneNames.Length == 0) return false;
        foreach (string levelName in levelSceneNames)
        {
            if (levelName == sceneName) return true;
        }
        return false;
    }


    public void SumarPuntos(int puntos)
    {
        puntuacion += puntos;
        OnScoreChanged?.Invoke();

        //miramos si el numero de bloques que faltan del nivel esta completo o no
        bloquesRestantesEnNivel--;
        if (bloquesRestantesEnNivel <= 0 && IsCurrentSceneLevel())
        {
            Debug.Log("Todos los bloques destruidos!");
            GoToNextLevel();
        }
    }

    public void ReiniciarPuntos()
    {
        puntuacion = 0;

    }

    // ** Método que se ejecuta al inicio del juego/escena **
    /*void Start()
        {
            pala = GameObject.FindGameObjectWithTag("Pala");
            initialSpawnPosition = pala.transform.position + Vector3.forward * 3.0f + Vector3.up * 1.0f + Vector3.right * 0.25f;
            InstantiateNewBall(initialSpawnPosition); // Llama al método auxiliar para crear y registrar la bola
        }*/

    //Inicializamos la bola en su sitio otra vez
    /* void Start2()
       {
     InstantiateNewBall(initialSpawnPosition); 
      }

 */



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

                if (activeBalls.Count == 0)
                {
                vidas_player--;
                OnLivesChanged?.Invoke();

                if (vidas_player > 0)
                {
                    // Respawnear una nueva bola
                    RespawnBall(false);
                }
                else {
                    HandleGameOver();
                }
            }
        }
        }

    void RespawnBall(bool isInitialSpawn)
    {
        if (pala == null)
        {
            pala = GameObject.FindGameObjectWithTag("Pala"); // Intento extra por si acaso
            if (pala == null)
            {
                Debug.LogError("No se puede respawnear la bola, la pala es nula y no se encontró.");
                return;
            }
        }
        Vector3 spawnPosition = pala.transform.position + pala.transform.TransformDirection(palaInitialSpawnOffset);
        Debug.LogError("Spawn position: " + spawnPosition);
        InstantiateNewBall(spawnPosition, isInitialSpawn);
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
                InstantiateNewBall(spawnPos, false);// Reutilizamos el método auxiliar
            }}
        }

    // Método auxiliar para instanciar una sola bola y añadirla al seguimiento (IGUAL QUE ANTES)
    private void InstantiateNewBall(Vector3 spawnPosition, bool isInitialSpawn)
    {
        if (ballPrefab != null)
        {
            GameObject newBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
            AddBall(newBall);

            Ball3D ballScript = newBall.GetComponent<Ball3D>();
            if (ballScript != null)
            {
                // Lanza si es multibola o si no es el spawn inicial (es decir, una bola de reemplazo tras perder una)
                if (!isInitialSpawn || activeBalls.Count > 1)
                {
                    ballScript.launch(); // Asume que Ball3D tiene Launch()
                }
                // Si es isInitialSpawn y es la única bola, se esperará a LaunchFirstBallIfReady()
            }
        }
        else Debug.LogError("¡Prefab de Bola no asignado!");
    }


    private void ClearAllBalls() // Nueva función utilitaria
    {
        foreach (GameObject ball in new List<GameObject>(activeBalls)) // Copia para modificar la colección
        {
            if (ball != null) Destroy(ball);
        }
        activeBalls.Clear();
    }

    // Reemplaza tu destruccion_bola()
    public void HandleBallLost(GameObject bola)
    {
        if (bola != null && activeBalls.Contains(bola))
        {
            activeBalls.Remove(bola);
            Destroy(bola);
        }

        if (activeBalls.Count == 0 && IsCurrentSceneLevel())
        {
            vidas_player--;
            OnLivesChanged?.Invoke();

            if (vidas_player <= 0)
            {
                HandleGameOver();
            }
            else
            {
                primeraBolaLanzadaDelNivel = false;
                RespawnBall(true); // Es un nuevo "primer spawn" para esta vida
            }
        }
    }

    //el setup level solo calcula el numero de bloques HACE FALTA CAMBIAR EL TAAAAg
    public void SetupLevel()
    {
        GameObject[] bloques = GameObject.FindGameObjectsWithTag("BloqueDestruible"); // Asegúrate que tus bloques tengan este tag
        bloquesRestantesEnNivel = bloques.Length;
        Debug.Log("Nivel iniciado con " + bloquesRestantesEnNivel + " bloques.");
        // Podrías invocar un evento aquí si la UI necesita saber el total de bloques.
    }
    public void LaunchFirstBallIfReady()
    {
        if (IsCurrentSceneLevel() && !primeraBolaLanzadaDelNivel && activeBalls.Count == 1)
        {
            Ball3D ballScript = activeBalls[0].GetComponent<Ball3D>();
            if (ballScript != null)
            {
                ballScript.launch();
                primeraBolaLanzadaDelNivel = true;
            }
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

    public int GetVidasPlayer() {
        return vidas_player;
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
