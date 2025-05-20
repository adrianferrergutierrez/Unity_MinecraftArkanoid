// SceneUIManager.cs
using UnityEngine;
using TMPro; // Para TextMeshProUGUI
using System;

public class ManagerScene : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    // public TextMeshProUGUI blocksText; // Si quieres mostrar bloques restantes

    void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("SceneUIManager no pudo encontrar GameManager.instance!");
            if (scoreText) scoreText.text = "Error: GM null";
            if (livesText) livesText.text = "Error: GM null";
            return;
        }

        // Nos subscruivbimos con esta notacion en C#, usamos el "patron observer"
        GameManager.instance.OnScoreChanged += UpdateScoreDisplay;
        GameManager.instance.OnLivesChanged += UpdateLivesDisplay;
        // GameManager.instance.OnBlockDestroyed += UpdateBlocksDisplay; // Si tuvieras este evento

        // Actualizar la UI con los valores iniciales al cargar la escena
        UpdateScoreDisplay();
        UpdateLivesDisplay();
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