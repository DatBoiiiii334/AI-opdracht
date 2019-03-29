using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayGame()
    {
        //Loads the next scene in the scene manager
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quitgame()
    {
        Debug.Log("game quit");
        Application.Quit();
    }

    public void Retry()
    {
        //Loads the next scene in the scene manager
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Menu()
    {
        //Loads the next scene in the scene manager
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
}
