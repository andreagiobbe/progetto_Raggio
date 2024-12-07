using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Gestione pulsanti schermata di pausa
public class PauseMenu : MonoBehaviour
{
    private GameObject pauseCanvas;         // oggetto canvas dello schermo di pausa
    public static bool isPaused = false;    // variabile per indicare se siamo in pausa

    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);   // all'inizio non siamo in pausa
    }

    // Update is called once per frame
    void Update()
    {
        // Premendo ESC si entra/esce dalla pausa
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // Mette in pausa
    public void PauseGame()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Uscire dalla pausa
    public void ResumeGame()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Torna al Main Menu
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
        isPaused = false;
    }

    // Chiude il "gioco"
    public void PauseQuit()
    {
        Application.Quit();
    }
}
