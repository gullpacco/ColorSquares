using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;


namespace MobileFramework
{
    public class FirebaseManager : MonoBehaviour
    {
        public enum FirebaseEvents { Login }
        FirebaseApp FirebaseApp;
        bool Initialized;
        private void Awake()
        {
            Init();
        }
        void Init()
        {
            if (Initialized)
            {
                return;
            }
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    FirebaseApp = FirebaseApp.DefaultInstance;
                    Initialized = true;
                    InternetManager.Instance.OnInternetRecoverConnection -= Init;
                }
                else
                {
                    if (!InternetManager.Instance.IsInternetAvailable)
                    {
                        InternetManager.Instance.OnInternetRecoverConnection += Init;
                    }
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
        }
        public bool IsFirebaseReady()
        {
            if (Initialized)
            {
                return true;
            }
            else
            {
                Init();
                return Initialized;
            }
        }
    }
}