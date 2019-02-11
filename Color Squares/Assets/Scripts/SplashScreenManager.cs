using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobileFramework;

public class SplashScreenManager : MonoBehaviour
{
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Invoke("LoadScene", 2f);
    }
    void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
