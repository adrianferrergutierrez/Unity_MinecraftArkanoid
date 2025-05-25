using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//cogemos la libreria especifica de las escenas
using UnityEngine.SceneManagement;
//importamos una nueva LIBRERIA, tenemos las funciones y clases de Text MEsh Pro
using TMPro;

//libreria qyue nois permite interactuar direcamente con el boton
using UnityEngine.UI;


public class GameOverManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public TextMeshProUGUI puntuacionFinalText; // Arrastra aquí tu TextMeshPro para la puntuación


    void Start()
    {
        // Ocultar el cursor o mostrarlo según necesites
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;

        // --- MOSTRAR LA PUNTUACIÓN FINAL ---
        if (puntuacionFinalText != null && GameManager.instance != null)
        {
            puntuacionFinalText.text = "Puntuación: " + GameManager.instance.puntuacion.ToString();
        }
        else if (puntuacionFinalText != null)
        {
            puntuacionFinalText.text = "Puntuación: N/A";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void empezar_juego()
    {
        GameManager.instance.GoToMainMenu();

    }
}
