using UnityEngine;
using TMPro;


public class GameOverManager : MonoBehaviour
{

    public TextMeshProUGUI puntuacionFinalText; 

    void Start()
    {
       
        if (puntuacionFinalText != null && GameManager.instance != null)
        {
            puntuacionFinalText.text = "Puntuación: " + GameManager.instance.puntuacion.ToString();
        }
        else if (puntuacionFinalText != null)
        {
            puntuacionFinalText.text = "Puntuación: N/A";
        }
    }

   public void empezar_juego()
    {
        GameManager.instance.GoToMainMenu();

    }
}
