﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace TileMadness
{
    public class GameManager : MonoBehaviour
    {

        private float timeLeft;
        private int currentLevel;
        private int validTilesLeft;
        private static GameManager instance;
        private float allowedTime = 2f;
        private float currentTime;
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
            GameOver();
        }

        public void GameOver()
        {
            LockGame();
            Debug.LogError("Game over");
            bool isAdReady = AdManager.Instance.IsAdReady();
            bool isHiScore = SaveManager.Instance.IsHiScore(currentLevel);
            GUIManager.instance.EnableGameOver(currentLevel, isHiScore, isAdReady);
        }

        void StartNewLevel()
        {
            currentLevel++;
            GUIManager.instance.UpdateCurrentLevelText(currentLevel);
            validTilesLeft = TileManager.Instance.SpawnTileset();
        }

        void AddValidTile(TileElement tile)
        {
            validTilesLeft--;
            tile.DeSpawn();
        }

        void IncreaseLevel()
        {
            currentLevel++;
        }

        public void CheckValidTile(TileElement tile)
        {
            if (!gameLocked && tile.Color != Color.None)
            {
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
        public void ToggleSound()
        {
            bool on = true;
            GUIManager.instance.ToggleSoundSprite(on);
        }
        public void ToggleMusic()
        {
            bool on = true;
            GUIManager.instance.ToggleMusicSprite(on);
        }
        public void BackToMain()
        {
            //TODO load main menu
            SceneManager.LoadScene("MainMenu");
        }
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void ContinueGame()
        {
            AdManager.Instance.ShowAd();
        }
        public void OnAdEnd()
        {
            GUIManager.instance.EnableResumeAfterAd();
        }
        public void ResumeAfterAd()
        {
            currentTime = 0;
            GUIManager.instance.DisableGameOver();
            UnlockGame();
        }
    }
}