using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TileMadness
{
    public class GUIManager : MonoBehaviour
    {

        public Text tutorialText;
        public Text currentLevelText;
        public Image tutorialBakgroundPanel;
        public Button tutorialContinueButton;
        public Slider timerSlider;
        public static GUIManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void ShowTutorial(string _tutorialText, Color backgroundColor, Color continueColor)
        {
            GameManager.Instance.LockGame();
            tutorialText.text = _tutorialText;
            tutorialBakgroundPanel.gameObject.SetActive(true);
            tutorialBakgroundPanel.color = TileManager.Instance.colors[(int)backgroundColor];
            tutorialContinueButton.image.sprite = TileManager.Instance.tileSprites[(int)continueColor];
        }


        public void HideTutorial()
        {
            tutorialText.text = "";
            tutorialBakgroundPanel.gameObject.SetActive(false);
            GameManager.Instance.UnlockGame();
        }

        public void UpdateTimerSlider(float time)
        {
            timerSlider.value = time;
        }

        public void UpdateCurrentLevelText(int levelNumber)
        {
            currentLevelText.text = "" + levelNumber;
        }
    }
}
