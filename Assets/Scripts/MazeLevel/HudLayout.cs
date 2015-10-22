using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MonsterGame;

public class HudLayout : MonoBehaviour {

    public GameObject centerBoard;
    public GameObject background;
    private float screenWidth;
    private float screenHeight;
    private bool needsChange; 
	// Use this for initialization
	void Start () {

        needsChange = true;
        screenWidth = screenHeight = 0.0f; 
    }
	
	// Update is called once per frame
	void Update () {
        
        if (this.screenHeight != Screen.height ||
             this.screenWidth != Screen.width)
            needsChange = true;

        if (needsChange && centerBoard != null)
        {
            Vector3 boardPosition = centerBoard.transform.position;
            Vector3 boardPositionTopLeft = boardPosition;

            var canvasTransform = GetComponent<RectTransform>(); 
           
            float canvasWidth = canvasTransform.rect.width ;
            float canvasHeight = 720f; // / canvasTransform.localScale.y;


            float boardHeight = GameState.Instance.MazeConfiguration.Height;  
            float boardWeight = GameState.Instance.MazeConfiguration.Width;
            float height, width;
            height = Screen.height;
            width = Screen.width;
            Vector3 boardPositionScreen = Camera.main.WorldToScreenPoint(boardPositionTopLeft);
            Vector3 boardPositionViewport = Camera.main.WorldToViewportPoint(boardPositionTopLeft);
              
                
            var gameObject = GameObject.Find(Constants.Maze_ScoreText );
            Text score = gameObject.GetComponent<Text>();
            RectTransform scoreTransform = score.GetComponent<RectTransform>();
            //Aligns top left so  x->   and y is negative offset from top.. 

            // scoreTransform.rect.xMin =  boardPositionViewport.x * canvasWidth; 
            // scoreTransform.rect.y = boardPositionViewport.y * canvasHeight;
            scoreTransform.anchoredPosition = new Vector2(boardPositionViewport.x * canvasWidth, 
                                                    ( boardPositionViewport.y * canvasHeight )   + scoreTransform.rect.height/2 ); 
     


            gameObject = GameObject.Find( Constants.Maze_NameText );
            //it is possible for this to not be Active if we are not using login 
            if (gameObject != null)
            {
                Vector3 boardPositionTopRight = new Vector3(boardPositionTopLeft.x + boardWeight, boardPositionTopLeft.y, boardPositionTopLeft.z);
                boardPositionViewport = Camera.main.WorldToViewportPoint(boardPositionTopRight);
                Text name = gameObject.GetComponent<Text>();
                RectTransform nameTransform = name.GetComponent<RectTransform>();

                //Alighs top right 
                nameTransform.anchoredPosition =
                    new Vector2((boardPositionViewport.x * canvasWidth) - nameTransform.rect.width,
                                (boardPositionViewport.y * canvasHeight) + scoreTransform.rect.height / 2);

            } 
            gameObject = GameObject.Find(Constants.Maze_MonsterCountText);
            Vector3 boardPositionBottomLeft = new Vector3(boardPositionTopLeft.x, boardPositionTopLeft.y - boardHeight, boardPositionTopLeft.z);
            boardPositionViewport = Camera.main.WorldToViewportPoint(boardPositionBottomLeft);             
            Image monster = gameObject.GetComponent<UnityEngine.UI.Image>();
            RectTransform monsterTransform = gameObject.GetComponent<RectTransform>();

            
            monsterTransform.anchoredPosition = 
                            new Vector2((boardPositionViewport.x * canvasWidth) ,
                            (boardPositionViewport.y * canvasHeight) - (monsterTransform.rect.height/2) );


            gameObject = GameObject.Find(Constants.Maze_LivesText);

            Text lives = gameObject.GetComponent<Text>();
            RectTransform livesTransform = lives.GetComponent<RectTransform>();
            // Aligns bottom left so no need to map coords 
            livesTransform.anchoredPosition =  
                            new Vector2((boardPositionViewport.x * canvasWidth) + monsterTransform.rect.width + 10,
                            (boardPositionViewport.y * canvasHeight) - (monsterTransform.rect.height / 2));
             


            needsChange = false;
            this.screenHeight = Screen.height;
            this.screenWidth = Screen.width; 
        }
    }

 }
