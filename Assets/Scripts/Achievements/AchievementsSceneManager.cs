using UnityEngine;
using System.Collections;
using Microsoft.UnityPlugins;
using System.Collections.Generic;
#if USEXBOXLIVE 
using Microsoft.Plugins.XBL;
#endif 
using MonsterGame;

public class AchievementsSceneManager : MonoBehaviour {

    void Awake ()
    {
        Debug.Log("AchievementsSceneManager"); 
    }

    // Use this for initialization
    void Start() {
        if (GameState.Instance.User.IsLoggedIn)
        {             
            Debug.Log("Getting achievements");
#if USEXBOXLIVE
            GameState.Instance.User.GetAchievements(OnAchievementsComplete);              
#endif 
            }                       
    }
	
	// Update is called once per frame
	void Update () {
	
	}


#if USEXBOXLIVE
    void OnAchievementsComplete ( CallbackResponse<IList<AchievementData>> response )
    {
        Debug.Log("OnAchievementsComplete");
        if(response.Status == CallbackStatus.Success )
        {
            Debug.Log("Successs");
            var achievements = response.Result;

            var canvas =  GameObject.Find("AchievementsCanvas");
            var container = canvas.transform.FindChild("AchievementsContainer");
            GameObject prefab = Resources.Load("Prefabs/AchievementRoot") as GameObject;

            int row = 0;
            int column = 0;
            int rowSize = 300;
            int columnSize = 520;
            Vector2 startPosition = new Vector2(-860, 0);  

            foreach ( AchievementData ad in achievements )
            {
                var item = Instantiate(prefab);
                var renderer = item.GetComponent<AchievementRenderer>();
                item.transform.SetParent(container.transform);
                item.SetActive(true);
                var rectTransform = item.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = rectTransform.anchorMin;
                    rectTransform.anchoredPosition = new Vector2(startPosition.x + (column*columnSize),  startPosition.y - (row*rowSize));
                }
                renderer.Initialize(ad.Name, ad.Description, ad.Score, ad.ImageUrl);
                column++; 
                if ( column == 3 )
                {
                    row++;
                    column = 0; 
                }
            }
        }
    }
#endif
}
