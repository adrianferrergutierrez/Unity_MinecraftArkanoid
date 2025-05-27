using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SceneMainMenu : MonoBehaviour
{
    public Button boton_retart;
    public Button boton_options;
    public TextMeshProUGUI highScoreText;
    private const string HighScoreKey = "MaxPuntuacion"; //�misma clave que en game manager

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
  
        // Cargamos la puntuaci�n guardada. Si no hay ninguna, el valor por defecto es 0.
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (highScoreText != null)
        {
            highScoreText.text = "Puntuaci�n M�xima: " + highScore.ToString();
        }
        else
        {
            Debug.LogWarning("El campo de texto para el r�cord no est� asignado en el script SceneMainMenu.");
        }
    }

    public void empezar_juego() {
        GameManager.instance.StartGameFromMenu();
        
    }
}
