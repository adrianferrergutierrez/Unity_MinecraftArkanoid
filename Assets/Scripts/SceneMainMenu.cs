using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SceneMainMenu : MonoBehaviour
{
    public Button boton_retart;
    public Button boton_options;
    public TextMeshProUGUI highScoreText;
    private const string HighScoreKey = "MaxPuntuacion"; //çmisma clave que en game manager

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
  
        // Cargamos la puntuación guardada. Si no hay ninguna, el valor por defecto es 0.
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (highScoreText != null)
        {
            highScoreText.text = "Puntuación Máxima: " + highScore.ToString();
        }
        else
        {
            Debug.LogWarning("El campo de texto para el récord no está asignado en el script SceneMainMenu.");
        }
    }

    public void empezar_juego() {
        GameManager.instance.StartGameFromMenu();
        
    }
}
