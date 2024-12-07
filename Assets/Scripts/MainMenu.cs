using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Funzioni per pulsanti del Main Menu
public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Carica la scena principale
        SceneManager.LoadSceneAsync("Main Level");
    }

    public void QuitGame()
    {
        // Ignorata dall'editor
        Application.Quit();
    }
}
