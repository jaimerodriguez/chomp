using UnityEngine;
using System.Collections;
using MonsterGame;

public class AudioSourceVolumePreferencesHelper : MonoBehaviour {

    public string layer; 
	// Use this for initialization
	void Start () {
        ApplyUserPreferences();  
	}

    void ApplyUserPreferences ()
    {
        if (!string.IsNullOrEmpty (layer))
        {
            AudioSource source = this.GetComponent<AudioSource>();
            Debug.Assert(source != null); 

            if ( layer == "background")
            {
                source.volume = Preferences.Current.MusicVolume; 
            }
            else if ( layer == "fx")
            {
                source.volume = Preferences.Current.FXVolume;  
            }
        }
    }
	 
}
