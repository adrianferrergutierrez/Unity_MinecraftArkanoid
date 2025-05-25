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
     public Vector3 palaInitialSpawnOffset = new Vector3(0.5f, 1.0f, -3.0f); // Para calcular la posición de spawn de la bola


    //numero de vidas que tenemos, se mostrará en la ui a partir de este numero
    public int vidas_player = 3;
    private Vector3 initialSpawnPosition;
    public int puntuacion = 0;
    private bool estado_oro = false;


    private bool trueOriginalFogWasEnabled;
    private Color trueOriginalFogColor;
    private float trueOriginalFogDensity;
    private bool isDebuffFogCurrentlyActive = false; // Para saber si ya hay un debuff de niebla activo

    private Coroutine activeFogDebuffCoroutine = null; // Para mantener una referencia a la corutina activa





    //eventos
    public delegate void GameStateChanged(); // Un tipo de delegado para eventos simples
    public event GameStateChanged OnScoreChanged;
    public event GameStateChanged OnLivesChanged;

    //escenas


    private string[] levelSceneNames = { "Scene1", "Scene2", "Scene3", "Scene4", "Scene5" };
    private string mainMenuSceneName = "SceneMainMenu";
    private string gameOverSceneName = "SceneGameOver";
    private string winSceneName = "SceneWin";
    public string creditsSceneName = "SceneCredits"; 


    public int currentLevelIndex = 0; // Para rastrear el nivel actual
    private bool primeraBolaLanzadaDelNivel = false; // Para la lógica del primer lanzamiento


   public AudioSource musicSource;     // Aquí se reproduce la música
    public AudioClip[] musicClips;      // Lista de clips de música
    public AudioClip main_menu;
    public AudioClip GameOver;
    public AudioClip Winner;
    public AudioClip creditsMusic; 

    private bool powerUpImanActivo = false;


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

        musicSource = GetComponent<AudioSource>();

    }
    private void PlayMusic(AudioClip clipToPlay)
    {
        // Si no tenemos el componente AudioSource o el clip es nulo, no hacemos nada.
        if (musicSource == null || clipToPlay == null)
        {
            Debug.LogWarning("MusicSource o el AudioClip es nulo. No se puede reproducir música.");
            return;
        }

        // Para no reiniciar la música si ya está sonando la misma canción
        // (por ejemplo, si se reinicia el nivel con "Retry")
        if (musicSource.clip == clipToPlay)
        {
            return;
        }

        musicSource.clip = clipToPlay;
        musicSource.Play();
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

    void Update()
    {
        CheckDebugLevelLoadKeys();
    }

    private void CheckDebugLevelLoadKeys()
    {
        // Asumimos que Scene1 es el nivel 0, Scene2 el 1, etc.
        if (Input.GetKeyDown(KeyCode.Alpha1)) { LoadLevelByIndex(0, true); } // Tecla 1 para Scene1
        if (Input.GetKeyDown(KeyCode.Alpha2)) { LoadLevelByIndex(1, true); } // Tecla 2 para Scene2
        if (Input.GetKeyDown(KeyCode.Alpha3)) { LoadLevelByIndex(2, true); } // Tecla 3 para Scene3
        if (Input.GetKeyDown(KeyCode.Alpha4)) { LoadLevelByIndex(3, true); } // Tecla 4 para Scene4
        if (Input.GetKeyDown(KeyCode.Alpha5)) { LoadLevelByIndex(4, true); } // Tecla 5 para Scene5
    }

    public void LoadLevelByIndex(int levelIndex, bool isDebugLoad)
    {
        if (levelSceneNames == null || levelIndex < 0 || levelIndex >= levelSceneNames.Length)
        {
            Debug.LogError("Índice de nivel " + levelIndex + " es inválido o 'levelSceneNames' no está configurado.");
            return;
        }

        Debug.LogWarning("Cargando nivel por índice: " + levelIndex + " (Escena: " + levelSceneNames[levelIndex] + "), Debug: " + isDebugLoad);

        currentLevelIndex = levelIndex; // Establecemos el índice actual

        if (isDebugLoad)
        {
            puntuacion = 0;
            vidas_player = 3; // O tu valor inicial de vidas
                              // Aquí puedes añadir cualquier otro reseteo específico del modo debug
        }

        primeraBolaLanzadaDelNivel = false; // Siempre se resetea al cargar un nivel

        // Notificamos a la UI para que se actualice (aunque la escena va a cambiar)
        OnScoreChanged?.Invoke();
        OnLivesChanged?.Invoke();

        SceneManager.LoadScene(levelSceneNames[levelIndex]);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name);
        pala = GameObject.FindGameObjectWithTag("Pala");


        if (scene.name == mainMenuSceneName) { PlayMusic(main_menu); } // Asumiendo que main_menu es main_menu_music
        else if (scene.name == gameOverSceneName) { PlayMusic(GameOver); } // Asumiendo que GameOver es gameOverMusic
        else if (scene.name == winSceneName) { PlayMusic(Winner); } // Para la pantalla de victoria
        else if (scene.name == creditsSceneName) { PlayMusic(creditsMusic); } // Para la pantalla de créditos

        else if (IsCurrentSceneLevel(scene.name))
        {
            // Nos aseguramos de que el índice del nivel sea válido para el array de música
            if (currentLevelIndex >= 0 && currentLevelIndex < musicClips.Length)
            {
                PlayMusic(musicClips[currentLevelIndex]);
            }
            else
            {
                Debug.LogError("No hay clip de música para el nivel " + currentLevelIndex + ". Revisa el array 'musicClips' en el GameManager.");
            }
        }

        if (pala == null && IsCurrentSceneLevel(scene.name)) // Pasamos scene.name para la comprobación
        {
            Debug.LogError("¡ADVERTENCIA! No se encontró la pala ('Pala' tag) en la escena de nivel: " + scene.name);
        }

        // --- LÓGICA ACTUALIZADA PARA DETERMINAR currentLevelIndex ---
        int foundIndex = System.Array.IndexOf(levelSceneNames, scene.name);
        if (foundIndex != -1)
        {
            // Si la escena cargada está en nuestra lista de niveles, actualizamos el índice.
            currentLevelIndex = foundIndex;
            Debug.Log("GameManager: Nivel actual establecido al índice " + currentLevelIndex + " (" + scene.name + ")");
        }
        // Si no se encuentra (ej. es el menú, game over), currentLevelIndex no cambia o
        // podrías asignarle un valor especial como -1 si necesitas saber que no es un nivel.

        if (IsCurrentSceneLevel(scene.name))
        {
            activeBalls.Clear();
            primeraBolaLanzadaDelNivel = false;
            // Aquí podrías llamar a un ManagerScene.SetupLevel() si necesitas que la escena se configure.

            if (vidas_player > 0)
            {
                RespawnBall(true);
            }
            else
            {
                HandleGameOver();
            }
        }
        else
        {
            ClearAllBalls();
        }

        OnScoreChanged?.Invoke();
        OnLivesChanged?.Invoke();
    }

    public void GoToCreditsScreen()
    {
        SceneManager.LoadScene(creditsSceneName);
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
    }

    public void ReiniciarPuntos()
    {
        puntuacion = 0;

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
            pala = GameObject.FindGameObjectWithTag("Pala");
            if (pala == null)
            {
                Debug.LogError("No se puede respawnear la bola, la pala es nula y no se encontró.");
                return;
            }
        }
        // Calculamos la posición de spawn usando el offset
        Vector3 spawnPosition = pala.transform.position + pala.transform.TransformDirection(palaInitialSpawnOffset);
        Debug.Log("Posición de spawn calculada: " + spawnPosition);
        // Usamos la spawnPosition calculada
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

    private void InstantiateNewBall(Vector3 spawnPosition, bool isInitialSpawn)
    {
        if (ballPrefab != null)
        {
            GameObject newBallGO = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
            AddBall(newBallGO);
            Ball3D ballScript = newBallGO.GetComponent<Ball3D>();

            if (ballScript != null)
            {
                if (isInitialSpawn)
                {
                    // Esta parte está bien, la pala se encarga de la bola inicial
                    Paddle paddleScript = pala.GetComponent<Paddle>();
                    if (paddleScript != null)
                    {
                        paddleScript.AsignarBola(ballScript);
                    }
                }
                else // Multibola
                {
                    // ¡Llamamos al NUEVO método en lugar de a LanzarDesdePala!
                    ballScript.LanzarComoNuevaBola(new Vector3(Random.Range(-0.5f, 0.5f), 0, 1).normalized);
                }
            }
        }
        else Debug.LogError("¡Prefab de Bola no asignado!");
    }

    public void ActivarPowerUpIman(bool estaActivo)
    {
        powerUpImanActivo = estaActivo;
    }

    public bool EstaImanActivo()
    {
        return powerUpImanActivo;
    }

    // Renombramos tu "LaunchFirstBallIfReady" para que sea más claro
    public void MarcarPrimeraBolaLanzada()
    {
        primeraBolaLanzadaDelNivel = true;
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

    public void setVidasPlayer(int vidas) {
        vidas_player = vidas;
        OnLivesChanged?.Invoke();

    }

    public void ActivarNiebla(Color debuffColor, float debuffIntensity)
    {
        // Si no había ya un debuff de niebla activo, guardamos el estado actual REAL del juego
        if (!isDebuffFogCurrentlyActive)
        {
            trueOriginalFogWasEnabled = RenderSettings.fog;
            trueOriginalFogColor = RenderSettings.fogColor;
            trueOriginalFogDensity = RenderSettings.fogDensity;
        }

        isDebuffFogCurrentlyActive = true; // Marcamos que un debuff de niebla está activo

        // Aplicamos la nueva niebla del debuff
        RenderSettings.fog = true;
        RenderSettings.fogColor = debuffColor;
        RenderSettings.fogDensity = debuffIntensity;

        // Si ya hay una corutina de desactivación de niebla corriendo, la paramos.
        // Esto es para "resetear" el contador si se activa otro debuff de niebla.
        if (activeFogDebuffCoroutine != null)
        {
            StopCoroutine(activeFogDebuffCoroutine);
        }

        // Iniciamos (o reiniciamos) la corutina para desactivar la niebla después de 5 segundos.
        activeFogDebuffCoroutine = StartCoroutine(DesactivarNieblaTrasTiempo());
    }

    IEnumerator DesactivarNieblaTrasTiempo()
    {
        yield return new WaitForSeconds(5);

        // Cuando la corutina termina, restauramos a los valores ORIGINALES que guardamos al principio.
        RenderSettings.fog = trueOriginalFogWasEnabled;
        RenderSettings.fogColor = trueOriginalFogColor;
        RenderSettings.fogDensity = trueOriginalFogDensity;

        isDebuffFogCurrentlyActive = false; // Ya no hay un debuff de niebla activo
        activeFogDebuffCoroutine = null; // Limpiamos la referencia a la corutina
    }



}
