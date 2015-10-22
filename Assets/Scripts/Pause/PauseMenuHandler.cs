using UnityEngine;
using System.Collections;

public class PauseMenuHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnResumeClicked ()
    {
        MonsterGame.GameState.Instance.UnPause(); 
    }

    public void OnExitClicked ()
    {
        NavigationHelpers.GoBack(); 
    }
}
