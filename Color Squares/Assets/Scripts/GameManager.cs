using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MobileFramework;

namespace TileMadness
{
    public class GameManager : MonoBehaviour
    {
        static int gamesEnded;
        private float timeLeft;
        private int currentLevel;
        private int validTilesLeft;
        private static GameManager instance;
        private float allowedTime = 2f;
        private float currentTime;
        bool revived;
        private bool specialTutorial = false;
        private float specialTime;
        private float allowedSpecialTime = 5f;
        private bool gameLocked;
        public static GameManager Instance
        {
            get { return instance; }
        }
        private void Awake()
        {
            instance = this;
        }
        public int CurrentLevel
        {
            get { return currentLevel; }
        }
        // Use this for initialization
        void Start()
        {
            SaveManager.Instance.IncreaseGamesPlayed();
            StartNewLevel();
        }
        // Update is called once per frame
        void Update()
        {
            UpdateTime();
        }
        void UpdateTime()
        {
            if (specialTutorial)
            {
                specialTime += Time.unscaledDeltaTime;
                if (specialTime > allowedSpecialTime)
                {
                    GUIManager.instance.HideTutorial(true);
                    specialTutorial = false;
                }
                GUIManager.instance.UpdateTimerSlider((allowedSpecialTime - specialTime) / (allowedSpecialTime));
            }
            else if (!gameLocked)
            {
                currentTime += Time.deltaTime;
                if (currentTime > allowedTime)
                {
                    EndLevel();
                }
                GUIManager.instance.UpdateTimerSlider((allowedTime - currentTime) / (allowedTime));
            }
        }
        void EndLevel()
        {
            currentTime = 0;
            if (CheckLevelComplete())
            {
                SaveManager.Instance.IncreaseLevelsCompleted();
                               FirebaseEventsHandler.Instance.FirebaseLevelEventEnd(currentLevel);
                StartNewLevel();
            }
            else
            {
                AddError();
            }
        }
        bool CheckLevelComplete()
        {
            return validTilesLeft < 1;
        }
        void Pause()
        {

        }
        public void AddError()
        {
            AudioManager.Instance.PlaySound(0);
            FirebaseEventsHandler.Instance.TapTileEvent();
            FirebaseEventsHandler.Instance.ErrorEvent();
            gamesEnded++;
            if(gamesEnded>3)
            {
                AdManager.Instance.ShowInterstitial(0);
            }
            GameOver();
        }
        public void GameOver()
        {
            LockGame();
            Debug.LogError("Game over");
            bool isAdReady = AdManager.Instance.IsRewardedAdLoaded(0) && !revived;
            bool isHiScore = SaveManager.Instance.IsHiScore(currentLevel);
            GUIManager.instance.EnableGameOver(currentLevel, isHiScore, isAdReady);
        }
        void StartNewLevel()
        {
            TileManager.Instance.CheckTutorialCompleted();
            currentLevel++;
            if (SaveManager.Instance.IsHiScore(currentLevel))
            {
                //TODO CALL LEADERBOARD
            }
            GUIManager.instance.UpdateCurrentLevelText(currentLevel);
            validTilesLeft = TileManager.Instance.SpawnTileset();
        }
        void AddValidTile(TileElement tile)
        {
            AudioManager.Instance.PlaySound(1);
            validTilesLeft--;
            tile.DeSpawn();
            FirebaseEventsHandler.Instance.TapTileEvent();
        }
        void IncreaseLevel()
        {
            currentLevel++;
        }
        public void CheckValidTile(TileElement tile)
        {
            if (!gameLocked && tile.Color != Color.None)
            {
                SaveManager.Instance.IncreaseTapCount(tile.Color.ToString());
                switch (TileManager.Instance.BackgroundState)
                {
                    case BackGroundState.Full:
                        if (!CheckValidColors(tile.Color, TileManager.Instance.FullBackgroundColor))
                        {
                            AddError();
                        }
                        else
                        {
                            AddValidTile(tile);
                        }
                        break;
                    case BackGroundState.HorizontalSplit:
                        if (tile.Section == 1 || tile.Section == 3)
                        {
                            if (!CheckValidColors(tile.Color, TileManager.Instance.LeftBackgroundColor))
                            {
                                AddError();
                            }
                            else
                            {
                                AddValidTile(tile);
                            }
                        }
                        else
                        {
                            if (!CheckValidColors(tile.Color, TileManager.Instance.RightBackgroundColor))
                            {
                                AddError();
                            }
                            else
                            {
                                AddValidTile(tile);
                            }
                        }
                        break;
                    case BackGroundState.VerticalSplit:
                        if (tile.Section == 1 || tile.Section == 2)
                        {
                            if (!CheckValidColors(tile.Color, TileManager.Instance.UpBackgroundColor))
                            {
                                AddError();
                            }
                            else
                            {
                                AddValidTile(tile);
                            }
                        }
                        else
                        {
                            if (!CheckValidColors(tile.Color, TileManager.Instance.DownBackgroundColor))
                            {
                                AddError();
                            }
                            else
                            {
                                AddValidTile(tile);
                            }
                        }
                        break;
                }
            }
        }
        public bool CheckValidColors(Color tileColor, Color backgroundColor)
        {
            switch (backgroundColor)
            {
                case Color.White:
                    return (tileColor == Color.Green || tileColor == Color.Blue);
                case Color.Black:
                    return (tileColor == Color.Red || tileColor == Color.Blue);
                case Color.Green:
                    return true;
                case Color.Red:
                    return (tileColor == Color.Blue);
            }
            return true;
        }
        public void PauseGame()
        {
            LockGame();
            GUIManager.instance.EnablePausePanel();
        }
        public void ResumeGame()
        {
            GUIManager.instance.DisablePausePanel();
            UnlockGame();
        }
        public void LockGame()
        {
            gameLocked = true;
            Time.timeScale = 0;
        }
        public void UnlockGame()
        {
            StartCoroutine(UnlockDelayed());
        }
        private IEnumerator UnlockDelayed()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            gameLocked = false;
            Time.timeScale = 1;
        }
        public void SpecialTutorial()
        {
            LockGame();
            specialTutorial = true;
        }
        public void BackToMain()
        {
            AudioManager.Instance.PlaySound(2);
            SceneManager.LoadScene("MainMenu");
        }
        public void RestartGame()
        {
            AudioManager.Instance.PlaySound(2);
            if (gamesEnded > 3)
            {
                AdManager.Instance.HideInterstitial(0);
                gamesEnded = 0;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void ContinueGame()
        {
            AudioManager.Instance.PlaySound(2);
            if (gamesEnded > 3)
            {
                AdManager.Instance.HideInterstitial(0);
                gamesEnded = 0;
            }
            FirebaseEventsHandler.Instance.ReviveRequestEvent();
            StartCoroutine(WaitForAdToClose());
            AdManager.Instance.ShowRewardedAds(0);
        }
        public void ResumeAfterAd()
        {
            FirebaseEventsHandler.Instance.ReviveCompletedEvent();
            revived = true;
            currentTime = 0;
            GUIManager.instance.DisableGameOver();
            UnlockGame();
        }
        IEnumerator WaitForAdToClose()
        {
            yield return new WaitUntil(() => AdManager.Instance.IsRewardedAdClosed(0));
            if (AdManager.Instance.IsRewardedAdRewarded(0))
            {
                ResumeAfterAd();
            }
        }
    }
}