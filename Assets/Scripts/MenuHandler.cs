using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public Image playOn;
    public Image playOff;
    public Image exitOn;
    public Image exitOff;


    private void OnMouseOver()
    {
        if (gameObject.name == "PlayButton")
        {

        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
