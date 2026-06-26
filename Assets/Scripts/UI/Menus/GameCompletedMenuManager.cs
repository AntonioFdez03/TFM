using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameCompletedMenuManager : MonoBehaviour
{   
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private TMP_Text statsText;
    private float creditsSpeed = 30f;
    //private AudioSource audioSource;
    private InputAction exit;
    private ScreenFaderController fader;
    private bool canExit = true;

    void Start()
    {   
        if(File.Exists(Application.persistentDataPath + "/save.json"))
            File.Delete(Application.persistentDataPath + "/save.json");
        
        if(AudioManager.instance != null)
        {
            AudioManager.instance.Play(backgroundMusic, true);
        }

        fader = GetComponent<ScreenFaderController>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        exit = InputSystem.actions.FindAction("Pause");

        if(StatisticsController.instance != null)
            statsText.text = StatisticsController.instance.GetStats();
    }

    void Update()
    {
        if (exit.WasPressedThisFrame() && canExit)
        {
            StartCoroutine(FadeCooldownCR());
        }

        creditsPanel.transform.position += Vector3.up * creditsSpeed * Time.deltaTime;
    }


    private IEnumerator FadeCooldownCR()
    {   
        canExit = false;
        fader.FadeOut();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenuScene");
    }


}
