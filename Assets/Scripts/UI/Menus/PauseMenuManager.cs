using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text AlertMessage;

    public void Resume()
    {
        UIController.instance.SetState(UIState.Gameplay);
    }

    public void Settings()
    {
        UIController.instance.SetState(UIState.Settings);
    }

    public void Save()
    {
        if (SaveManager.instance.SaveGame())
        {
            StartCoroutine(MessageCR("Game saved"));
        }

    }
    public void Quit()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private IEnumerator MessageCR(string text)
    {   
        AlertMessage.gameObject.SetActive(true);
        AlertMessage.text = text;
        yield return new WaitForSecondsRealtime(3f);
        AlertMessage.gameObject.SetActive(false);
    }
}
