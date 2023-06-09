using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        // options to load checkpoint in other functions?
        // weird scene loading problems. 
        // 1. StartMenu scene seems to be holding onto controls in some way
        // 2. Lighting on game scene is not right
        // SceneManager.UnloadSceneAsync(0); // makes no difference, async or not
        Debug.Log("Loading Scene");
        SceneManager.LoadScene("Lv1_city");    // single mode should unload all other scenes anyways, and is default
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
