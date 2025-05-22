// SceneUIManager.cs
using UnityEngine;
using TMPro; // Para TextMeshProUGUI
using System;

public class ManagerScene : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public int numero_bloques_destruibles_para_ganar;
    public int numero_bloques_destruidos_para_ganer;
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


    public void add_numero_bloques_destruidos(int bloques)
    {
        numero_bloques_destruidos_para_ganer += bloques;
        if (numero_bloques_destruidos_para_ganer >= numero_bloques_destruibles_para_ganar) {
            //perimro enviamos nmuestor index acutal para poder actualizar y irnos al siguiente nivel tenemos que poner index - 1 porque el vector va de 0 a n-1
            GameManager.instance.updateCurrentIndex(level_index);
            GameManager.instance.GoToNextLevel();
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