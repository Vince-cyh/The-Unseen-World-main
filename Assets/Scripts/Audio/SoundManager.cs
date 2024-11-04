using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioSource audioSrc;
    public static AudioClip pickup;
    public static AudioClip LevelUp;
    public AudioSource BackgroundMusic;
    public AudioSource BattleBGM;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        pickup = Resources.Load<AudioClip>("Pickup");
        LevelUp = Resources.Load<AudioClip>("LevelUp");
        PlayMusic();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void PlayPickUp()
    {
        audioSrc.PlayOneShot(pickup);
    }

    public static void PlayLevelUp()
    {
        audioSrc.PlayOneShot(LevelUp);
    }

    public void PlayMusic()
    {
        if (!BackgroundMusic.isPlaying)
        {
            BackgroundMusic.Play();
        }
    }

    public void StopMusic()
    {
        if (BackgroundMusic.isPlaying)
        {
            BackgroundMusic.Stop();
        }
    }

    public void PauseMusic()
    {
        if (BackgroundMusic.isPlaying)
        {
            BackgroundMusic.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!BackgroundMusic.isPlaying)
        {
            BackgroundMusic.UnPause();
        }
    }

    public void PlayBattleMusic()
    {
        if (!BattleBGM.isPlaying)
        {
            BattleBGM.Play();
        }
    }

    public void StopBattleMusic()
    {
        if (BattleBGM.isPlaying)
        {
            BattleBGM.Stop();
        }
    }

    public void PauseBattleMusic()
    {
        if (BattleBGM.isPlaying)
        {
            BattleBGM.Pause();
        }
    }

    public void ResumeBattleMusic()
    {
        if (!BattleBGM.isPlaying)
        {
            BattleBGM.UnPause();
        }
    }
}
