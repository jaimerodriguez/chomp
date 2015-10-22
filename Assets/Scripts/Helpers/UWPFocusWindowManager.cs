using UnityEngine;
using System.Collections;
using MonsterGame;

#if REMOVEME 
public class UWPFocusWindowManager : MonoBehaviour
{
    GameState.GameStatus status;  
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnApplicationFocus(bool focus)
    {
        Debug.Log("Focus changed" + focus);
        if (!focus)
        {
            status = GameState.Instance.GetState();
#if USEPAUSE
            GameState.Instance.SetState(GameState.GameStatus.Paused);            
#endif 
        } 
        else
        {             
            GameState.Instance.SetState(status);
        }

    }

    public void OnApplicationQuit()
    {

    }

    public void OnApplicationPause(bool pause)
    {

    }
}
#endif 
