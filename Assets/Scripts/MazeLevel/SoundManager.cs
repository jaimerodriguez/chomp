//TODO: Copyright 
//TODO: cleanup 
using UnityEngine;
using System.Collections;
using MonsterGame;

public class SoundManager : MonoBehaviour
{

    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager Instance = null;
    public AudioClip gameOver;
    public AudioClip getReady;
    public AudioClip hit;
    public AudioClip eat;
    public AudioClip walk;
    public AudioClip win;
    public AudioClip lifeLost; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            //TODO: 
            Debug.Assert(false, "when does this happen"); 
 //           Destroy(gameObject);
        } 
    }
    // Use this for initialization
    void Start()
    {
      ApplyNewPreferences(); 
    }

    public void ApplyNewPreferences()
    {
      efxSource.volume = Preferences.Current.FXVolume;
      musicSource.volume = Preferences.Current.MusicVolume;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBackgroundMusic()
    {
        musicSource.Play(); 
    }
    public void PauseBackgroundMusic()
    {
        musicSource.Pause();
    }
    public void UnPauseBackgroundMusic()
    {
        musicSource.UnPause();
    }

    void PlayOnce(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();         
    }


    IEnumerator PlayOnce(AudioClip clip, bool stopMusic , MonsterGame.GameState.GameStatus nextState )
    {
        efxSource.clip = clip;
        efxSource.Play();

        bool wasPlayingMusic = musicSource.isPlaying; 

        if (stopMusic)
            musicSource.Pause();


        yield return new WaitForSeconds (efxSource.clip.length);

        if ( nextState != GameState.GameStatus.Unknown )
            GameState.Instance.SetState(nextState);

        if ( stopMusic && wasPlayingMusic )
            musicSource.Play(); 

        yield return null; 
    }


    public void PlayEat()
    {
        PlayOnce(eat);
    }

    public void PlayHit()
    {
        PlayOnce(hit);
    }

    public void PlayMove()
    {
        if (!efxSource.isPlaying)
            PlayOnce(walk);
    }

    public void PlayLifeLost()
    {         
        GameState.Instance.SetState(GameState.GameStatus.LifeLost); 
        StartCoroutine(PlayOnce(lifeLost, true, GameState.GameStatus.LifeLostCompleted ));
    }

    public void PlayGetReady()
    {
        // efxSource.PlayOneShot(ready);         
        StartCoroutine(PlayOnce(getReady, true, GameState.GameStatus.Unknown)); 
    }

#if REMOVE
    public bool IsPlayingWinMusic
    {
        get
        {
            bool ret = (efxSource.clip == win && efxSource.isPlaying);
            return ret;
        }
    }

    public bool IsPlayingReadyMusic
    {
        get
        {
            bool ret = (efxSource.clip == getReady && efxSource.isPlaying);
            return ret;
        }
    }

    public bool IsPlayingGameOverMusic
    {
        get
        {
            bool ret = (efxSource.clip == gameOver && efxSource.isPlaying);
            return ret;
        }
    }


#endif 

    public void PlayWin()
    {        
        StartCoroutine(PlayOnce(win , true, GameState.GameStatus.WinCompleted));
    }

    public void PlayGameOver()
    {
        //   PlayOnce(gameOver);
        StartCoroutine(PlayOnce(gameOver, true, GameState.GameStatus.GameOverCompleted)); 
    }

    public void PlayGamePlay()
    {
        musicSource.Play();
    }

    public void StopGamePlay()
    {
        musicSource.Stop();
    }

    public void OnApplicationFocus(bool focus)
    {
        Debug.Log("OnApplicationFocus: " + focus); 
        FocusChanged(focus); 
    }

    public void OnApplicationPause(bool pause)
    {         
        FocusChanged(!pause); 
    }

public void FocusChanged(bool isFocused)
{
    if (isFocused)
    {
        musicSource.UnPause();             
    }
    else
    { 
        musicSource.Pause();
#if USEPAUSE
            GameState.Instance.SetState(GameState.GameStatus.Paused); 
#endif 
    }
}

    public void ResumeFocus()
    {
        musicSource.UnPause();
    }
}
