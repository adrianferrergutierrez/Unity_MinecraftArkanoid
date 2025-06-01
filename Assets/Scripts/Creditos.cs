using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Creditos : MonoBehaviour
{
    public Button botonMenu;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void IrAMenuPrincipal()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GoToMainMenu();
        }
    }
}
