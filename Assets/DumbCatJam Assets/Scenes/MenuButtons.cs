using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {
    public string sceneToLoad;
    public FadeImageInOut canvasFader;

    public GameObject Menu;
    bool m_isPaused = false;
    public bool m_isPauseMenu = true;
    private void Awake() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
    }
    void Start () {        
        if (Menu != null && m_isPauseMenu == true)
        {
            ToggleMenu(false);
        }
        m_isPaused = false;
        Time.timeScale = 1;
    }
	
	// Update is called once per frame
	void Update () {
        if (Menu != null && m_isPauseMenu == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //print("MenuCalled");
                if (m_isPaused == false)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Menu.SetActive(true);
                    m_isPaused = true;
                    Time.timeScale = 0;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Menu.SetActive(false);
                    m_isPaused = false;
                    Time.timeScale = 1;
                }
            }
        }
		
	}

    IEnumerator FirstLevelStarts()
    {
        canvasFader.FadeIn();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneToLoad);
        yield return null;
    }

    IEnumerator EnterCredits()
    {
        canvasFader.FadeIn();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Credits");
        yield return null;
    }
    IEnumerator EntermainMenu()
    {
        canvasFader.FadeIn();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("StartTemp");
        yield return null;
    }
    IEnumerator RestartLevel()
    {
        canvasFader.FadeIn();
        string LevelName = SceneManager.GetActiveScene().name;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(LevelName);
        yield return null;
    }

    public void FirstLevels()
    {
        ToggleMenu(false);

        Time.timeScale = 1;

        StartCoroutine(FirstLevelStarts());
    }
    public void Retry()
    {
        ToggleMenu(false);
        Time.timeScale = 1;

        StartCoroutine(RestartLevel());

    }

    public void Credits()
    {
        ToggleMenu(false);
        Time.timeScale = 1;

        StartCoroutine(EnterCredits());
    }
    public void mainMenu()
    {
        ToggleMenu(false);
        Time.timeScale = 1;

        StartCoroutine(EntermainMenu());
    }

    public void ToggleMenu(bool _toggled)
    {
        if (Menu != null && m_isPauseMenu == true)
        {
            Menu.SetActive(_toggled);
        }
    }
    public void CloseGame()
    {

        Time.timeScale = 1;

        Application.Quit();
    }
    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        Menu.SetActive(false);
        m_isPaused = false;
        Time.timeScale = 1;
    }
}
