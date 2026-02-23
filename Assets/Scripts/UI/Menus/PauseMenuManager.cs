using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] UIController controller;
    public void Resume()
    {
        controller.SetState(UIController.UIState.Gameplay);
    }

    public void Settings()
    {
        
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
