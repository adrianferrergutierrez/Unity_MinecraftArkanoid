using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//cogemos la libreria especifica de las escenas
using UnityEngine.SceneManagement;
//importamos una nueva LIBRERIA, tenemos las funciones y clases de Text MEsh Pro
using TMPro;

//libreria qyue nois permite interactuar direcamente con el boton
using UnityEngine.UI;

public class SceneMainMenu : MonoBehaviour
{
    public Button boton_retart;
    public Button boton_options;
    public TextMeshProUGUI highScoreText;

    // La misma clave que usamos en GameManager. �Es crucial que sea id�ntica!
    private const string HighScoreKey = "MaxPuntuacion";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
      
        // Cargamos la puntuaci�n guardada. Si no hay ninguna, el valor por defecto es 0.
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        // Actualizamos el texto en la UI para que muestre el r�cord.
        if (highScoreText != null)
        {
            highScoreText.text = "Puntuaci�n M�xima: " + highScore.ToString();
        }
        else
        {
            Debug.LogWarning("El campo de texto para el r�cord no est� asignado en el script SceneMainMenu.");
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void empezar_juego() {
        GameManager.instance.StartGameFromMenu();
        
    }
}
