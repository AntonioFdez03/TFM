using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    public void ToggleFPS()
    {   
        print("Entra");
        GameplayUI.instance.ToggleFPS();
    }
}
