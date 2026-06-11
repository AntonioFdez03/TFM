using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuManager : MonoBehaviour
{   
    [SerializeField] private AudioClip buttonSound;
    private AudioSource audioSource;

    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void NewGame()
    {   
        audioSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnHome()
    {
        audioSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene("MainMenuScene");
    }
}
