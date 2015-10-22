using UnityEngine;
using System.Collections;
using MonsterGame;

public class GetReadyHud : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void GetReadyFadeOutCompleted ()
    {
        GameState.Instance.SetState(GameState.GameStatus.GetReadyCompleted); 
    }
}
