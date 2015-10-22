using UnityEngine;
using System.Collections;

public class BackButtonHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if ( Input.GetKeyUp ( KeyCode.Escape ))
        {
            NavigationHelpers.GoBack(); 
        }          
	}

    public void OnBackButtonClicked ( )
    {
        NavigationHelpers.GoBack(); 
    }
}
