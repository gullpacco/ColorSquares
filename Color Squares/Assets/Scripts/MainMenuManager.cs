using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MobileFramework;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Image soundImage;
    public Image musicImage;
    public GameObject creditsPanel;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    void Awake()
    {
        AdManager.Instance.ShowBanner(0);
    }
    public void Start()
    {
        UpdateMusicSprites(SaveManager.Instance.MusicOn);
        UpdateSoundSprites(SaveManager.Instance.SoundOn);
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
            musicImage.sprite = musicOnSprite;
        }
        else
        {
            musicImage.sprite = musicOffSprite;
        }
    }
    void UpdateSoundSprites(bool state)
    {
        if (state)
        {
            soundImage.sprite = soundOnSprite;
        }
        else
        {
            soundImage.sprite = soundOffSprite;
        }
    }
    public void PlayGame()
    {
        AudioManager.Instance.PlaySound(2);
        FirebaseEventsHandler.Instance.GameStartedEvent();
        SceneManager.LoadScene("Main");
    }
    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
    }
    public void HideCredits()
    {
        creditsPanel.SetActive(false);
    }
    public void Rate()
    {
        AudioManager.Instance.PlaySound(2);
    }
}
