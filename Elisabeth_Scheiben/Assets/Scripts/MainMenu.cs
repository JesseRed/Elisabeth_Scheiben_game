using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameSession gameSession;

    private void Awake()
    {
//        print("Awake MainMenux");
        //this.gameObject
        GameObject tut = GameObject.Find("OptionsMenu");
//        print("name = " + tut.name);
        gameSession = FindObjectOfType<GameSession>();
//        print("name = " + gameSession.name);
        if (gameSession.isInitialized == false)
        {
            this.gameObject.SetActive(false);
            tut.SetActive(true);
        }
        if (gameSession.isInitialized == true)
        {
            this.gameObject.SetActive(true);
            tut.SetActive(false);
        }

    }

    public void StartExperiment()
    {

        gameSession = FindObjectOfType<GameSession>();
        gameSession.isTutorial = false;
        //gameSession.configureExperimentalDesign();
        //SceneManager.LoadScene("SRTTScene");
        // alternative
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartTutorial()
    {
        gameSession = FindObjectOfType<GameSession>();
        gameSession.isTutorial = true;
        SceneManager.LoadScene("SRTTScene");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void QuitProgram()
    {
        Debug.Log("quit");
        Application.Quit();
    }

}
