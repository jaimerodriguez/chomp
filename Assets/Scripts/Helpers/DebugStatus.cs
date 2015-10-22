using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MonsterGame;

public class DebugStatus : MonoBehaviour {
#if DEBUG 
    // Use this for initialization

    Text pillsLeft;
    Text prizesLeft;
    Text playerDirection;
    Text monsterDirection;
    Text invincible;
    Text gameStatus;
    bool isInitialized = false; 

    protected class Constants { 
        public const string PillsLeft = "PillsLeft";
        public const string PrizesLeft = "PrizesLeft";
        public const string PlayerDirection = "PlayerDirection";
        public const string MonsterDirection = "MonsterDirection";
        public const string Invincible = "Invincible";
        public const string GameStatus = "GameStatus";
    } 

    void Start () {
        
        
    }

    bool  Initialize ()
    {
        GameObject panel = GameObject.Find("DebugHudPanel");
        if (panel != null )
        {
            Text[] elements = panel.GetComponentsInChildren<Text>();
            foreach (Text item in elements)
            {
                if (item.name == Constants.PillsLeft)
                    pillsLeft = item;
                else if (item.name == Constants.PrizesLeft)
                    prizesLeft = item;
                else if (item.name == Constants.PlayerDirection)
                    playerDirection = item;
                else if (item.name == Constants.MonsterDirection)
                    monsterDirection = item;
                else if (item.name == Constants.Invincible)
                    invincible = item;
                else if (item.name == Constants.GameStatus)
                {
                    gameStatus = item;
                }
            }           
            return isInitialized = true;             
        }
        return false; ; 
    }


	
	// Update is called once per frame
	void Update () {
       
        if (!isInitialized)
        {
            if (!Initialize())
                return;
        }

        Debug.Assert(pillsLeft != null && prizesLeft != null && playerDirection != null && monsterDirection != null &&
            gameStatus != null && invincible != null);   
        pillsLeft.text = string.Format("Pills: {0}", GameState.Instance.PillsLeft);
        prizesLeft.text = string.Format("Prizes: {0}", GameState.Instance.PrizesLeft);
        playerDirection.text = string.Format("PlayerDirection: {0}", GameState.Instance.PlayerDirection.ToString());
        monsterDirection.text  = string.Format("MonsterDirection: {0}", GameState.Instance.MonsterDirection.ToString());
        invincible.text = string.Format ("Invincible(f11): {0} ", GameState.Instance.IsInvincible ? "Yes" : "No" );

        gameStatus.text = string.Format("Status: {0}", GameState.Instance.GetState()); 

    }

#endif 
}
