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
            AudioManager.Instance.PlaySound(2);
            UpdateMusicSprites(SaveManager.Instance.MusicOn);
            UpdateSoundSprites(SaveManager.Instance.SoundOn);
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
        public void ToggleSound()
        {
            bool state = SaveManager.Instance.ToggleSound();
            UpdateSoundSprites(state);
            AudioManager.Instance.PlaySound(2);
        }
        public void ToggleMusic()
        {
            AudioManager.Instance.PlaySound(2);
            bool state = SaveManager.Instance.ToggleMusic();
            UpdateMusicSprites(state);
        }
        void UpdateMusicSprites(bool state)
        {
            if (state)
            {
                musicButton.image.sprite = musicOnSprite;
            }
            else
            {
                musicButton.image.sprite = musicOffSprite;
            }
        }
        void UpdateSoundSprites(bool state)
        {
            if (state)
            {
                soundButton.image.sprite = soundOnSprite;
            }
            else
            {
                soundButton.image.sprite = soundOffSprite;
            }
        }
        public void EnableGameOver(int score, bool isHiScore, bool canContinue)
        {
            gameOverPanel.SetActive(true);
            pauseButton.SetActive(false);
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
            gameOverPanel.SetActive(false);
            pauseButton.SetActive(true);
        }
        //public void EnableResumeAfterAd()
        //{
        //    gameOverResumeButton.gameObject.SetActive(true);
        //    gameOverContinueButton.gameObject.SetActive(false);
        //}
    }
}
