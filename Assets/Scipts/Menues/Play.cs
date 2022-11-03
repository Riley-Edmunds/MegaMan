using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    public Button startButton;
    public void CallPlay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(4);
    }

}
