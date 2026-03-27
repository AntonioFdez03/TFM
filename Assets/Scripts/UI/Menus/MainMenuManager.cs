using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;
using TMPro;

public class MainMenuManager : MonoBehaviour
{   
    public static MainMenuManager instance;
    [SerializeField] private TMP_Text startButtonText;
    private bool gameDataFound;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        startButtonText.text = "New game";
        gameDataFound = false;
        if (File.Exists(Application.persistentDataPath + "/save.json"))
        {
            startButtonText.text = "Load game";
            gameDataFound = true;
        }
    }

    public bool GameDataFound() => gameDataFound;
    
    public void StartGame()
    {   
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
