using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class MainMenuManager : MonoBehaviour
{   
    public static MainMenuManager instance;
    [SerializeField] private TMP_Text startButtonText;
    [SerializeField] private AudioClip buttonSound;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
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
        audioSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(buttonSound);
        Application.Quit();
    }
}
