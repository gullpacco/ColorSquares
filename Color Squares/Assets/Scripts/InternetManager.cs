using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;

public class InternetManager : MonoBehaviour
{
    static InternetManager instance;
    public bool IsInternetAvailable
    {
        get; private set;
    }
    bool firstConnectionPerformed;
    bool checking;
    private bool updateRequested;
    public delegate void ConnnectionStatusChange();
    public event ConnnectionStatusChange OnInternetFirstConnected;
    public event ConnnectionStatusChange OnInternetRecoverConnection;
    public event ConnnectionStatusChange OnInternetConnectionLost;

    public static InternetManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(checkInternetConnection());
        instance = this;
    }


    IEnumerator checkInternetConnection()
    {
        checking = true;
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error == null)
        {
            Debug.Log("Internet Recovered");
            OnInternetRecoverConnection();
            checking = false;
            updateRequested = false;
            if (!firstConnectionPerformed)
            {
                IsInternetAvailable = true;
                firstConnectionPerformed = true;
                //                OnInternetFirstConnected();
            }
            else
            {
                if (!IsInternetAvailable)
                {
                    IsInternetAvailable = true;
                }
            }
        }
        else
        {
            Debug.Log("No internet");

            if (IsInternetAvailable)
            {
                IsInternetAvailable = false;
                OnInternetConnectionLost();
            }
            StartCoroutine(checkInternetConnection());
        }
    }

    public void RequestUpdate()
    {
        updateRequested = true;
    }


    private void LateUpdate()
    {
        if(updateRequested && !checking)
        {
            StartCoroutine(checkInternetConnection());
        }
    }
    void Start()
    {
    }

}
