//TODO: Copyright .. 

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.UnityPlugins;
#if USEXBOXLIVE
using Microsoft.Plugins.XBL;
#endif 
using MonsterGame;

public class MainMenuSceneManager : MonoBehaviour
{
    public AudioSource selectionChangedAudioSource;
    public AudioSource startButtonAudioSource;
	public GameObject backgroundMusic;

    public void Awake ()
    {

        ToggleLoggedInButtons(false); 
        
    }


    // Use this for initialization
    void Start ()
	{
        LogCallbackListener.Start();
        if ( GameState.Instance.User.NeedsLogin )
        {
#if USEXBOXLIVE
            IXboxLiveBridge bridge = XboxLiveBridge.Instance;
            if (bridge != null)
			    bridge.SignIn(OnSignInComplete);
#endif 
        }
    }


#if USEXBOXLIVE
    void OnSignInComplete(CallbackResponse<SignInResult> cbr)
	{

#if USEXBOXLIVE
        GameState.Instance.User = new XboxLiveUser (cbr.Result);  
#else
        GameState.Instance.User = new DummyUser();
#endif

        GameObject obj = GameObject.Find("GamertagText");
		if(obj != null)
		{
			Text text = obj.GetComponent<Text>();
			if(text != null)
				text.text = cbr.Result.User.Gamertag;
		}

		StartCoroutine(NavigationHelpers.GetSpriteFromUrl(cbr.Result.Profile.GameDisplayPictureResizeUri.ToString(), AssignSprite));

        ToggleLoggedInButtons(true);
    }
#endif 

	private void AssignSprite(Sprite sprite)
	{
		GameObject imgObj = GameObject.Find( Constants.MainMenu_GamerpicImage );
		if(imgObj != null)
		{
			Image img = imgObj.GetComponent<Image>();
			img.sprite = sprite;
			img.enabled = true;
		}
	}


    private void ToggleLoggedInButtons ( bool enabled )
    {
        var achievementsButton = GameObject.Find(Constants.MainMenu_AchievementsButton).GetComponent<Button>();
        var leaderBoardButton = GameObject.Find(Constants.MainMenu_LeaderBoardButton).GetComponent<Button>();
        achievementsButton.interactable = enabled;
        leaderBoardButton.interactable = enabled;
        GameObject.Find(Constants.MainMenu_GamerTagText).GetComponent<Text>().enabled = enabled;
        GameObject.Find(Constants.MainMenu_GamerpicImage).GetComponent<Image>().enabled = enabled;

        if (!enabled)
        {
            achievementsButton.image.color = Constants.DisabledButtonColorMask;
            leaderBoardButton.image.color = Constants.DisabledButtonColorMask; 
        }
        else
        {
            achievementsButton.image.color = Constants.EnabledButtonColorMask;
            leaderBoardButton.image.color = Constants.EnabledButtonColorMask;

        }
    }


	IEnumerator PlayStart()
    {
		AudioSource src = backgroundMusic.GetComponent<AudioSource>();
		src.Stop();

        startButtonAudioSource.Play();

        yield return new WaitForSeconds (startButtonAudioSource.clip.length);

        Navigate("MazeLevel"); 

        yield return null; 
    }

    public void OnStartButtonClicked ()
    {
        Debug.Assert(startButtonAudioSource != null); 
		StartCoroutine(PlayStart());
    }

    public void OnLeaderBoardButtonClicked ()
    {
        Navigate("Leaderboard");  
    }

    public void OnAchievementsButtonClicked ()
    {
        Navigate("Achievements"); 
    }

    public void OnSettingsClicked ()
    {
        Navigate("PlayerSettings"); 
    }

    private void Navigate ( string newScene )
    {
        PlayerPrefs.SetInt("previousLevel", 0);
        Application.LoadLevel( newScene );
    }
    public void OnStoreClicked ()
    {
        Navigate("Store"); 
    }

    public void OnSelectionChanged ()
    {
       
        selectionChangedAudioSource.Play(); 
    }  
}

