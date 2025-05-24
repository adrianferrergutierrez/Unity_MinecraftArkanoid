// SceneUIManager.cs
using UnityEngine;
using TMPro; // Para TextMeshProUGUI
using System;
using UnityEngine.UI;

public class ManagerScene : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public Slider experienceBar;            // Arrastra tu Slider aquí
    public TextMeshProUGUI levelNumberText; // Arrastra el texto del nivel aquí
    private int totalBloquesDelNivel;
    private int bloquesDestruidos;
    public int level_index;



    void Start()
    {
        if (GameManager.instance == null)
        {

            Debug.LogError("SceneUIManager no pudo encontrar GameManager.instance!");
            if (scoreText) scoreText.text = "Error: GM null";
            if (livesText) livesText.text = "Error: GM null";
            return;
        }

        //contamos elnumero de blques qeu tenemos que destruir para ganar el nivel
        totalBloquesDelNivel = FindObjectsByType<Bloque_destruible>(FindObjectsSortMode.None).Length;
        bloquesDestruidos = 0;
        ActualizarBarraExperiencia();

        // Nos subscruivbimos con esta notacion en C#, usamos el "patron observer"
        GameManager.instance.OnScoreChanged += UpdateScoreDisplay;
        GameManager.instance.OnLivesChanged += UpdateLivesDisplay;
        // GameManager.instance.OnBlockDestroyed += UpdateBlocksDisplay; // Si tuvieras este evento

        // Actualizar la UI con los valores iniciales al cargar la escena
        UpdateScoreDisplay();
        UpdateLivesDisplay();

        //actualizamos la barra
        if (levelNumberText != null)
        {
            levelNumberText.text = (GameManager.instance.currentLevelIndex + 1).ToString();
        }
        // UpdateBlocksDisplay();
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
        ActualizarBarraExperiencia(); // ¡Actualizamos la barra!

        if (bloquesDestruidos >= totalBloquesDelNivel)
        {
            // ¡Nivel completado!
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
                // Calculamos la fracción de bloques destruidos.
                experienceBar.value = (float)bloquesDestruidos / totalBloquesDelNivel;
            }
            else
            {
                experienceBar.value = 0; // Evitar división por cero
            }
        }
    }

    // void UpdateBlocksDisplay() { /* Lógica para actualizar texto de bloques */ }


    // --- Métodos para botones (ejemplos que llamarían al GameManager) ---
    // Estos métodos los conectarías a los OnClick() de tus botones en el Inspector de Unity

    /* public void BotonPausaPresionado()
     {
         // GameManager.instance.PausarJuego(); // Si GameManager tuviera un método PausarJuego()
         Debug.Log("Botón Pausa presionado (lógica de pausa no implementada en GM)");
     }

     public void BotonReintentarNivelPresionado()
     {
         if (GameManager.instance != null) GameManager.instance.RetryCurrentLevel();
     }

     public void BotonVolverAlMenuPresionado()
     {
         if (GameManager.instance != null) GameManager.instance.GoToMainMenu();
     }

     public void BotonEmpezarJuegoPresionado() // Para el botón en el menú principal
     {
         if (GameManager.instance != null) GameManager.instance.StartGameFromMenu();
     }*/
}