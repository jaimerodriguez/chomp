using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MonsterGame;

public class PreferencesSceneManager : MonoBehaviour
{   
    internal const string TogglePostfix = "Toggle";
    internal const string SliderPostfix = "Slider";


    // Use this for initialization
    void Start()
    {
        LoadAndSetSliderFloat( Preferences.FXVolumeKey, Preferences.Current.FXVolume );
        LoadAndSetSliderFloat( Preferences.MusicVolumeKey, Preferences.Current.MusicVolume );
        LoadAndSetSliderFloat( Preferences.PlayerSpeedKey, Preferences.Current.PlayerSpeed );
        LoadAndSetToggleBool ( Preferences.LiveTilesKey , Preferences.Current.UseLiveTiles );
        LoadAndSetToggleBool ( Preferences.XboxLiveKey, Preferences.Current.UseXboxLive );

        //Doing this from code just for kicks.. 
        GameObject playerSpeedObject = GameObject.Find(Preferences.PlayerSpeedKey + SliderPostfix);
        if (playerSpeedObject != null)
        {
            Slider slider = playerSpeedObject.GetComponent<Slider>();
            Debug.Assert(slider != null);
            Slider.SliderEvent evt = new Slider.SliderEvent();
            evt.AddListener(OnPlayerSpeedValueChanged);
        }

        if (!UserFactory.UseIdentity)
        {
            GameObject.Find( Preferences.XboxLiveKey + TogglePostfix  ).GetComponent<Toggle>().interactable = false; 
        }
    }


    private void LoadAndSetSliderFloat(string key , float value )
    { 
      GameObject go = GameObject.Find(key + SliderPostfix); Debug.Assert(go != null);
      UnityEngine.UI.Slider slider = go.GetComponent<Slider>(); Debug.Assert(slider != null);
      slider.value = value;         
    }

    private void LoadAndSetToggleBool(string key, bool value )
    {        
      GameObject go = GameObject.Find(key + TogglePostfix); Debug.Assert(go != null);
      Toggle toggle = go.GetComponent<Toggle>(); Debug.Assert(toggle != null);
      toggle.isOn = value; 
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F12))
        {
            NavigationHelpers.GoBack();
        }
    }

    public void OnFXValueChanged(float newValue)
    {
#if DEBUG 
        UnityEngine.Debug.Log(string.Format("Fx:{0}", newValue));
#endif 
        Preferences.Current.FXVolume = newValue; 
    }

    public void OnMusicValueChanged(float newValue)
    {
#if DEBUG 
        UnityEngine.Debug.Log(string.Format("Music:{0}", newValue));
#endif 
        Preferences.Current.MusicVolume = newValue; 
    }


    public void OnPlayerSpeedValueChanged(float newValue)
    {
#if DEBUG 
        UnityEngine.Debug.Log(string.Format("PlayerSpeed:{0}", newValue));
#endif 
        Preferences.Current.PlayerSpeed = newValue; 
    }

    public void OnXboxLiveChanged(bool newValue)
    {
#if DEBUG 
        UnityEngine.Debug.Log(string.Format("XboxLive:{0}", newValue));
#endif 
        Preferences.Current.UseXboxLive = newValue; 
    }

    public void OnLiveTilesChanged(bool newValue)
    {
#if DEBUG 
        UnityEngine.Debug.Log(string.Format("LiveTiles:{0}", newValue));
#endif          
        Preferences.Current.UseLiveTiles = newValue; 
    }

    public void OnDestroy()
    {
#if DEBUG 
        UnityEngine.Debug.Log("Saving Preferences");
#endif
      Preferences.Current.Save();  
        
            
    }
}
