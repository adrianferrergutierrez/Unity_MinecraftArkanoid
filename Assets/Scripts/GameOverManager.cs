using UnityEngine;
using TMPro;


public class GameOverManager : MonoBehaviour
{

    public TextMeshProUGUI puntuacionFinalText; 

    void Start()
    {
       
        if (puntuacionFinalText != null && GameManager.instance != null)
        {
            puntuacionFinalText.text = "Puntuaci�n: " + GameManager.instance.puntuacion.ToString();
        }
        else if (puntuacionFinalText != null)
        {
            puntuacionFinalText.text = "Puntuaci�n: N/A";
        }
    }

   public void empezar_juego()
    {
        GameManager.instance.GoToMainMenu();

    }
}
