using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;


namespace MobileFramework
{
    public class FirebaseEventsHandler : MonoBehaviour
    {
        [SerializeField]
        FirebaseManager firebaseManager;
        static FirebaseEventsHandler instance;
        public static FirebaseEventsHandler Instance
        {
            get { return instance; }
        }

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        #region Custom events
        public void TapTileEvent()
        {
            if (firebaseManager.IsFirebaseReady())
            {
                FirebaseAnalytics.LogEvent("Tap");
            }
        }
        public void ErrorEvent()
        {
            if (firebaseManager.IsFirebaseReady())
            {
                FirebaseAnalytics.LogEvent("Error");
            }
        }
        public void ReviveRequestEvent()
        {
            if (firebaseManager.IsFirebaseReady())
            {
                FirebaseAnalytics.LogEvent("Revive Requested");
            }
        }
        public void ReviveCompletedEvent()
        {
            if (firebaseManager.IsFirebaseReady())
            {
                FirebaseAnalytics.LogEvent("Revive Completed");
            }
        }
        public void GameStartedEvent()
        {
            if (firebaseManager.IsFirebaseReady())
            {
                FirebaseAnalytics.LogEvent("Game Started");
            }
        }
        public void FirebaseLevelEventEnd(int level)
        {
            if (firebaseManager.IsFirebaseReady())
            {
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, FirebaseAnalytics.ParameterLevel, level);
            }
        }
        #endregion
    }
}