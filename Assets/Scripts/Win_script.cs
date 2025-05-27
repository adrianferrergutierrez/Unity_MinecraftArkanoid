    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;

public class Win_script : MonoBehaviour
{
    public TextMeshProUGUI puntuacionFinalText; 
    public Button botonCreditos;
    public Button botonMenu;

    void Start()
    {
        // Mostrar puntuación final
        if (puntuacionFinalText != null && GameManager.instance != null)
        {
            puntuacionFinalText.text = "Puntuación Final: " + GameManager.instance.puntuacion.ToString();
        }

    }

    public void IrACreditos()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GoToCreditsScreen(); // Necesitaremos añadir este método al GameManager
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
