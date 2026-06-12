using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{   
    public static MainMenuManager instance;
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject secondaryButtons;
    [SerializeField] private TMP_Text alertText;
    [SerializeField] private Button continueButton;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip backgroundMusic;
    private AudioSource audioSource;

    private string savePath;
    private bool gameContinued = false;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public bool GameContinued() => gameContinued;

    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        audioSource.volume = 0.5f;
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();

        ShowMainButtons();

        savePath = Application.persistentDataPath + "/save.json";
        if (!File.Exists(savePath)) 
            continueButton.interactable = false;
        else
            print("Hay partida");
    }
    
    // Primary buttons
    public void TryNewGame()
    {   
        audioSource.PlayOneShot(buttonSound);
        if (File.Exists(savePath))
            ShowAlert();
        else
            SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        audioSource.PlayOneShot(buttonSound);
        gameContinued = true;
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(buttonSound);
        Application.Quit();
    }

    // Secondary buttons
    public void Confirm()
    {
        audioSource.PlayOneShot(buttonSound);
        File.Delete(savePath);
        SceneManager.LoadScene("GameScene");
    }

    public void Cancel()
    {
        audioSource.PlayOneShot(buttonSound);
        ShowMainButtons();
    }

    private void ShowAlert()
    {   
        print("Entra");
        mainButtons.SetActive(false);
        secondaryButtons.SetActive(true);
        alertText.text = "Are you sure?\nThe saved game will be deleted";
    }

    private void ShowMainButtons()
    {
        mainButtons.SetActive(true);
        secondaryButtons.SetActive(false);
        alertText.text = "";
    }
}
