using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button BMButton;
    public Button IMButton;
    public Button GMButton;
    public Button CMButton;
    public Button EMButton;
    public Button FMButton;
    //Calls each level upon selection
    public void CallBM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(6);
    }
    public void CallIM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(5);
    }
    public void CallGM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(10);
    }
    public void CallCM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(7);
    }
    public void CallEM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(8);
    }
    public void CallFM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(9);
    }
}
