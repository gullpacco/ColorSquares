using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Advertisements;


public class AdManager : MonoBehaviour
{

    static AdManager instance;

    public static AdManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsAdReady()
    {
        return Advertisement.IsReady();
    }

    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            ShowOptions showOptions = new ShowOptions();
            showOptions.resultCallback += ResultCallback;
            Advertisement.Show(showOptions);
        }
        else
        {
            TileMadness.GameManager.Instance.OnAdEnd();
        }
    }


    private void ResultCallback(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            TileMadness.GameManager.Instance.OnAdEnd();
        }
        else
        {
            Debug.Log("No award given. Result was :: " + result);
        }
    }
}
