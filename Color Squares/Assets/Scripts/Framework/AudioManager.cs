using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private bool musicOn;
    private bool soundOn;
    static AudioManager instance;
    public AudioSource music;
    public AudioClip[] sounds;

    public static AudioManager Instance
    {
        get { return instance; }
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

    public void PlaySound(int soundIndex)
    {
        if (soundOn)
        {
            if (soundIndex < sounds.Length)
            {
                AudioSource.PlayClipAtPoint(sounds[soundIndex], Camera.main.transform.position);
            }
        }
    }

    public bool ToggleSound()
    {
        soundOn = !soundOn;
        return soundOn;
    }

    public bool ToggleMusic()
    {
        musicOn = !musicOn;
        if (!musicOn)
        {
            music.Pause();
        }
        else
        {
            music.UnPause();
        }
        return musicOn;
    }

    public void LoadSound(bool state)
    {
        soundOn = state;
    }

    public void LoadMusic(bool state)
    {
        musicOn = state;
        if (!musicOn)
        {
            music.Pause();
        }
        else
        {
            music.UnPause();
        }
    }


}
