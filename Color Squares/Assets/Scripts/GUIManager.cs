using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TileMadness
{
    public class GUIManager : MonoBehaviour
    {
        bool specialTutorial;
        bool specialTutorialDone;
        [Header("Texts")]
        public Text tutorialText;
        public Text currentLevelText;
        public static GUIManager instance;
        public Text gameOverScore;
        public Text hiScore;
        public Text newBestScore;
       

        [Header("Panels")]
        public GameObject gameOverPanel;
        public GameObject pausePanel;
        public GameObject pauseButton;
        public GameObject bestScoreSection;
        public GameObject noBestScoreSection;
        [Header("Buttons")]
        public Button soundButton;
        public Button musicButton;
        public Button tutorialContinueButton;
        public Button gameOverResumeButton;
        public Button gameOverContinueButton;
        [Header("Sprites")]
        public Sprite soundOnSprite;
        public Sprite soundOffSprite;
        public Sprite musicOnSprite;
        public Sprite musicOffSprite;
        [Header("Other Elements")]
        public Slider timerSlider;
        public Image tutorialBakgroundPanel;


        private void Awake()
        {
            instance = this;
        }

        public void ShowTutorial(string _tutorialText, Color backgroundColor, Color continueColor)
        {
            pauseButton.SetActive(false);
            if (backgroundColor == Color.Red && !specialTutorialDone)
            {
                specialTutorial = true;
                GameManager.Instance.SpecialTutorial();
                tutorialContinueButton.gameObject.SetActive(false);
            }
            else
            {
                GameManager.Instance.LockGame();
                tutorialContinueButton.gameObject.SetActive(true);
            }
            tutorialText.text = _tutorialText.Replace("NEWLINE", "\n");
            tutorialBakgroundPanel.gameObject.SetActive(true);
            tutorialBakgroundPanel.color = TileManager.Instance.colors[(int)backgroundColor];
            switch (backgroundColor)
            {
                case Color.White:
                    tutorialText.color = UnityEngine.Color.black;
                    break;
                case Color.Black:
                    tutorialText.color = UnityEngine.Color.white;
                    break;
                case Color.Red:
                    goto case Color.White;
                case Color.Green:
                    goto case Color.White;
            }
            tutorialContinueButton.image.sprite = TileManager.Instance.tileSprites[(int)continueColor];
        }


        public void HideTutorial(bool tutorialEnded = false)
        {
            pauseButton.SetActive(true);
            if (!specialTutorial || tutorialEnded)
            {
                if (specialTutorial)
                {
                    specialTutorial = false;
                    specialTutorialDone = true;
                }
                tutorialText.text = "";
                tutorialBakgroundPanel.gameObject.SetActive(false);
                GameManager.Instance.UnlockGame();
            }
        }

        public void UpdateTimerSlider(float time)
        {
            timerSlider.value = time;
        }

        public void UpdateCurrentLevelText(int levelNumber)
        {
            currentLevelText.text = "" + levelNumber;
        }

        public void EnablePausePanel()
        {
            pausePanel.SetActive(true);
            timerSlider.gameObject.SetActive(false);
            pauseButton.SetActive(false);
        }

        public void DisablePausePanel()
        {
            pausePanel.SetActive(false);
            timerSlider.gameObject.SetActive(true);
            pauseButton.SetActive(true);
        }

        public void ToggleSoundSprite(bool on)
        {
            soundButton.image.sprite = on ? soundOnSprite : soundOffSprite;
        }
        public void ToggleMusicSprite(bool on)
        {
            musicButton.image.sprite = on ? musicOnSprite : musicOffSprite;
        }

        public void EnableGameOver(int score, bool isHiScore, bool canContinue)
        {
            gameOverPanel.SetActive(true);
            if (isHiScore)
            {
                //TODO implement hi score
                bestScoreSection.SetActive(true);
                noBestScoreSection.SetActive(false);
                newBestScore.text = "" + score;
            }
            else
            {
                bestScoreSection.SetActive(false);
                noBestScoreSection.SetActive(true);
                hiScore.text = "" + SaveManager.Instance.HiScore;
                gameOverScore.text = "" + score;
            }
            if (!canContinue)
            {
                gameOverContinueButton.gameObject.SetActive(false);
            }
            else
            {
                gameOverContinueButton.gameObject.SetActive(true);
            }
        }
        public void DisableGameOver()
        {
            //gameOverResumeButton.gameObject.SetActive(false);
            //gameOverContinueButton.gameObject.SetActive(true);
            gameOverPanel.SetActive(false);
        }

        //public void EnableResumeAfterAd()
        //{
        //    gameOverResumeButton.gameObject.SetActive(true);
        //    gameOverContinueButton.gameObject.SetActive(false);
        //}
    }
}
