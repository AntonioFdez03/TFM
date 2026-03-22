using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public void Resume()
    {
        UIController.instance.SetState(UIState.Gameplay);
    }

    public void Settings()
    {
        
    }

    public void Save()
    {
        
    }
    public void Quit()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
