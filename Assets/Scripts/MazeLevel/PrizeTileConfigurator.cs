using UnityEngine;
using System.Collections;

public class PrizeTileConfigurator : MonoBehaviour {

    public bool IsRandom = true;
    public Color color;
    public Sprite Sprite;
    // Use this for initialization
    void Start () {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = RandomPrizeConfigurator.Instance.GetNextSprite();
        spriteRenderer.color = RandomPrizeConfigurator.Instance.GetNextColor(); 
	}
	
	// Update is called once per frame
	void Update () {
	

	}


    private class RandomPrizeConfigurator
    {
        private static RandomPrizeConfigurator instance;
        string[] spritesToUse;
        Color[] colorsToUse;  
        int currentIndex; 
        static public RandomPrizeConfigurator Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = new RandomPrizeConfigurator(); 
                }
                return instance; 
            }
        }

        public RandomPrizeConfigurator Create ( bool singleUse , string [] sprites )
        {
            Debug.Assert(instance == null); 
            instance = new RandomPrizeConfigurator(singleUse, sprites);
            return instance; 
        }

        private RandomPrizeConfigurator(bool singleUse = true, string[] sprites = null)
        {
            if (sprites == null)
            {
                spritesToUse = new string[]
                {
                    "Sprites/Features/PPAchievements" ,
                    "Sprites/Features/PPActivity" ,
                    "Sprites/Features/PPApp2App" ,
                    "Sprites/Features/PPAvatars" ,
                    "Sprites/Features/PPCastingChat" ,
                    "Sprites/Features/PPDesktop" ,
                    "Sprites/Features/PPDesktop" ,
                    "Sprites/Features/PPGamepad" ,
                    "Sprites/Features/PPHeadPhones" ,
                    "Sprites/Features/PPHeadset" ,
                    "Sprites/Features/PPInputPane" ,
                    "Sprites/Features/PPNotifications" ,
                    "Sprites/Features/PPPCGames" ,
                    "Sprites/Features/PPSharing" ,
                    "Sprites/Features/PPStore"                                           
                }; 
            }
            EnforceSingleUse = singleUse;
            currentIndex = 0;

            colorsToUse = new Color[]
            {
                Color.red , 
                Color.green , 
                Color.magenta, 
                Color.yellow , 
                Color.white                  
            }; 
        }

        
        public bool EnforceSingleUse { get; set;  }

        public Sprite GetNextSprite ()
        {
            if (spritesToUse == null || spritesToUse.Length == 0)
                return null;

            string path = spritesToUse[currentIndex++ % spritesToUse.Length];
            Sprite sprite = Resources.Load<Sprite>(path);            
            Debug.Assert(sprite != null); 
            return sprite; 
        }         

        public Color GetNextColor ()
        {
            if (colorsToUse == null || colorsToUse.Length == 0)
                return Color.green;

            return colorsToUse[currentIndex % colorsToUse.Length]; 

        }
    }
}
