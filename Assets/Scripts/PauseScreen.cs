using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }
}
