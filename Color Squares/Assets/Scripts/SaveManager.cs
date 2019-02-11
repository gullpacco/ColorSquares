using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    //private SaveData mySaveData;
    static class Keys
    {
        public static string InitKey = "Init";
        public static string SoundOnKey = "SoundOn";
        public static string MusicOnKey = "MusicOn";
        public static string HiScoreKey = "HiScore";
        public static string TapsKey = "Taps";
        public static string GreenTapsKey = "GreenTaps";
        public static string BlueTapsKey = "BlueTaps";
        public static string RedTapsKey = "RedTaps";
        public static string BlackTapsKey = "BlackTaps";
        public static string WhiteTapsKey = "WhiteTaps";
        public static string DaysPlayed = "DaysPlayed";
        public static string LastDayPlayed = "LastDayPlayed";
        public static string GamesPlayedKey = "GamesPlayed";
        public static string LevelsCompletedKey = "LevelsCompleted";
        public static string TutorialsDoneKey = "TutorialsDone";

    }
    public bool SoundOn
    {
        get { return PlayerPrefs.GetInt(Keys.SoundOnKey) == 1; }
    }
    public bool MusicOn
    {
        get { return PlayerPrefs.GetInt(Keys.MusicOnKey) == 1; }
    }
    public static SaveManager Instance
    {
        get { return instance; }
    }
    public int HiScore
    {
        get
        {
            return PlayerPrefs.GetInt("HiScore");
        }
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    private void Start()
    {
        Load();
    }
    public bool IsHiScore(int newScore)
    {
        if (HiScore < newScore)
        {
            SetHighScore(newScore);
            return true;
        }
        return false;
    }
    public int LastTutorialDone()
    {
        return PlayerPrefs.GetInt(Keys.TutorialsDoneKey);
    }
    public void SetTutorialDone(int index)
    {
        PlayerPrefs.SetInt(Keys.TutorialsDoneKey, index);
    }
    public void SetHighScore(int newScore)
    {
        PlayerPrefs.SetFloat(Keys.HiScoreKey, newScore);
    }
    public void IncreaseTapCount(string color)
    {
        int totalTaps = PlayerPrefs.GetInt(Keys.TapsKey);
        PlayerPrefs.SetInt(Keys.TapsKey, totalTaps + 1);
        string colorKey = color + Keys.TapsKey;
        int tapsByColor = PlayerPrefs.GetInt(colorKey);
        PlayerPrefs.SetInt(colorKey, tapsByColor + 1);
    }
    public void IncreaseGamesPlayed()
    {
        int games = PlayerPrefs.GetInt(Keys.GamesPlayedKey);
        PlayerPrefs.SetInt(Keys.GamesPlayedKey, games + 1);
    }
    public void IncreaseLevelsCompleted()
    {
        int levels = PlayerPrefs.GetInt(Keys.LevelsCompletedKey);
        PlayerPrefs.SetInt(Keys.LevelsCompletedKey, levels + 1);
    }
    void UpdateDaysPlayed()
    {
        PlayerPrefs.SetString(Keys.LastDayPlayed, System.DateTime.Today.ToString());
        int days = PlayerPrefs.GetInt(Keys.DaysPlayed);
        PlayerPrefs.SetInt(Keys.DaysPlayed, days + 1);
    }
    private void Load()
    {
        if (!PlayerPrefs.HasKey(Keys.InitKey))
        {
            InitKeys();
        }
        if (PlayerPrefs.GetString(Keys.LastDayPlayed) != System.DateTime.Today.ToString())
        {
            UpdateDaysPlayed();
        }
        AudioManager.Instance.LoadMusic(PlayerPrefs.GetInt("MusicOn") == 1);
        AudioManager.Instance.LoadSound(PlayerPrefs.GetInt("SoundOn") == 1);
    }
    void InitKeys()
    {
        PlayerPrefs.SetInt(Keys.InitKey, 1);
        PlayerPrefs.SetInt(Keys.HiScoreKey, 0);
        PlayerPrefs.SetInt(Keys.TapsKey, 0);
        PlayerPrefs.SetInt(Keys.BlueTapsKey, 0);
        PlayerPrefs.SetInt(Keys.RedTapsKey, 0);
        PlayerPrefs.SetInt(Keys.GreenTapsKey, 0);
        PlayerPrefs.SetInt(Keys.WhiteTapsKey, 0);
        PlayerPrefs.SetInt(Keys.BlackTapsKey, 0);
        PlayerPrefs.SetInt(Keys.TutorialsDoneKey, -1);
        PlayerPrefs.SetInt(Keys.DaysPlayed, 0);
        PlayerPrefs.SetString(Keys.LastDayPlayed, System.DateTime.Today.ToString());
        PlayerPrefs.SetInt(Keys.SoundOnKey, 1);
        PlayerPrefs.SetInt(Keys.MusicOnKey, 1);
    }
    public bool ToggleSound()
    {
        bool soundState = PlayerPrefs.GetInt(Keys.SoundOnKey) == 1;
        soundState = !soundState;
        PlayerPrefs.SetInt(Keys.SoundOnKey, soundState ? 1 : 0);
        AudioManager.Instance.ToggleSound(soundState);
        return soundState;
    }
    public bool ToggleMusic()
    {
        bool musicState = PlayerPrefs.GetInt(Keys.MusicOnKey) == 1;
        musicState = !musicState;
        PlayerPrefs.SetInt(Keys.MusicOnKey, musicState ? 1 : 0);
        AudioManager.Instance.ToggleMusic(musicState);
        return musicState;
    }
}
