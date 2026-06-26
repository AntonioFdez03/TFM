using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{   
    private static bool gameContinued = false;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject secondaryButtons;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private TMP_Text alertText;
    [SerializeField] private GameObject Keys;
    [SerializeField] private Button continueButton;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip backgroundMusic;
    private AudioSource audioSource;
    private InputAction space;
    private InputAction escape;

    private Vector3 titleInitPosition;
    private Vector3 creditsInitPosition;
    private float titleWaitTime = 10f;
    private float creditsSpeed = 30f;
    private float timer = 0;

    private string savePath;
    private ScreenFaderController fader;


    public static bool GameContinued() => gameContinued;

    void Start()
    {   
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        audioSource.volume = 0.5f;
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();

        space = InputSystem.actions.FindAction("Jump");
        escape = InputSystem.actions.FindAction("Pause");

        titleInitPosition = title.transform.position;
        creditsInitPosition = creditsPanel.transform.position;

        fader = GetComponent<ScreenFaderController>();

        ShowMainButtons();

        savePath = Application.persistentDataPath + "/save.json";
        if (!File.Exists(savePath)) 
            continueButton.interactable = false;
        else
            print("Hay partida");
    }

    void Update()
    {
        if(!creditsPanel.activeSelf)
            return;

        if (escape.WasPressedThisFrame() || space.WasPressedThisFrame())
        {
            ShowMainButtons();
            return;
        }

        creditsPanel.transform.position += Vector3.up * creditsSpeed * Time.deltaTime;

        timer += Time.deltaTime;

        if(timer >= titleWaitTime)
            title.transform.position += Vector3.up * creditsSpeed * Time.deltaTime;
    }

    // Primary buttons
    public void TryNewGame()
    {   
        audioSource.PlayOneShot(buttonSound);
        if (File.Exists(savePath))
            ShowAlert();
        else
        {   
            StartCoroutine(FadeCooldownCR());
        }
    }

    public void ContinueGame()
    {
        audioSource.PlayOneShot(buttonSound);
        gameContinued = true;
        StartCoroutine(FadeCooldownCR());
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(buttonSound);
        Application.Quit();
    }

    public void Credits()
    {
        audioSource.PlayOneShot(buttonSound);

        Keys.SetActive(true);
        creditsPanel.SetActive(true);
        mainButtons.SetActive(false);
        secondaryButtons.SetActive(false);
        alertText.gameObject.SetActive(false);

        timer = 0;
    }

    // Secondary buttons
    public void Confirm()
    {
        audioSource.PlayOneShot(buttonSound);
        File.Delete(savePath);
        StartCoroutine(FadeCooldownCR());
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
        EventSystem.current.SetSelectedGameObject(null);
        
        mainButtons.SetActive(true);
        alertText.gameObject.SetActive(true);
        secondaryButtons.SetActive(false);
        creditsPanel.gameObject.SetActive(false);
        alertText.text = "";
        Keys.SetActive(false);

        title.transform.position = titleInitPosition;
        creditsPanel.transform.position = creditsInitPosition;
    }

    private IEnumerator FadeCooldownCR()
    {   
        fader.FadeOut();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("GameScene");
    }
}
