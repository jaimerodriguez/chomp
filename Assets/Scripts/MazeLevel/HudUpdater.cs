//TODO: CopyRight 
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MonsterGame; 

public class HudUpdater : MonoBehaviour {

    // Use this for initialization

    UnityEngine.UI.Text score;
    UnityEngine.UI.Text lives;
    UnityEngine.UI.Text username;
    string cachedUserName; 

    int cachedLives = 0;  
    int cachedScore; 

	void Start () {

        var container = GameObject.Find("Score");
        Debug.Assert(container != null);
        score= container.GetComponent<Text>();
        container = GameObject.Find("Lives"); 
        lives = container.GetComponent<Text>(); 
	}
	
	// Update is called once per frame
	void Update () {
      
        if (GameState.Instance.IsRunning)
        {           
            if (cachedScore != GameState.Instance.Score)
            {
                cachedScore = GameState.Instance.Score;
                score.text = cachedScore.ToString();
            }
        }
        else if ( GameState.Instance.IsLifeLost )
        {
            if (cachedLives != GameState.Instance.Lives)
            {
                cachedLives = GameState.Instance.Lives;
                lives.text = string.Format("x {0}", cachedLives);
            }
        } 

	}
}
