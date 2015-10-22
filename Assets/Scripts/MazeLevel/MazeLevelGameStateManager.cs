#define SHOWADS  

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MonsterGame;
using Microsoft.UnityPlugins;
#if USEXBOXLIVE 
using Microsoft.Plugins.XBL;
#endif 

public class MazeLevelGameStateManager : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject getReady;
    public BoardManager boardManager;
    private int adsFrequency = 2; 

#if SHOWADS
    private IInterstittialAd ad; 
#endif

    // Use this for initialization
    void Start ()
	{
        GameState.Instance.StatusChanged += GameState_StatusChanged;        
        boardManager.InstantiateLevel(0, GameState.Instance.GetLevel(0), new DefaultMazeConfig( boardManager.transform.position ));         

        if ( !PlatformHelper.UseJoystick )
        {
            GameObject jsInput = GameObject.Find("JoystickInput");
            if (jsInput != null)
                jsInput.SetActive(false); 
        }


        if ( GameState.Instance.User.IsLoggedIn )
        {
           GameObject.Find(Constants.Maze_NameText).GetComponent<Text>().text = GameState.Instance.User.Gamertag;            
        }
        else
        {
            GameObject.Find(Constants.Maze_NameText).SetActive(false); 

        }


#if UNITY_EDITOR
        MicrosoftAdsBridge.InterstitialAdFactory = new EditorAdFactory(); 
#endif
	}

    private void GameState_StatusChanged(object sender, GameStatusChangedEventArgs e)
    {         
       if ( e.NewState == GameState.GameStatus.GetReadyCompleted )
        {            
            Debug.Assert(e.OldState == GameState.GameStatus.GetReady); 
             GotoPlaying(); 
        }
        else if ( e.NewState == GameState.GameStatus.Restarting )
        {            
            SoundManager.Instance.PlayLifeLost();
        }
        else if ( e.NewState == GameState.GameStatus.GameOver )
        {
            SoundManager.Instance.PlayGameOver();              
        }
        else if ( e.NewState == GameState.GameStatus.Win )
        {       
            boardManager.BroadcastMessage("WinAchieved");
           //NOTE: it is important that we do PLayWin () as that triggers next step..  (lame!!)                                
#if SHOWADS
            if ((GameState.Instance.CurrentLevel % adsFrequency ) == (adsFrequency-1))
            {
                SoundManager.Instance.PlayWin();
                if (MicrosoftAdsBridge.InterstitialAdFactory != null)
                {
                    if (ad == null )
                    {
                        ad = MicrosoftAdsBridge.InterstitialAdFactory.CreateAd();
                        ad.AddCallback(AdCallback.Completed, adCompleted);
                        ad.AddCallback(AdCallback.Cancelled, adCancelled); 
#if UNITY_EDITOR
                        EditorInterstitialAd eiad = ad as EditorInterstitialAd;
                        if (eiad != null)
                        {
                            eiad.canvas = GameObject.Find("Hud").GetComponent<Canvas>();
                            eiad.syncMonoBehaviour = this; 
                        }
                        

#endif
                    }
                    ad.Request (appId, GetNextAdUnitId(AdType.Video ), AdType.Video); 
                }
            }          
            else if ( (GameState.Instance.CurrentLevel % adsFrequency == 0) && (ad != null)  && (ad.State == InterstitialAdState.Ready))
            {
                GameState.Instance.SetState(GameState.GameStatus.PlayingAdvertisement);
                SoundManager.Instance.PauseBackgroundMusic();
                ad.Show();
                
            }
            else
#endif
            {                
                SoundManager.Instance.PlayWin();
            }
        }
        else if (e.NewState == GameState.GameStatus.AdvertisementCompleted || 
                 e.NewState == GameState.GameStatus.WinCompleted)
        {
            if (e.NewState == GameState.GameStatus.AdvertisementCompleted) 
                SoundManager.Instance.UnPauseBackgroundMusic();

            int next = GameState.Instance.GetNextLevel();
            boardManager.InstantiateLevel(next, GameState.Instance.GetLevel(next), new MonsterGame.DefaultMazeConfig(boardManager.transform.position));
            GotoGetReady();
            HandleLevelCompleted(next);
        } 
        else if (e.NewState == GameState.GameStatus.Paused)
        {
#if USEPAUSE
            if (!isPauseShowing && hasGameStarted )
            {
                Debug.Log("Show pause");
                Application.LoadLevelAdditive("Pause");
                isPauseShowing = true;  
            }
#endif
        }
        else if (e.NewState == GameState.GameStatus.WinCompleted)
        {
           
        }
        else if (e.NewState == GameState.GameStatus.LifeLostCompleted)
        {
            Debug.Assert(GameState.Instance.HasLives);
            boardManager.UseNextLive();
            GotoGetReady();
        }
        else if (e.NewState == GameState.GameStatus.GameOverCompleted)
        {
            GoBack();
        }


        if (e.OldState == GameState.GameStatus.Paused)
        {
#if USEPAUSE
            if (isPauseShowing)
            {
                Application.UnloadLevel("Pause");
            } 
            isPauseShowing = false;
#endif
        } 

        
    }

    bool isPauseShowing = false;
    bool hasGameStarted = false; 

    // Update is called once per frame
    void Update ()
    {
        if (!GameState.Instance.IsRunning)
        {
            if (GameState.Instance.IsLoading)
            {
                GotoGetReady(); 
                return;
            }             
            else if (GameState.Instance.IsGameOver)
            {
                if (!gameOver.activeSelf)
                {
                    gameOver.SetActive(true);
                }                 
            }
            else if ( GameState.Instance.IsGameOverCompleted )
            {
                GoBack();   
            }            
            else if (GameState.Instance.IsLifeLostCompleted )
            {
                //Debug.Assert(GameState.Instance.HasLives); 
                //boardManager.UseNextLive();
                //GotoGetReady(); 
            }
            else if ( GameState.Instance.IsWinCompleted )
            {
               

            }
            else if (GameState.Instance.IsWin)
            {

            }           
        }

#if DEBUG
        if ( Input.GetKeyUp ( KeyCode.F5 ))
        {
            var hud = GameObject.FindGameObjectWithTag ("DebugHud");
            if (hud != null)
            {
                var panel = hud.transform.GetChild(0); 
                if ( panel != null && panel.gameObject != null )
                {
                    panel.gameObject.SetActive(!panel.gameObject.activeSelf);  
                }
            } 
        }
        else if (Input.GetKeyUp (KeyCode.F12 ))
        {
            var enemy = GameObject.Find("Enemy"); 
            if ( enemy  != null )
            {
                GameState.Instance.IsInvincible = !GameState.Instance.IsInvincible; 
                var collider = enemy.GetComponent<BoxCollider2D>();
                collider.enabled = !collider.enabled; 
            }
        }
#endif
        else if (Input.GetKeyUp (KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;

#if UNITY_WSA_10_0XX
                    
                if (Microsoft.UnityPlugins.Core.WindowingHelpers.GetIsFullScreen())
                {
                    Microsoft.UnityPlugins.Core.WindowingHelpers.ExitFullScreen();
                }
                else
                {
                    Microsoft.UnityPlugins.Core.WindowingHelpers.TryEnterFullScreenMode();
                }
#endif
            }            
            else if ( Input.GetKeyUp (KeyCode.Escape))
            {

                GoBack(); 
            }                    
    }


#if SHOWADS
    void adCompeted(object ununsed)
    {
        ad.Dispose();
        ad = null; 
    }
    private const string appId = "d25517cb-12d4-4699-8bdc-52040c712cab";
    private string adUnitId = "11389925";

    string GetNextAdUnitId(AdType type)
    {
        return adUnitId; 
    }

    void adCompleted(object e)
    {
        GameState.Instance.SetState(GameState.GameStatus.AdvertisementCompleted); 
    }

    void adCancelled (object e)
    {
        GameState.Instance.SetState(GameState.GameStatus.AdvertisementCompleted);
    }
#endif 

    void HandleLevelCompleted(int nextLevel)
    {
 
        int previousLevelScore = GameState.Instance.Score;
        string msg = string.Format(Constants.Display_NextLevelAwaits, nextLevel);
        Microsoft.UnityPlugins.Tiles.UpdateTile(
            TileTemplateType.TileSquareText02,
            new string[] { Constants.Display_AppName, msg });

#if !UNITY_EDITOR
        Microsoft.UnityPlugins.RoamingSettings.SetValueForKey(Constants.RoamingSettings_NextLevel, nextLevel );
        object score = Microsoft.UnityPlugins.RoamingSettings.GetValueForKey(Constants.RoamingSettings_MaxScore);

        if (score != null)
        {
            int maxScore = (int)score;
            if (previousLevelScore > maxScore)
                Microsoft.UnityPlugins.RoamingSettings.SetValueForKey(Constants.RoamingSettings_MaxScore, score);
        }
        else
        {
            Microsoft.UnityPlugins.RoamingSettings.SetValueForKey(Constants.RoamingSettings_MaxScore, previousLevelScore);
        }
#endif 
    }

    void GoBack()
    {
        int previous = PlayerPrefs.GetInt("previousLevel");
        Application.LoadLevel(previous);
    }
    void GotoGetReady()
    {
        getReady.SetActive(true);
        getReady.GetComponent<Animator>().SetTrigger("Reset");            
        SoundManager.Instance.PlayGetReady();
        getReady.GetComponent<Animator>().SetTrigger("Fade"); 
        GameState.Instance.SetState(GameState.GameStatus.GetReady);
    }

    void GotoPlaying ()
    {
        hasGameStarted = true; 
        Debug.Assert(getReady != null);
        try
        {
            //TODO: state is corrupt..why ?? this  
            getReady.SetActive(false);
        }
		catch ( System.Exception /*ex*/ )
        {
            Debug.Assert(false);           
        }
        GameState.Instance.SetState(GameState.GameStatus.Playing);
        SoundManager.Instance.PlayGamePlay();
    }
}
