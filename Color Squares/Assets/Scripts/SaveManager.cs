using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    private SaveData mySaveData;

    public static SaveManager Instance
    {
        get { return instance; }
    }
    public int HiScore
    {
        get { return mySaveData.HiScore; }
    }
    class SaveData
    {
        private int hiScore;
        private bool musicOn;
        private bool soundOn;
        public int HiScore
        {
            get { return hiScore; }
            set { hiScore = value; }
        }
        public bool MusicOn
        {
            get { return musicOn; }
            set { musicOn = value; }
        }
        public bool SoundOn
        {
            get { return soundOn; }
            set { soundOn = value; }
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
        Load();
    }

    public bool IsHiScore(int hiScore)
    {
        if (mySaveData.HiScore < hiScore)
        {
            SetHighScore(hiScore);
            return true;
        }
        return false;
    }

    public void SetHighScore(int hiScore)
    {
        mySaveData.HiScore = hiScore;
    }

    private void Save()
    {
        //TODO implement save
    }
    private void Load()
    {
        //TODO implement load
        if (mySaveData == null)
        {
            mySaveData = new SaveData();
            AudioManager.Instance.LoadMusic(true);
            AudioManager.Instance.LoadSound(true);
        }
        else
        {
            AudioManager.Instance.LoadMusic(mySaveData.MusicOn);
            AudioManager.Instance.LoadSound(mySaveData.SoundOn);
        }
    }

}
