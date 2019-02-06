using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MobileFramework;

public class MainMenuManager : MonoBehaviour
{


    // Use this for initialization
    void Awake()
    {
        AdManager.Instance.ShowBanner(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayGame()
    {
        FirebaseEventsHandler.Instance.GameStartedEvent();
        SceneManager.LoadScene("Main");
    }
}
