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

public class Win_script : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI puntuacionFinalText; // Arrastra aqu� el texto de la puntuaci�n
    public Button botonCreditos;
    public Button botonMenu;

    void Start()
    {
        // Mostrar puntuaci�n final
        if (puntuacionFinalText != null && GameManager.instance != null)
        {
            puntuacionFinalText.text = "Puntuaci�n Final: " + GameManager.instance.puntuacion.ToString();
        }

    }

    public void IrACreditos()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GoToCreditsScreen(); // Necesitaremos a�adir este m�todo al GameManager
        }
    }

    public void IrAMenuPrincipal()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GoToMainMenu();
        }
    }

}
