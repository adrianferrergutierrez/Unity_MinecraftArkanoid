    // SceneUIManager.cs
    using UnityEngine;
    using TMPro; // Para TextMeshProUGUI
    using System;
    using UnityEngine.UI;

    public class ManagerScene : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI livesText;
        public Slider experienceBar;            // Arrastra tu Slider aqu�
        public TextMeshProUGUI levelNumberText; // Arrastra el texto del nivel aqu�
        public int totalBloquesDelNivel;
        public int bloquesDestruidos;
        public int level_index;



    void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("ManagerScene no pudo encontrar GameManager.instance!");
            // ... (tu c�digo de error) ...
            return;
        }

        totalBloquesDelNivel = FindObjectsByType<Bloque_destruible>(FindObjectsSortMode.None).Length;
        bloquesDestruidos = 0;
        ActualizarBarraExperiencia();

        GameManager.instance.OnScoreChanged += UpdateScoreDisplay;
        GameManager.instance.OnLivesChanged += UpdateLivesDisplay;

        UpdateScoreDisplay();
        UpdateLivesDisplay();

        // Esta l�nea AHORA usar� el currentLevelIndex correcto del GameManager,
        // porque GameManager lo actualiz� en su OnSceneLoaded.
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



    public void RegistrarBloqueDestruido()
    {
        bloquesDestruidos++;
        ActualizarBarraExperiencia();

        // A�adimos una comprobaci�n para evitar pasar de nivel si no hab�a bloques que destruir
        if (totalBloquesDelNivel > 0 && bloquesDestruidos >= totalBloquesDelNivel)
        {
            Debug.Log("Nivel completado! Pasando al siguiente...");
          
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
                    // Calculamos la fracci�n de bloques destruidos.
                    experienceBar.value = (float)bloquesDestruidos / totalBloquesDelNivel;
                }
                else
                {
                    experienceBar.value = 0; // Evitar divisi�n por cero
                }
            }
        }

        // void UpdateBlocksDisplay() { /* L�gica para actualizar texto de bloques */ }


        // --- M�todos para botones (ejemplos que llamar�an al GameManager) ---
        // Estos m�todos los conectar�as a los OnClick() de tus botones en el Inspector de Unity

        /* public void BotonPausaPresionado()
         {
             // GameManager.instance.PausarJuego(); // Si GameManager tuviera un m�todo PausarJuego()
             Debug.Log("Bot�n Pausa presionado (l�gica de pausa no implementada en GM)");
         }

         public void BotonReintentarNivelPresionado()
         {
             if (GameManager.instance != null) GameManager.instance.RetryCurrentLevel();
         }

         public void BotonVolverAlMenuPresionado()
         {
             if (GameManager.instance != null) GameManager.instance.GoToMainMenu();
         }

         public void BotonEmpezarJuegoPresionado() // Para el bot�n en el men� principal
         {
             if (GameManager.instance != null) GameManager.instance.StartGameFromMenu();
         }*/
    }