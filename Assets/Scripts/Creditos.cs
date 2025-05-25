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
