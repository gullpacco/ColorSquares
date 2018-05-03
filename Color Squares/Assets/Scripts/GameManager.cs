using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
            currentTime += Time.deltaTime;
            if (currentTime > allowedTime)
            {
                EndLevel();
            }
            GUIManager.instance.UpdateTimerSlider((allowedTime - currentTime) / (allowedTime));
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
            Debug.LogError("Game over");
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
            if (tile.Color != Color.None)
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

        }
        public void LockGame()
        {
            Time.timeScale = 0;
        }

        public void UnlockGame()
        {
            Time.timeScale = 1;
        }
    }

}