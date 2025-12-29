using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGameInput : MonoBehaviour
{
    public event Action OnGamePaused;
    public event Action OnGameResume;
    public static Action OnLoadMinigame;
    public static Action OnUnloadMinigame;
    public static PauseGameInput Instance { get; private set; }
    [SerializeField] private GameObject pauseMenuPanel;

    [SerializeField] private static bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !SceneManager.GetSceneByName("DemoMainMenu").isLoaded)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    

    public void PauseGame()
    {
        OnGamePaused?.Invoke();
        pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;                  
    }

    public void ResumeGame()
    {
        OnGameResume?.Invoke();
        pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;                 
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("DemoMainMenu");
    }
}
