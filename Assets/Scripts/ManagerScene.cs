    using UnityEngine;
    using TMPro; 
    using System;
    using UnityEngine.UI;

    public class ManagerScene : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI livesText;
        public Slider experienceBar;            
        public TextMeshProUGUI levelNumberText; 
        public int totalBloquesDelNivel;
        public int bloquesDestruidos;
        public int level_index;



    void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("ManagerScene no pudo encontrar GameManager.instance!");
            // ... (tu código de error) ...
            return;
        }

        totalBloquesDelNivel = FindObjectsByType<Bloque_destruible>(FindObjectsSortMode.None).Length;
        bloquesDestruidos = 0;
        ActualizarBarraExperiencia();

        GameManager.instance.OnScoreChanged += UpdateScoreDisplay;
        GameManager.instance.OnLivesChanged += UpdateLivesDisplay;

        UpdateScoreDisplay();
        UpdateLivesDisplay();

        // Esta línea AHORA usará el currentLevelIndex correcto del GameManager,
        // porque GameManager lo actualizó en su OnSceneLoaded.
        if (levelNumberText != null)
        {
            levelNumberText.text = (GameManager.instance.currentLevelIndex + 1).ToString();
        }
    }

    void OnDestroy()
        {
            // MUY IMPORTANTE: Desuscribirse para evitar errores cuando este objeto se destruye
            if (GameManager.instance != null)
            {
                //COn el += Nos subscribimos, con el -= nos desuscribimos
                GameManager.instance.OnScoreChanged -= UpdateScoreDisplay;
                GameManager.instance.OnLivesChanged -= UpdateLivesDisplay;
                // GameManager.instance.OnBlockDestroyed -= UpdateBlocksDisplay;
            }
        }

        void UpdateScoreDisplay()
        {
            if (scoreText != null && GameManager.instance != null)
            {
                scoreText.text = "Puntos: " + GameManager.instance.puntuacion;
            }
    
        }

        void UpdateLivesDisplay()
        {
            if (livesText != null && GameManager.instance != null)
            {
                livesText.text = "Vidas: " + GameManager.instance.GetVidasPlayer(); // Usamos el getter
            }
       
        }

        internal static object GetActiveScene()
        {
            throw new NotImplementedException();
        }

    public float ratio_acabado_nivel()
    {
        if (totalBloquesDelNivel <= 0) return 0f; 
        return (float)bloquesDestruidos / totalBloquesDelNivel;
    }


    public void RegistrarBloqueDestruido()
    {
        bloquesDestruidos++;
        ActualizarBarraExperiencia();

        // Añadimos una comprobación para evitar pasar de nivel si no había bloques que destruir
        if (totalBloquesDelNivel > 0 && bloquesDestruidos >= totalBloquesDelNivel)
        {          
            GameManager.instance.GoToNextLevel(); 
        }
    }
    public void acabarNivel()
    {

        // Nos aseguramos de que los bloques destruidos igualen al total
        // (o lo superen si totalBloquesDelNivel era 0)
        if (totalBloquesDelNivel > 0)
        {
            bloquesDestruidos = totalBloquesDelNivel;
        }
        else
        {
            bloquesDestruidos = 0; 
        }

        ActualizarBarraExperiencia(); // ¡Importante! Actualiza la UI de la barra.

        // Ahora comprobamos si se debe pasar de nivel (que debería ser siempre true ahora)
        if (totalBloquesDelNivel == 0 || bloquesDestruidos >= totalBloquesDelNivel)
        {
            GameManager.instance.GoToNextLevel();
        }
     
    }

    private void ActualizarBarraExperiencia()
        {
            if (experienceBar != null)
            {
                if (totalBloquesDelNivel > 0)
                {
                    // El valor de un slider va de 0.0 a 1.0
                    // Calculamos la fracción de bloques destruidos.
                    experienceBar.value = (float)bloquesDestruidos / totalBloquesDelNivel;
                }
                else
                {
                    experienceBar.value = 0; // Evitar división por cero
                }
            }
        }
    }