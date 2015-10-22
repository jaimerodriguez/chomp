//TODO: Copyright 

using UnityEngine;
using System.Collections;
using MonsterGame;

public class BoardManager : MonoBehaviour {


    
    public GameObject prizePrefab;   
    public GameObject enemy;
    public GameObject player;
    public GameObject pillTile;
    public GameObject BackgroundTile;
    
    public int Pills { get; private set; } 
    public int Prizes { get; private set; }  

    private System.Random outerWallRandom, obstacleTilesRandom ;
    private bool isPreloaded = false;
    private bool isInitialized = false;
    private GameObject[] outerWallTiles;
    private GameObject[] obstacleTiles;

    private float topOffset;
    

    Vector3 playerStartingPosition;
    Vector3 enemyStartingPosition; 


    private Transform boardTransform;

    void Awake()
    {
        if (!isPreloaded)
            Prepopulate();

        boardTransform = this.GetComponent<Transform>(); 
       
    }

    void Prepopulate ()
    {
        lock ( this )
        {
            outerWallRandom = new System.Random();
            obstacleTilesRandom = new System.Random();
            outerWallTiles = new GameObject[5];
            obstacleTiles = new GameObject[5];        
            LoadSpriteSeries("Prefabs/WallTile2", "Sprites/Maze/WallGray", outerWallTiles);
            LoadSpriteSeries("Prefabs/ObstacleTile", "Sprites/Maze/WallBlue", obstacleTiles);  

            isPreloaded = true; 
        } 
    }

    void ResetLevelData ()
    {
        Prizes = 0;
        Pills = 0;
    }

    void LoadSpriteSeries ( string prefabPath , string spritePath, GameObject[] target  )
    {        
        GameObject prefab = Resources.Load(prefabPath ) as GameObject;

        for (int i = 0; i < target.Length; i++)
        {
            string spritePathIndexed = string.Format("{0}{1}", spritePath, i + 1);
            Debug.Log(spritePathIndexed);
            var resource = Resources.Load<Sprite>(spritePathIndexed); 
            Sprite sprite =  resource as Sprite;
            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            target[i] = prefab;
        }
    }
	// Use this for initialization
	void Start () {        
        CenterBoard(); 
       
	}

    void CenterBoard ()
    {
        var worldTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        var worldBottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        float width =  GameState.Instance.MazeConfiguration.TileWidth * GameState.Instance.MazeConfiguration.NumColumns ;
        float height = GameState.Instance.MazeConfiguration.TileHeight * GameState.Instance.MazeConfiguration.NumRows ;

        Vector3 center = new Vector3(((worldBottomRight.x - worldTopLeft.x) - width) / 2, ((worldTopLeft.y - worldBottomRight.y) - height) / 2, -Camera.main.transform.position.z);
        center.y *= -1;
        worldTopLeft += center;
        this.transform.position = worldTopLeft;
        GameState.Instance.MazeConfiguration.WorldPosition = worldTopLeft; 
    }
	

    void RemoveChildren()
    {
        foreach ( Transform child in transform )
        {
            if (child.tag == "Player" || child.tag == "Enemy")
                continue;
            else
                Destroy(child.gameObject);        
        }


    }
    public void InstantiateLevel(int levelIndex, ILevel level, MazeConfig config)
    {
        RemoveChildren();
        ResetLevelData();
       

        float height = config.TileHeight;
        float width = config.TileWidth;
        float verticalDirection = config.VerticalDirection; 
         
        for (int row = 0; row < config.NumRows; row++)
        {
            for (int col = 0; col < config.NumColumns; col++)
            {
                TileType tileType = (TileType)level.Data[row, col];
                GameObject newTile = null ; 
                Vector3 newPosition = new Vector3 ( (float)col * width, (float)row * height * verticalDirection, 0f); 
                switch (tileType)
                {
                    case TileType.PathWithDot:                         
                        newTile = Instantiate(pillTile, newPosition, Quaternion.identity) as GameObject;                         
                        Pills++;                                            
                        break;
                    case TileType.PathWithoutDot:                         
                        newTile = Instantiate(BackgroundTile, newPosition , Quaternion.identity) as GameObject;
                        break;
                    case TileType.BlueWall:
                        newTile = Instantiate(obstacleTiles[obstacleTilesRandom.Next(0, obstacleTiles.Length)], newPosition, Quaternion.identity) as GameObject;                              
                        break;
                    case TileType.GrayWall:
                        newTile = Instantiate(outerWallTiles[outerWallRandom.Next(0, outerWallTiles.Length-1)],
                        newPosition, Quaternion.identity) as GameObject;
                    break;
                    case TileType.PlayerStart:
                        newTile = Instantiate ( BackgroundTile, newPosition, Quaternion.identity) as GameObject;
                        newTile.transform.SetParent(boardTransform, false);
                        newTile = player;
                        newTile.transform.SetParent(null);
                        playerStartingPosition = newPosition;  
                        newTile.transform.position = playerStartingPosition; 
                        break;
                    case TileType.PathWithIcon:                       
                        newTile = Instantiate(prizePrefab ,
                        newPosition, Quaternion.identity) as GameObject;
                        Prizes++;  
                        break; 
                    case TileType.EnemyStart:                         
                        newTile = Instantiate(BackgroundTile, newPosition, Quaternion.identity) as GameObject;
                        newTile.transform.SetParent(boardTransform, false);
                        newTile = enemy;
                        newTile.transform.SetParent(null);
                        enemyStartingPosition = newPosition; 
                        newTile.transform.position = enemyStartingPosition; 
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, "Invalid tile type");
                        break; 

                }
                if ( newTile != null )
                    newTile.transform.SetParent(boardTransform, false );


            }
        }

        config.Data = level.Data; 
        GameState.Instance.InitLevel(levelIndex, Pills, Prizes, config);
        BroadcastMessage("LevelReset");
    }


    public void UseNextLive ()
    {
        Debug.Assert(enemy != null && player != null); 
        enemy.transform.SetParent(null);         
        enemy.transform.position = enemyStartingPosition;
        enemy.transform.SetParent(boardTransform, false);

        //TODO: this might be better served via BroadCastMesage ("LevelReset") but that does more 
        // work on player and enemy ... so not broadcasting for now. 
        //  player.GetComponent<Player>().ResetPlayerState();
        BroadcastMessage("CharacterReset");

        player.transform.SetParent(null);
        player.transform.position = playerStartingPosition;
        player.transform.SetParent(boardTransform, false);

        
    }

}
