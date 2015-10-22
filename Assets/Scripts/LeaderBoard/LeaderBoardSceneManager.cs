using UnityEngine;
using System.Collections;
using Microsoft.UnityPlugins;
using System.Collections.Generic;
#if USEXBOXLIVE
using Microsoft.Plugins.XBL;
#endif 
using MonsterGame;

public class LeaderBoardSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

        if (GameState.Instance.User.IsLoggedIn)
        {             
            Debug.Log("Getting achievements");
#if USEXBOXLIVE
            ((XboxLiveUser)GameState.Instance.User).GetLeaderboard( Constants.Achievements_LeaderBoardName, OnLeaderboardComplete);             
        
#endif 
            }        
    }

#if USEXBOXLIVE
    void OnLeaderboardComplete(CallbackResponse<LeaderboardData> response)
	{
		Debug.Log("OnLeaderboardComplete");
		if(response.Status == CallbackStatus.Success)
		{
			LeaderboardData ld = response.Result;
            GameObject prefab = Resources.Load("Prefabs/LeaderboardItemPanel") as GameObject;
            var canvas =  GameObject.Find("Canvas");
            var container = canvas.transform.FindChild("LeaderboardContainer");
			int row = 0;
			int rowSize = 50;

			foreach(LeaderboardItem li in ld.Items)
			{
				Vector2 startPosition = new Vector2(0, 0);  

				var item = Instantiate(prefab);
                var renderer = item.GetComponent<LeaderboardItemRenderer>();
                item.transform.SetParent(container.transform);
                item.SetActive(true);
                var rectTransform = item.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.anchorMin = new Vector2(0f, 1f);
                    rectTransform.anchorMax = rectTransform.anchorMin;
                    rectTransform.anchoredPosition = new Vector2( startPosition.x,  startPosition.y - (row*rowSize) );
                }
				renderer.Initialize(li.Rank.ToString(), li.Gamertag, li.Value);
				row++;
			}
		}
	}
#endif 
	
	// Update is called once per frame
	void Update () {
	
	}
}
